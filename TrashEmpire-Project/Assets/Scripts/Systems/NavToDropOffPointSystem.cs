using Reese.Nav;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TMG.TrashEmpire
{
    public class NavToDropOffPointSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithNone<NavDestination>()
                .WithAll<MaxTrashHeldTag>().ForEach((Entity e, ref DropOffPointData dropOffPoint, in Translation position) =>
                {
                    if (math.distance(position.Value, GetComponent<Translation>(dropOffPoint.Value).Value) <= 2f)
                    {
                        EntityManager.AddComponent<DropOffTrashData>(e);
                    }
                    EntityManager.AddComponentData(e, new NavDestination
                    {
                        WorldPoint = GetComponent<Translation>(dropOffPoint.Value).Value,
                        Teleport = false
                    });
                }).WithStructuralChanges().WithoutBurst().Run();
        }
    }
}