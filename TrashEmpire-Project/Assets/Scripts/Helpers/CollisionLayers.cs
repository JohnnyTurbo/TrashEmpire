using System;

namespace TMG.TrashEmpire
{
    [Flags]
    public enum CollisionLayers
    {
        SelectionBox = 1 << 0,
        Trash = 1 << 1,
        PatrolArea = 1 << 2,
        DropOff = 1 << 3,
        Raycast = 1 << 4,
        Ground = 1 << 5,
        Units = 1 << 6
    }
}