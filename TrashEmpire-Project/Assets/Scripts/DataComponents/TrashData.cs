using Unity.Entities;

namespace TMG.TrashEmpire 
{
    [GenerateAuthoringComponent]
    public struct TrashData : IComponentData
    {
        public float Weight;
        public bool IsTargeted;
    }
}