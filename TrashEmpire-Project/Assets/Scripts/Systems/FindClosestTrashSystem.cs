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
                .ForEach((Entity e, in PatrolAreaData patrolAreaData, in Translation translation) =>
                {
                    var trashInRange = GetBuffer<TrashBufferElement>(patrolAreaData.Value);
                    if(trashInRange.Length <= 0){return;}

                    var closestTrash = Entity.Null;
                    var lowestDistance = float.MaxValue;
                    foreach (var trash in trashInRange)
                    {
                        var distanceToTrash = math.distance(translation.Value, GetComponent<Translation>(trash).Value);
                        if (distanceToTrash < lowestDistance)
                        {
                            lowestDistance = distanceToTrash;
                            closestTrash = trash;
                        }
                    }
                    if(closestTrash == Entity.Null) {return;}

                    ecb.AddComponent<TargetTrashData>(e);
                    ecb.SetComponent(e, new TargetTrashData{Value = closestTrash});
                    
                }).Run();
        }
    }

    public struct TargetTrashData : IComponentData
    {
        public Entity Value;
    }
}