using Unity.Entities;

namespace TMG.TrashEmpire
{
    public class DeleteTrashSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            var ecb = _ecbSystem.CreateCommandBuffer();
            Entities.WithAll<DeleteTrashTag>().ForEach((Entity e) =>
            {
                ecb.DestroyEntity(e);
            }).Run();
        }
    }
}