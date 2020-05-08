using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sokoban.Scripts.Levels.Data {
    [Serializable]
    public class LevelDataModel {
        
        public string levelName;
        
        public int levelWidth;
        public int levelHeight;
        
        public List<LevelTileDataModel> levelTiles;

        public LevelDataModel(int levelWidth, int levelHeight) {
            this.levelWidth = levelWidth;
            this.levelHeight = levelHeight;

            levelTiles = new List<LevelTileDataModel>();

            for (var x = 0; x < levelHeight; x++) {
                for (var y = 0; y < levelWidth; y++) {
                    var newTile = new LevelTileDataModel {
                            coordX = x,
                            coordY = y,
                            tileType = 0,
                            tileMaterialType = 0,
                            tileLayer = 0,
                            tileOrder = 0
                    };

                    levelTiles.Add(newTile);
                }
            }
        }

        public LevelTileDataModel GetTileByCoords(int x, int y) {
            return levelTiles.Find(t => t.coordX == x && t.coordY == y);
        }

        public Vector2Int GetLevelSize() {
            return new Vector2Int(levelWidth, levelHeight);
        }
    }
}