using Unity.Entities;

namespace TMG.TrashEmpire
{
    public class DeleteTrashSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<DeleteTrashTag>().ForEach((Entity e) =>
            {
                EntityManager.DestroyEntity(e);
            }).WithStructuralChanges().WithoutBurst().Run();
        }
    }
}