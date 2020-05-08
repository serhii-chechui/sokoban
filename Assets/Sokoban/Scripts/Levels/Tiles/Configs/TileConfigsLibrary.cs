using System.Collections.Generic;
using Sokoban.Scripts.Levels.Tiles.Data;
using UnityEngine;

namespace Sokoban.Scripts.Levels.Tiles.Configs {
    [CreateAssetMenu(fileName = "TileConfigsLibrary", menuName = "Sokoban/Levels/Tiles/Tile Configs Library", order = 0)]
    public class TileConfigsLibrary : ScriptableObject {
        
        public List<TileConfig> tileConfigsList = new List<TileConfig>();

        public TileConfig GetTileConfigByTileType(TileTypes tileType) {
            return tileConfigsList.Find(t => t.tileType == tileType);
        }
        
        public TileConfig GetTileConfigByMaterialType(TileMaterialTypes tileMaterialType) {
            return tileConfigsList.Find(t => t.tileMaterialConfig.tileMaterialType == tileMaterialType);
        }
    }
}
