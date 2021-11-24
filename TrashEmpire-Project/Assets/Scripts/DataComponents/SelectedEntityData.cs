using Unity.Entities;

namespace TMG.TrashEmpire
{
    [GenerateAuthoringComponent]
    public struct SelectedEntityData : IComponentData
    {
        public Entity SelectedUnit;
        public Entity SelectionUI;
        public Entity SelectionUIPrefab;
    }
}