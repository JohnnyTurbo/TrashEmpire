using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TMG.TrashEmpire
{
    public class FindClosestTrashSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem ecbSystem;

        protected override void OnCreate()
        {
            ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = ecbSystem.CreateCommandBuffer();
            
            Entities
                .WithNone<MaxTrashHeldTag, TargetTrashData, PickingUpTrashData>()
                .ForEach((Entity e, in PatrolAreaData patrolAreaData, in Translation translation, in TrashCollectionData trashCollectionData) =>
                {
                    var trashInRange = GetBuffer<TrashBufferElement>(patrolAreaData.Value);
                    if (trashInRange.Length <= 0)
                    {
                        // Return trash to destination if you have any
                        // OR/THEN
                        // Navigate to patrol area & wait
                        return;
                    }

                    var closestTrash = Entity.Null;
                    var lowestDistance = float.MaxValue;
                    foreach (var trash in trashInRange)
                    {
                        if (GetComponent<TrashData>(trash).Weight > trashCollectionData.AvailableTrashCapacity)
                        {
                            continue;
                        }
                        var distanceToTrash = math.distance(translation.Value, GetComponent<Translation>(trash).Value);
                        if (distanceToTrash < lowestDistance)
                        {
                            lowestDistance = distanceToTrash;
                            closestTrash = trash;
                        }
                    }

                    if (closestTrash == Entity.Null)
                    {
                        ecb.AddComponent<MaxTrashHeldTag>(e);
                    }
                    else
                    {
                        ecb.AddComponent<TargetTrashData>(e);
                        ecb.SetComponent(e, new TargetTrashData {Value = closestTrash});
                    }
                    
                }).Run();
        }
    }
    
    public struct TargetTrashData : IComponentData
    {
        public Entity Value;
    }
}