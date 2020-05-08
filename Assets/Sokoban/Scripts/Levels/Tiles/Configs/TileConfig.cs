using Sokoban.Scripts.Levels.Tiles.Data;
using UnityEngine;

namespace Sokoban.Scripts.Levels.Tiles.Configs {
    [CreateAssetMenu(fileName = "TileConfig", menuName = "Sokoban/Levels/Tiles/Tile Config", order = 0)]
    public class TileConfig : ScriptableObject {
        public TileTypes tileType;
        public TileMaterialConfig tileMaterialConfig;
    }
}
