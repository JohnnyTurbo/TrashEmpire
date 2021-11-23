using Unity.Entities;

namespace TMG.TrashEmpire
{
    [GenerateAuthoringComponent]
    public struct SteeringData : IComponentData
    {
        public float PreferredSpeed;
        public float BrakeSpeed;
    }
}