using System;
using Sokoban.Scripts.Levels.Tiles.Data;

namespace Sokoban.Scripts.Levels.Data {
    [Serializable]
    public class LevelTileDataModel {
        public int coordX;
        public int coordY;
        public TileTypes tileType;
        public TileMaterialTypes tileMaterialType;
        public int tileLayer;
        public int tileOrder;
    }
}
