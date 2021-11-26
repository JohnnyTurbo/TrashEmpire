﻿using Unity.Entities;
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
            
            Entities.ForEach((Entity e, ref PickingUpTrashData trashTimer, ref TrashCollectionData trashCollectionData, in TargetTrashData targetTrash)=>
            {
                trashTimer.Timer += deltaTime;
                if (trashTimer.Timer >= GetComponent<TrashData>(targetTrash.Value).Weight)
                {
                    trashCollectionData.CurrentTrashHeld += GetComponent<TrashData>(targetTrash.Value).Weight;
                    EntityManager.AddComponent<DeleteTrashTag>(targetTrash.Value);
                    EntityManager.RemoveComponent<PickingUpTrashData>(e);
                    EntityManager.RemoveComponent<TargetTrashData>(e);
                }
            }).WithStructuralChanges().WithoutBurst().Run();
        }
    }

    public struct DeleteTrashTag : IComponentData {}
}