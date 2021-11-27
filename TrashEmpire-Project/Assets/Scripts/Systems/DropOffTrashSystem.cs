using Unity.Entities;

namespace TMG.TrashEmpire
{
    public class DropOffTrashSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            var ecb = _ecbSystem.CreateCommandBuffer();
            var deltaTime = Time.DeltaTime;
            Entities.ForEach((Entity e, ref DropOffTrashData dropOffTrashData, ref TrashCollectionData trashCollectionData) =>
            {
                dropOffTrashData.Timer += deltaTime;
                if (dropOffTrashData.Timer >= trashCollectionData.CurrentTrashHeld)
                {
                    trashCollectionData.CurrentTrashHeld = 0f;
                    ecb.RemoveComponent<MaxTrashHeldTag>(e);
                    ecb.RemoveComponent<DropOffTrashData>(e);
                    ecb.RemoveComponent<ForceToDropOffPointTag>(e);
                }
            }).Run();
        }
    }
}