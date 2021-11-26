using Unity.Entities;

namespace TMG.TrashEmpire
{
    [GenerateAuthoringComponent]
    public struct SelectionUIData : IComponentData
    {
        public Entity SelectionUI;
        public Entity SelectionUIPrefab;
    }
}