using Unity.Entities;

namespace TMG.TrashEmpire
{
    [GenerateAuthoringComponent]
    internal struct PatrolAreaPrefabData : IComponentData
    {
        public Entity PatrolAreaSelectionPrefab;
        public Entity PatrolAreaPrefab;
    }
}