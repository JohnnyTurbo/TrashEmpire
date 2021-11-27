using Reese.Nav;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TMG.TrashEmpire
{
    public class NavToDropOffPointSystem : SystemBase
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
                .WithNone<NavDestination>()
                .WithAny<MaxTrashHeldTag, ForceToDropOffPointTag>()
                .ForEach((Entity e, ref DropOffPointData dropOffPoint, in Translation position) =>
                {
                    if (math.distance(position.Value, GetComponent<Translation>(dropOffPoint.Value).Value) <= 2f)
                    {
                        ecb.AddComponent<DropOffTrashData>(e);
                    }
                    ecb.AddComponent<NavDestination>(e);
                    ecb.SetComponent(e, new NavDestination
                    {
                        WorldPoint = GetComponent<Translation>(dropOffPoint.Value).Value,
                        Teleport = false
                    });
                }).Run();
        }
    }
}