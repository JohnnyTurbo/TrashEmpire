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
                    //EntityManager.AddComponent<StopMovingTag>(e);
                    ecb.AddComponent<PickingUpTrashData>(e);
                    ecb.AddComponent<BeingPickedUpTag>(targetTrash.Value);
                    //ecb.DestroyEntity(targetTrash.Value);
                    //ecb.RemoveComponent<TargetTrashData>(e);
                    return;
                }
                if (HasComponent<NavDestination>(e))
                {
                    //GetBuffer<NavPathBufferElement>(e).Clear();
                    //EntityManager.RemoveComponent<NavDestination>(e);
                    ecb.RemoveComponent<NavDestination>(e);
                }
               
                ecb.AddComponent<NavDestination>(e);
                ecb.SetComponent(e, new NavDestination
                {
                    WorldPoint = trashPosition,
                    Teleport = false
                });
                /*EntityManager.AddComponentData(e, new NavDestination
                {
                    WorldPoint = trashPosition,
                    Teleport = false
                });*/
            }).Run();
        }
    }

    public struct BeingPickedUpTag : IComponentData {}

    public struct PickingUpTrashData : IComponentData
    {
        public float timePickingUp;
    }
}