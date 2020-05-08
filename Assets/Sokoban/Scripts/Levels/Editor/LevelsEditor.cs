using System;
using Sokoban.Scripts.Levels.Data;
using Sokoban.Scripts.Levels.Tiles.Configs;
using UnityEditor;
using UnityEngine;
using System.IO;
using Sokoban.Scripts.Levels.Tiles.Data;

namespace Sokoban.Scripts.Levels.Editor {
    public class LevelsEditor : EditorWindow {
        /// <summary>
        /// Reference to the tiles library config.
        /// </summary>
        private TileConfigsLibrary _tileConfigsLibrary;
        
        /// <summary>
        /// Current selected tile config.
        /// </summary>
        private TileConfig _currentTileConfig;

        /// <summary>
        /// Last set level size.
        /// </summary>
        private Vector2Int _levelSize;
        
        /// <summary>
        /// Current level name.
        /// </summary>
        private string _levelName;
        
        /// <summary>
        /// Current selected tile Layer.
        /// </summary>
        private int _currentTileLayer;
        
        /// <summary>
        /// Current selected tile order in layer. 
        /// </summary>
        private int _currentTileLayerOrder;

        /// <summary>
        /// Reference to the Current level Data.
        /// </summary>
        private LevelDataModel _currentLevelDataModel;

        /// <summary>
        /// Reference to the level data saved into JSON. 
        /// </summary>
        private TextAsset _savedLevelJSON;

        /// <summary>
        /// Used to keep Scroll Position for EditorWindow.
        /// </summary>
        private Vector2 _scrollPosition;

        private const int TILE_BUTTON_SIZE = 20;
        private const int PRIMARY_BUTTON_HEIGHT = 32;
        
        GUIStyle _innerContainerGuiStyle;
        GUIStyle _tileEditButtonGuiStyle;
        GUIContent _tileEditButtonGuiContent;

        [MenuItem("Sokoban/Levels/Levels Editor")]
        private static void ShowWindow() {
            var window = GetWindow<LevelsEditor>();
            window.titleContent = new GUIContent("Sokoban - Levels Editor");
            window.Show();
        }

        private void OnEnable() {
            //Create custom editor gui styles
            CreateGuiStyles();
            _levelSize = new Vector2Int(4,4);
        }

        private void OnFocus() {
            CreateGuiStyles();
        }

        private void OnGUI() {
            //Calculate the scroll position
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            #region Tiles Configs Library Selection

            DisplayLibrarySelectionGui();

            #endregion
            
            if (_tileConfigsLibrary != null) {
                
                //Create Buttons for each tile config
                CreateTileButtonsGui(_tileConfigsLibrary);

                #region Current Tile Config

                DisplayCurrentTileGui();

                #endregion

                #region Level Creation

                DisplayCreateLevelGui();

                #endregion

                #region Level Edit

                DisplayEditLevelGui();

                #endregion

                #region Level Construction

                DisplayLevelConstructionGui();

                #endregion

                #region Data Save

                DisplaySaveGui();

                #endregion
            }

            EditorGUILayout.EndScrollView();
        }

