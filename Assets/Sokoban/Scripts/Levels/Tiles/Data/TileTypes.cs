using System;

namespace Sokoban.Scripts.Levels.Tiles.Data {
    [Serializable]
    public enum TileTypes : byte {
        EMPTY = 0,
        WALKABLE = 1,
        WALLS = 2,
        DAMAGEABLE = 3
    }
}

