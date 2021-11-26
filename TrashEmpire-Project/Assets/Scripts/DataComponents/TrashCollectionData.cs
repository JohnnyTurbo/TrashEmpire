using Unity.Entities;

namespace TMG.TrashEmpire
{
    [GenerateAuthoringComponent]
    public struct TrashCollectionData : IComponentData
    {
        public float CurrentTrashHeld;
        public float TrashCapacity;
    }
}