        private void DisplayLibrarySelectionGui() {
            EditorGUILayout.BeginVertical(_innerContainerGuiStyle);
            
            EditorGUILayout.LabelField("Tiles Configs Library", EditorStyles.boldLabel);
            
            EditorGUILayout.Space();
            
            //Set tiles library
            _tileConfigsLibrary = (TileConfigsLibrary) EditorGUILayout.ObjectField("Tile Configs Library", _tileConfigsLibrary, typeof(TileConfigsLibrary), false);
            
            if (_tileConfigsLibrary == null) {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("You should provide TileConfigsLibrary scriptable object:", MessageType.Warning);
            }

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Create GUI elements for operations with level creation or loading.
        /// </summary>
        private void DisplayCreateLevelGui() {
            
            EditorGUILayout.BeginVertical(_innerContainerGuiStyle);
                
            EditorGUILayout.LabelField("Level Creation", EditorStyles.boldLabel);

            _levelName = EditorGUILayout.TextField("Level Name", _levelName);

            EditorGUILayout.LabelField("How much tiles will have level with and height.", EditorStyles.miniLabel);
            _levelSize = EditorGUILayout.Vector2IntField("Level Size: ", _levelSize);

            EditorGUILayout.Space();
                
            if (GUILayout.Button("Create New Level", GUILayout.ExpandWidth(true), GUILayout.Height(PRIMARY_BUTTON_HEIGHT))) {
                _currentLevelDataModel = new LevelDataModel(_levelSize.x, _levelSize.y) {levelName = _levelName};
            }
                
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Level Loading", EditorStyles.boldLabel);

            _savedLevelJSON = (TextAsset)EditorGUILayout.ObjectField("JSON Level:", _savedLevelJSON, typeof(TextAsset), false);
                    
            if (GUILayout.Button("Load Level", GUILayout.ExpandWidth(true), GUILayout.Height(PRIMARY_BUTTON_HEIGHT))) {
                var deserializedData = JsonUtility.FromJson<LevelDataModel>(_savedLevelJSON.text);
                _currentLevelDataModel = deserializedData;
                _levelSize = _currentLevelDataModel.GetLevelSize();
                _levelName = _currentLevelDataModel.levelName;
            }
                
            EditorGUILayout.Space();

            if (_currentLevelDataModel == null) {
                EditorGUILayout.HelpBox("Click at the button \"Create New Level\" to create new Level Data.\nOr \"Load Level\" to load levels JSON config.", MessageType.Warning);
            }

            EditorGUILayout.EndVertical();
        }
        
        /// <summary>
        /// Create GUI elements for operations with level tiles.
        /// </summary>
        private void DisplayEditLevelGui() {
            
            EditorGUILayout.BeginVertical(_innerContainerGuiStyle);

            EditorGUILayout.LabelField("Level Edit", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Select tile config and click over the small buttons to repaint level tile.", EditorStyles.miniLabel);

            EditorGUILayout.Space();

            if (_currentLevelDataModel != null) {

                if (_currentTileConfig == null) {
                    EditorGUILayout.HelpBox("Select Tile Config to have ability edit current level tiles.", MessageType.Warning);
                } else {
                    var levelSize = _currentLevelDataModel.GetLevelSize();

                    for (var y = 0; y < levelSize.y; y++) {
                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.BeginHorizontal();

                        for (var x = 0; x < levelSize.x; x++) {
                            var materialType = _currentLevelDataModel.GetTileByCoords(x, y).tileMaterialType;

                            if (materialType != TileMaterialTypes.EMPTY) {
                                var texture = _tileConfigsLibrary.GetTileConfigByMaterialType(materialType).tileMaterialConfig.materialSprite.texture;
                                _tileEditButtonGuiContent = new GUIContent(texture);
                            } else {
                                _tileEditButtonGuiContent = new GUIContent($"{x}-{y}");
                            }

                            if (GUILayout.Button(_tileEditButtonGuiContent, _tileEditButtonGuiStyle, GUILayout.Width(TILE_BUTTON_SIZE), GUILayout.Height(TILE_BUTTON_SIZE))) {
                                var tileData = _currentLevelDataModel.GetTileByCoords(x, y);
                                tileData.tileType = _currentTileConfig.tileType;
                                tileData.tileMaterialType = _currentTileConfig.tileMaterialConfig.tileMaterialType;
                            }
                        }

                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();
                    }
                }
            }

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Create button for each tile based at tiles configs library.
        /// </summary>
        /// <param name="tileConfigsLibrary">Tiles configs library.</param>
        private void CreateTileButtonsGui(TileConfigsLibrary tileConfigsLibrary) {

            EditorGUILayout.BeginVertical(_innerContainerGuiStyle);
            
            //Display the title
            EditorGUILayout.LabelField("Tile Configs:", EditorStyles.boldLabel);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            foreach (var tileConfig in tileConfigsLibrary.tileConfigsList) {
                if (GUILayout.Button($"{tileConfig.tileMaterialConfig.tileMaterialType}", GUILayout.Width(64), GUILayout.Height(40))) {
                    Debug.Log($"Current tile material: {tileConfig.tileMaterialConfig.tileMaterialType}");
                    _currentTileConfig = tileConfig;
                }
            }

            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Render properties of the current tile config.
        /// </summary>
        private void DisplayCurrentTileGui() {

            if (_currentTileConfig != null) {
                
                EditorGUILayout.BeginVertical(_innerContainerGuiStyle);
            
                EditorGUILayout.LabelField("Current Tile", EditorStyles.boldLabel);
            
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
            
                if (_currentTileConfig.tileMaterialConfig.materialSprite) {
                    GUILayout.Box(_currentTileConfig.tileMaterialConfig.materialSprite.texture);
                }

                EditorGUILayout.BeginVertical();
            
                EditorGUILayout.LabelField($"Tile Type: {_currentTileConfig.tileType}");
            
                EditorGUILayout.LabelField($"Tile Material: {_currentTileConfig.tileMaterialConfig.tileMaterialType}");
            
                EditorGUILayout.Space();

                if (_currentLevelDataModel != null) {
                    if (GUILayout.Button($"Fill All Tiles with {_currentTileConfig.tileMaterialConfig.tileMaterialType}", GUILayout.Width(160), GUILayout.Height(PRIMARY_BUTTON_HEIGHT))) {
                    
                        var levelSize = _currentLevelDataModel.GetLevelSize();
                        
                        for (var y = 0; y < levelSize.y; y++) {
                            for (var x = 0; x < levelSize.x; x++) {
                                var tileData = _currentLevelDataModel.GetTileByCoords(x, y);
                                tileData.tileType = _currentTileConfig.tileType;
                                tileData.tileMaterialType = _currentTileConfig.tileMaterialConfig.tileMaterialType;
                            }
                        }
                    }
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
        }
        
        /// <summary>
        /// Display GUI to construct levels visual based at level data.  
        /// </summary>
        private void DisplayLevelConstructionGui() {
            if (GUILayout.Button("Construct level", GUILayout.ExpandWidth(true), GUILayout.Height(PRIMARY_BUTTON_HEIGHT))) {
                var levelRoot = new GameObject("Level-Root");

                var levelSize = _currentLevelDataModel.GetLevelSize();

                for (var y = 0; y < levelSize.y; y++) {
                    for (var x = 0; x < levelSize.x; x++) {
                        var newTileGameObject = new GameObject($"Tile-{x}-{y}", typeof(SpriteRenderer));
                        newTileGameObject.transform.position = new Vector2(x, -y);
                        var newTile    = newTileGameObject.GetComponent<SpriteRenderer>();
                        var tileData   = _currentLevelDataModel.GetTileByCoords(x, y);
                        var tileConfig = _tileConfigsLibrary.GetTileConfigByMaterialType(tileData.tileMaterialType);
                        newTile.sprite = tileConfig.tileMaterialConfig.materialSprite;

                        if (tileData.tileType == TileTypes.WALLS) {
                            newTileGameObject.AddComponent<BoxCollider2D>();
                        }

                        newTileGameObject.transform.SetParent(levelRoot.transform);
                    }
                }
            }
        }
        
        /// <summary>
        /// Display GUI to save current level data into JSON file.
        /// </summary>
        private void DisplaySaveGui() {

            if (GUILayout.Button("Save to JSON", GUILayout.ExpandWidth(true), GUILayout.Height(PRIMARY_BUTTON_HEIGHT))) {
                var projectName = Application.productName;
                if (!AssetDatabase.IsValidFolder($"Assets/{projectName}/Data/Levels")) {
                    AssetDatabase.CreateFolder($"Assets/{projectName}/Data", "Levels");
                    AssetDatabase.Refresh();
                }
                
                _currentLevelDataModel.levelName = _levelName;
                
                var serializedData = JsonUtility.ToJson(_currentLevelDataModel, true);
                var versionStamp = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");
                var levelName = string.IsNullOrEmpty(_levelName) ? "level" : _levelName;
                File.WriteAllText(Application.dataPath + $"/{projectName}/Data/Levels/{levelName}-{versionStamp}.json", serializedData);
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// Creates custom GUI styles.
        /// </summary>
        private void CreateGuiStyles() {
            
            _innerContainerGuiStyle = new GUIStyle(EditorStyles.helpBox);
            _innerContainerGuiStyle.padding = new RectOffset(16, 16, 16, 16);
            
            //Create GUI style for the GUI buttons 
            _tileEditButtonGuiStyle = new GUIStyle(EditorStyles.miniButton);
            _tileEditButtonGuiStyle.margin = new RectOffset(1,1,1,1);
            _tileEditButtonGuiStyle.border = new RectOffset(1,1,1,1);
            _tileEditButtonGuiStyle.padding = new RectOffset(0,0,0,0);
            _tileEditButtonGuiStyle.fontSize = 9;

            _tileEditButtonGuiContent = new GUIContent();
        }
    }
}
