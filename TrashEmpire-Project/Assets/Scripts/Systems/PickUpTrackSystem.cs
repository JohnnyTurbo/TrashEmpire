using Unity.Entities;
using Unity.Physics;

namespace TMG.TrashEmpire
{
    public class PickUpTrackSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<BeingPickedUpTag, PhysicsMass, PhysicsVelocity>().ForEach((Entity e) =>
            {
                EntityManager.RemoveComponent<PhysicsMass>(e);
                EntityManager.RemoveComponent<PhysicsVelocity>(e);

            }).WithStructuralChanges().WithoutBurst().Run();

            var deltaTime = Time.DeltaTime;
            
            Entities.ForEach((Entity e, ref PickingUpTrashData trashTimer, in TargetTrashData targetTrash)=>
            {
                trashTimer.timePickingUp += deltaTime;
                if (trashTimer.timePickingUp >= GetComponent<TrashData>(targetTrash.Value).Weight)
                {
                    EntityManager.AddComponent<DeleteTrashTag>(targetTrash.Value);
                    EntityManager.RemoveComponent<PickingUpTrashData>(e);
                    EntityManager.RemoveComponent<TargetTrashData>(e);
                }
            }).WithStructuralChanges().WithoutBurst().Run();
        }
    }

    public struct DeleteTrashTag : IComponentData {}
}