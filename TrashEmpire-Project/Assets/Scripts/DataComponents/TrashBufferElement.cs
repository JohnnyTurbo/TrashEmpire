using Unity.Entities;

namespace TMG.TrashEmpire
{
    [GenerateAuthoringComponent]
    [InternalBufferCapacity(8)]
    public struct TrashBufferElement : IBufferElementData
    {
        public Entity Value;

        public static implicit operator TrashBufferElement(Entity e)
        {
            return new TrashBufferElement {Value = e};
        }

        public static implicit operator Entity(TrashBufferElement trashBufferElement)
        {
            return trashBufferElement.Value;
        }
    }
}