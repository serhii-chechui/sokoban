using Sokoban.Scripts.Levels.Tiles.Data;
using UnityEngine;

namespace Sokoban.Scripts.Levels.Tiles.Configs {
    [CreateAssetMenu(fileName = "MaterialTileConfig", menuName = "Sokoban/Levels/Tiles/Material Tile Config", order = 0)]
    public class TileMaterialConfig : ScriptableObject {
        public TileMaterialTypes tileMaterialType;
        public Sprite materialSprite;
    }
}
