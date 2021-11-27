using Unity.Entities;
using Unity.Physics;

namespace TMG.TrashEmpire
{
    public class PickUpTrackSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _ecbSystem.CreateCommandBuffer();
            Entities.WithAll<BeingPickedUpTag, PhysicsMass, PhysicsVelocity>().ForEach((Entity e) =>
            {
                ecb.RemoveComponent<PhysicsMass>(e);
                ecb.RemoveComponent<PhysicsVelocity>(e);

            }).Run();

            var deltaTime = Time.DeltaTime;
            
            Entities.ForEach((Entity e, ref PickingUpTrashData trashTimer, ref TrashCollectionData trashCollectionData, in TargetTrashData targetTrash)=>
            {
                trashTimer.Timer += deltaTime;
                if (trashTimer.Timer >= GetComponent<TrashData>(targetTrash.Value).Weight)
                {
                    trashCollectionData.CurrentTrashHeld += GetComponent<TrashData>(targetTrash.Value).Weight;
                    ecb.AddComponent<DeleteTrashTag>(targetTrash.Value);
                    ecb.RemoveComponent<PickingUpTrashData>(e);
                    ecb.RemoveComponent<TargetTrashData>(e);
                }
            }).Run();
        }
    }
}