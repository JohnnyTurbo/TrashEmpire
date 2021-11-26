using Unity.Entities;

namespace TMG.TrashEmpire
{
    public class DropOffTrashSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            Entities.ForEach((Entity e, ref DropOffTrashData dropOffTrashData, ref TrashCollectionData trashCollectionData) =>
            {
                dropOffTrashData.Timer += deltaTime;
                if (dropOffTrashData.Timer >= trashCollectionData.CurrentTrashHeld)
                {
                    trashCollectionData.CurrentTrashHeld = 0f;
                    EntityManager.RemoveComponent<MaxTrashHeldTag>(e);
                    EntityManager.RemoveComponent<DropOffTrashData>(e);
                }
            }).WithStructuralChanges().WithoutBurst().Run();
        }
    }
}