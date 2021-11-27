using Reese.Nav;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TMG.TrashEmpire
{
    public class NavToTrashSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            var ecb = _ecbSystem.CreateCommandBuffer();
            
            Entities.WithNone<PickingUpTrashData>().ForEach((Entity e, in TargetTrashData targetTrash, in Translation translation) =>
            {
                var trashPosition = GetComponent<Translation>(targetTrash.Value).Value;
                if (math.distance(translation.Value, trashPosition) <= 2f)
                {
                    ecb.AddComponent<PickingUpTrashData>(e);
                    ecb.AddComponent<BeingPickedUpTag>(targetTrash.Value);
                    return;
                }
                if (HasComponent<NavDestination>(e))
                {
                    ecb.RemoveComponent<NavDestination>(e);
                }
               
                ecb.AddComponent<NavDestination>(e);
                ecb.SetComponent(e, new NavDestination
                {
                    WorldPoint = trashPosition,
                    Teleport = false
                });
            }).Run();
        }
    }
}