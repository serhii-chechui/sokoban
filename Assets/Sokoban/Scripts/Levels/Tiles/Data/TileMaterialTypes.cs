using System;

namespace Sokoban.Scripts.Levels.Tiles.Data {
    [Serializable]
    public enum TileMaterialTypes : byte {
        EMPTY = 0,
        FILLER = 1,
        GRASS = 2,
        BRICKS = 3,
    }
}