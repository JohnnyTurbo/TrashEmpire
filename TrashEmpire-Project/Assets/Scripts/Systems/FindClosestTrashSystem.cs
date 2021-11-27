using Reese.Nav;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TMG.TrashEmpire
{
    public class FindClosestTrashSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _ecbSystem.CreateCommandBuffer();
            
            Entities
                .WithNone<MaxTrashHeldTag, TargetTrashData, PickingUpTrashData>()
                .WithNone<ForceToDropOffPointTag>()
                .ForEach((Entity e, in PatrolAreaData patrolAreaData, in Translation translation, in TrashCollectionData trashCollectionData) =>
                {
                    var trashInRange = GetBuffer<TrashBufferElement>(patrolAreaData.Value);
                    if (trashInRange.Length <= 0)
                    {
                        if (trashCollectionData.CurrentTrashHeld > 0)
                        {
                            ecb.AddComponent<ForceToDropOffPointTag>(e);
                        }
                        else
                        {
                            ecb.AddComponent<NavDestination>(e);
                            ecb.SetComponent(e, new NavDestination
                            {
                                WorldPoint = GetComponent<Translation>(patrolAreaData.Value).Value,
                                Teleport = false
                            });
                        }
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
}