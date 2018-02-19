using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

namespace Gaia
{
    /// <summary>
    /// Handy helper for all things Gaia
    /// </summary>
    public class GaiaManagerEditor : EditorWindow
    {
        private GUIStyle m_boxStyle;
        private GUIStyle m_wrapStyle;
        private Vector2 m_scrollPosition = Vector2.zero;
        private GaiaDefaults m_defaults;
        private GaiaResource m_resources;
        private Gaia.GaiaConstants.ManagerEditorMode m_managerMode = Gaia.GaiaConstants.ManagerEditorMode.Standard;
        private bool m_foldoutTerrain = false;
        private bool m_foldoutSpawners = false;
        private bool m_foldoutCharacters = false;

        #region Gaia Menu Commands
        /// <summary>
        /// Show Gaia Manager editor window
        /// </summary>
        [MenuItem("Window/Gaia/Show Gaia Manager... %g", false, 1)]
        public static void ShowGaiaManager()
        {
            var manager = EditorWindow.GetWindow<Gaia.GaiaManagerEditor>(false, "Gaia Manager");
            manager.Show();
        }

        /// <summary>
        /// Show Gaia resource editor window
        /// </summary>
        [MenuItem("Window/Gaia/About...", false, 1)]
        private static void ShowAboutWindow()
        {
            var about = EditorWindow.GetWindow<Gaia.AboutEditor>(false, "About Gaia");
            about.minSize = new Vector2(300, 125);
            about.maxSize = new Vector2(300, 125);
            about.Show();
        }

        #endregion

        /// <summary>
        /// Create and returns a defaults asset
        /// </summary>
        /// <returns>New defaults asset</returns>
        public static GaiaDefaults CreateDefaultsAsset()
        {
            GaiaDefaults defaults = ScriptableObject.CreateInstance<Gaia.GaiaDefaults>();
            AssetDatabase.CreateAsset(defaults, string.Format("Assets/Gaia/Data/GaiaDefaults-{0:yyyyMMdd-HHmmss}.asset", DateTime.Now));
            AssetDatabase.SaveAssets();
            return defaults;
        }

        /// <summary>
        /// Create and returns a resources asset
        /// </summary>
        /// <returns>New resources asset</returns>
        public static GaiaResource CreateResourcesAsset()
        {
            GaiaResource resources = ScriptableObject.CreateInstance<Gaia.GaiaResource>();
            string[] path = EditorApplication.currentScene.Split(char.Parse("/"));
            if (path.Length > 0)
            {
                string sceneName = path[path.Length - 1];
                sceneName = sceneName.Replace(".unity", "");
                if (!string.IsNullOrEmpty(sceneName))
                {
                    AssetDatabase.CreateAsset(resources, string.Format("Assets/Gaia/Data/GaiaResources-{0}-{1:yyyyMMdd-HHmmss}.asset", sceneName, DateTime.Now));
                }
                else
                {
                    AssetDatabase.CreateAsset(resources, string.Format("Assets/Gaia/Data/GaiaResources-{0:yyyyMMdd-HHmmss}.asset", DateTime.Now));
                }
            }
            else
            {
                AssetDatabase.CreateAsset(resources, string.Format("Assets/Gaia/Data/GaiaResources-{0:yyyyMMdd-HHmmss}.asset", DateTime.Now));
            }
            AssetDatabase.SaveAssets();
            return resources;
        }

        /// <summary>
        /// See if we can preload the manager with existing settings
        /// </summary>
        void OnEnable()
        {
            //Set the Gaia directories up
            Utils.CreateGaiaAssetDirectories();

            //Grab first default we can find
            if (m_defaults == null)
            {
                m_defaults = (GaiaDefaults)GetScriptableObject("GaiaDefaults");
            }

            //Grab first resource we can find
            if (m_resources == null)
            {
                m_resources = (GaiaResource)GetScriptableObject("GaiaResources");
            }
        }

        void OnDisable()
        {
        }

        void OnGUI()
        {
            //Set up the box style
            if (m_boxStyle == null)
            {
                m_boxStyle = new GUIStyle(GUI.skin.box);
                m_boxStyle.normal.textColor = GUI.skin.label.normal.textColor;
                m_boxStyle.fontStyle = FontStyle.Bold;
                m_boxStyle.alignment = TextAnchor.UpperLeft;
            }

            //Setup the wrap style
            if (m_wrapStyle == null)
            {
                m_wrapStyle = new GUIStyle(GUI.skin.label);
                m_wrapStyle.fontStyle = FontStyle.Normal;
                m_wrapStyle.wordWrap = true;
            }

            //Text intro
            GUILayout.BeginVertical(string.Format("Gaia Manager ({0}.{1})", Gaia.GaiaConstants.GaiaMajorVersion, Gaia.GaiaConstants.GaiaMinorVersion), m_boxStyle);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("The Gaia manager guides you through common scene creation workflows.", m_wrapStyle);
            GUILayout.EndVertical();

            //Scroll
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, false, false);

            //Select or create new defaults and resources
            if (m_defaults == null || m_resources == null)
            {

                EditorGUILayout.BeginHorizontal();
                m_defaults = (GaiaDefaults)EditorGUILayout.ObjectField(GetLabel("Defaults"), m_defaults, typeof(GaiaDefaults), false);
                if (GUILayout.Button(GetLabel("New"), GUILayout.Width(45)))
                {
                    m_defaults = CreateDefaultsAsset();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                m_resources = (GaiaResource)EditorGUILayout.ObjectField(GetLabel("Resources"), m_resources, typeof(GaiaResource), false);
                if (GUILayout.Button(GetLabel("New"), GUILayout.Width(45)))
                {
                    m_resources = CreateResourcesAsset();
                    if (m_defaults != null)
                    {
                        m_resources.m_seaLevel = m_defaults.m_seaLevel;
                        m_resources.m_beachHeight = m_defaults.m_beachHeight;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                m_defaults = (GaiaDefaults)EditorGUILayout.ObjectField(GetLabel("Defaults"), m_defaults, typeof(GaiaDefaults), false);
                if (GUILayout.Button(GetLabel("New"), GUILayout.Width(45)))
                {
                    m_defaults = CreateDefaultsAsset();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                m_resources = (GaiaResource)EditorGUILayout.ObjectField(GetLabel("Resources"), m_resources, typeof(GaiaResource), false);
                if (GUILayout.Button(GetLabel("New"), GUILayout.Width(45)))
                {
                    m_resources = CreateResourcesAsset();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                if (m_managerMode == Gaia.GaiaConstants.ManagerEditorMode.Standard)
                {
                    GUI.enabled = false;
                }
                if (GUILayout.Button(GetLabel("STANDARD")))
                {
                    m_managerMode = Gaia.GaiaConstants.ManagerEditorMode.Standard;
                }
                GUI.enabled = true;
                if (m_managerMode == Gaia.GaiaConstants.ManagerEditorMode.Advanced)
                {
                    GUI.enabled = false;
                }
                if (GUILayout.Button(GetLabel("ADVANCED")))
                {
                    m_managerMode = Gaia.GaiaConstants.ManagerEditorMode.Advanced;
                }
                GUI.enabled = true;
                if (m_managerMode == Gaia.GaiaConstants.ManagerEditorMode.Utilities)
                {
                    GUI.enabled = false;
                }
                if (GUILayout.Button(GetLabel("UTILITIES")))
                {
                    m_managerMode = Gaia.GaiaConstants.ManagerEditorMode.Utilities;
                }
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();

                //m_managerMode = (Gaia.Constants.ManagerEditorMode)EditorGUILayout.EnumPopup(GetLabel("Operation Mode"), m_managerMode);
                if (m_managerMode == Gaia.GaiaConstants.ManagerEditorMode.Standard)
                {
                    DrawStandardEditor();
                }
                else if (m_managerMode == Gaia.GaiaConstants.ManagerEditorMode.Advanced)
                {
                    DrawAdvancedEditor();
                }
                else
                {
                    DrawUtilsEditor();
                }
            }

            //End scroll
            GUILayout.EndScrollView();
        }

        /// <summary>
        /// Draw the brief editor
        /// </summary>
        void DrawStandardEditor()
        {
            EditorGUI.indentLevel++;

            if (DisplayButton(GetLabel("1. Create Terrain & Show Stamper")))
            {
                CreateTerrain();
                ShowStamper();
            }
            if (DisplayButton(GetLabel("2. Create Spawners")))
            {
                //Create the spawners
                GameObject textureSpawner = CreateTextureSpawner();
                GameObject detailSpawner = CreateDetailSpawner();
                GameObject treeSpawner = CreateTreeSpawner();
                GameObject goSpawner = CreateGameObjectSpawner();
                GameObject spawnerGroup = CreateSpawnerGroup();

                //Create the spawner group
                SpawnerGroup sg = spawnerGroup.GetComponent<SpawnerGroup>();

                //And add the spawner instances
                SpawnerGroup.SpawnerInstance si;

                si = new SpawnerGroup.SpawnerInstance();
                si.m_name = textureSpawner.name;
                si.m_interationsPerSpawn = 1;
                si.m_spawner = textureSpawner.GetComponent<Spawner>();
                sg.m_spawners.Add(si);

                si = new SpawnerGroup.SpawnerInstance();
                si.m_name = goSpawner.name;
                si.m_interationsPerSpawn = 1;
                si.m_spawner = goSpawner.GetComponent<Spawner>();
                sg.m_spawners.Add(si);

                si = new SpawnerGroup.SpawnerInstance();
                si.m_name = treeSpawner.name;
                si.m_interationsPerSpawn = 1;
                si.m_spawner = treeSpawner.GetComponent<Spawner>();
                sg.m_spawners.Add(si);

                si = new SpawnerGroup.SpawnerInstance();
                si.m_name = detailSpawner.name;
                si.m_interationsPerSpawn = 1;
                si.m_spawner = detailSpawner.GetComponent<Spawner>();
                sg.m_spawners.Add(si);

                //Select the spawner group
                Selection.activeGameObject = spawnerGroup;
            }
            if (DisplayButton(GetLabel("3. Create Player, Wind and Water")))
            {
                CreatePlayer();
                CreateWindZone();
                CreateWater();
            }
            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Draw the detailed editor
        /// </summary>
        void DrawAdvancedEditor()
        {
            EditorGUI.indentLevel++;
            if (m_foldoutTerrain = EditorGUILayout.Foldout(m_foldoutTerrain, GetLabel("1. Create your Terrain...")))
            {
                EditorGUI.indentLevel++;
                if (DisplayButton(GetLabel("Create Terrain")))
                {
                    CreateTerrain();
                }
                if (DisplayButton(GetLabel("Show Stamper")))
                {
                    ShowStamper();
                }
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }
            if (m_foldoutSpawners = EditorGUILayout.Foldout(m_foldoutSpawners, GetLabel("2. Create and configure your Spawners...")))
            {
                EditorGUI.indentLevel++;
                //if (DisplayButton(GetLabel("Create Stamp Spawner")))
                //{
                //    Selection.activeObject = CreateStampSpawner();
                //}
                if (DisplayButton(GetLabel("Create Texture Spawner")))
                {
                    Selection.activeObject = CreateTextureSpawner();
                }
                if (DisplayButton(GetLabel("Create Detail Spawner")))
                {
                    Selection.activeObject = CreateDetailSpawner();
                }
                if (DisplayButton(GetLabel("Create Tree Spawner")))
                {
                    Selection.activeObject = CreateTreeSpawner();
                }
                if (DisplayButton(GetLabel("Create GameObject Spawner")))
                {
                    Selection.activeObject = CreateGameObjectSpawner();
                }
                if (DisplayButton(GetLabel("Create Spawner Group")))
                {
                    Selection.activeObject = CreateSpawnerGroup();
                }
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }
            if (m_foldoutCharacters = EditorGUILayout.Foldout(m_foldoutCharacters, GetLabel("3. Add common Game Objects...")))
            {
                EditorGUI.indentLevel++;
                if (DisplayButton(GetLabel("Add First Person Character")))
                {
                    CreatePlayer();
                }
                if (DisplayButton(GetLabel("Add Wind Zone")))
                {
                    CreateWindZone();
                }
                if (DisplayButton(GetLabel("Add Water4 Advanced Water")))
                {
                    CreateWater();
                }
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }
            /*
            if (m_foldoutPlugins = EditorGUILayout.Foldout(m_foldoutPlugins, GetLabel("4. Add and configure your Plugins...")))
            {
                EditorGUI.indentLevel++;
                if (m_foldoutTanuki = EditorGUILayout.Foldout(m_foldoutTanuki, GetLabel("Tanuki Digital")))
                {
                    EditorGUI.indentLevel++;
                    if (DisplayButton(GetLabel("Configure Suimono")))
                    {
                        Debug.LogWarning("Not implemented yet");
                    }
                    if (DisplayButton(GetLabel("Configure Tenkoku")))
                    {
                        Debug.LogWarning("Not implemented yet");
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }
            if (m_foldoutOptimisation = EditorGUILayout.Foldout(m_foldoutOptimisation, GetLabel("5. Optimise your Scene...")))
            {
                EditorGUI.indentLevel++;
                if (GUILayout.Button("Optimise Scene"))
                {
                    Debug.LogWarning("Not implemented yet");
                }
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }
             */
            EditorGUILayout.LabelField("Celebrate!", m_wrapStyle);
            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Draw the utils editor
        /// </summary>
        void DrawUtilsEditor()
        {
            EditorGUI.indentLevel++;

            if (DisplayButton(GetLabel("Show Scanner")))
            {
                Selection.activeGameObject = CreateScanner();
            }
            if (DisplayButton(GetLabel("Show Visualiser")))
            {
                Selection.activeGameObject = CreateVisualiser();
            }
            if (DisplayButton(GetLabel("Show Texture Mask Exporter")))
            {
                var export = EditorWindow.GetWindow<GaiaMaskExporterEditor>(false, "Mask Exporter");
                export.Show();
            }
            
            if (DisplayButton(GetLabel("Show Terrain OBJ Exporter")))
            {
                var export = EditorWindow.GetWindow<ExportTerrain>(false, "Export Terrain");
                export.Show();
            }

            if (DisplayButton(GetLabel("Export Terrain Heightmap as PNG")))
            {
                GaiaWorldManager mgr = new GaiaWorldManager(Terrain.activeTerrains);
                if (mgr.TileCount > 0)
                {
                    string path = string.Format("Terrain-{0:yyyyMMdd-HHmmss}", DateTime.Now);
                    path = Path.Combine(Application.dataPath, path);
                    path = path.Replace('\\', '/');
                    mgr.ExportWorldAsPng(path);
                    AssetDatabase.Refresh();
                    EditorUtility.FocusProjectWindow();
                    EditorUtility.DisplayDialog("Export complete", " Your heightmap has been saved to : " + path, "OK");
                }
            }

            if (DisplayButton(GetLabel("Export Shore Mask as PNG")))
            {
                var export = EditorWindow.GetWindow<ShorelineMaskerEditor>(false, "Export Shore");
                export.m_seaLevel = m_resources.m_seaLevel;
                export.Show();

                //string path = string.Format("Terrain-Mask-{0:yyyyMMdd-HHmmss}", DateTime.Now);
                //path = Path.Combine(Application.dataPath, path);
                //path = path.Replace('\\', '/');
                //mgr.ExportShorelineMask(path, m_resources.m_seaLevel, 20f);
            }
            

            /*
            if (m_foldoutNibbler = EditorGUILayout.Foldout(m_foldoutNibbler, GetLabel("3. Nibblers...")))
            {
                EditorGUI.indentLevel++;
                if (DisplayButton(GetLabel("Show Beach Nibbler")))
                {
                    Debug.LogWarning("Coming next beta");
                }
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }*/

            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Create the terrain
        /// </summary>

        void CreateTerrain()
        {
            m_defaults.CreateTerrain(m_resources);
        }


        /// <summary>
        /// Select or create a stamper
        /// </summary>
        void ShowStamper()
        {
            GameObject gaiaObj = GameObject.Find("Gaia");
            if (gaiaObj == null)
            {
                gaiaObj = new GameObject("Gaia");
            }
            GameObject stamperObj = GameObject.Find("Stamper");
            if (stamperObj == null)
            {
                stamperObj = new GameObject("Stamper");
                stamperObj.transform.parent = gaiaObj.transform;
                Stamper stamper = stamperObj.AddComponent<Stamper>();
                stamper.m_resources = m_resources;
                stamper.FitToTerrain();
            }
            Selection.activeGameObject = stamperObj;
        }

        /// <summary>
        /// Select or create a scanner
        /// </summary>
        GameObject CreateScanner()
        {
            GameObject gaiaObj = GameObject.Find("Gaia");
            if (gaiaObj == null)
            {
                gaiaObj = new GameObject("Gaia");
            }

            GameObject scannerObj = GameObject.Find("Scanner");
            if (scannerObj == null)
            {
                scannerObj = new GameObject("Scanner");
                scannerObj.transform.parent = gaiaObj.transform;
                Scanner scanner = scannerObj.AddComponent<Scanner>();

                //Load the material to draw it
                string matPath = GetAssetPath("GaiaScannerMaterial");
                if (!string.IsNullOrEmpty(matPath))
                {
                    scanner.m_previewMaterial = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                }

                //See if we can make some useful defaults for the scanner
                Terrain t = Terrain.activeTerrain;
                Bounds b = new Bounds();
                if (TerrainHelper.GetTerrainBounds(t, ref b))
                {
                    scannerObj.transform.localPosition = b.center;
                }
            }
            return scannerObj;
        }

        /// <summary>
        /// Create or select the existing visualiser
        /// </summary>
        /// <returns>Exsiting visualiser</returns>
        GameObject CreateVisualiser()
        {
            GameObject gaiaObj = GameObject.Find("Gaia");
            if (gaiaObj == null)
            {
                gaiaObj = new GameObject("Gaia");
            }
            GameObject visualiserObj = GameObject.Find("Visualiser");
            if (visualiserObj == null)
            {
                visualiserObj = new GameObject("Visualiser");
                visualiserObj.AddComponent<ResourceVisualiser>();
                visualiserObj.transform.parent = gaiaObj.transform;
            }
            ResourceVisualiser visualiser = visualiserObj.GetComponent<ResourceVisualiser>();
            visualiser.m_resources = m_resources;
            return visualiserObj;
        }


        /*
        /// <summary>
        /// Create a stamp spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateStampSpawner()
        {
            float range = (m_defaults.m_terrainSize / 2) * m_defaults.m_tilesX;
            return m_resources.CreateStampSpawner(range);
        }
         */

        /// <summary>
        /// Create a texture spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateTextureSpawner()
        {
            float range = (m_defaults.m_terrainSize / 2) * m_defaults.m_tilesX;
            return m_resources.CreateTextureSpawner(range);
        }

        /// <summary>
        /// Create a detail spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateDetailSpawner()
        {
            float range = (m_defaults.m_terrainSize / 2) * m_defaults.m_tilesX;
            return m_resources.CreateDetailSpawner(range);
        }

        /// <summary>
        /// Create a tree spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateTreeSpawner()
        {
            float range = (m_defaults.m_terrainSize / 2) * m_defaults.m_tilesX;
            return m_resources.CreateTreeSpawner(range);
        }

        /// <summary>
        /// Create a game object spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateGameObjectSpawner()
        {
            float range = (m_defaults.m_terrainSize / 2) * m_defaults.m_tilesX;
            return m_resources.CreateGameObjectSpawner(range);
        }

        /// <summary>
        /// Create a spawner group
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateSpawnerGroup()
        {
            return m_resources.CreateSpawnerGroup();
        }

        /// <summary>
        /// Create a player
        /// </summary>
        void CreatePlayer()
        {
            GameObject fps = GetAssetPrefab("FPSController");
            if (fps != null)
            {
                //Place at centre of world at game height
                Vector3 location = Vector3.zero;
                Terrain t = TerrainHelper.GetTerrain(location);
                if (t != null)
                {
                    location.y = t.SampleHeight(location) + 1f;
                    Instantiate(fps, location, Quaternion.identity);
                }
                else
                {
                    Debug.LogWarning("Unable to find terrain to place character on.");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("OOPS!", "Unable to locate the FPSCharacter prefab!! Please Import Unity Standard Character Assets and try again.", "OK");
            }
        }

        /// <summary>
        /// Create a wind zone
        /// </summary>
        void CreateWindZone()
        {
            GameObject windZoneObj = GameObject.Find("WindZone");
            if (windZoneObj == null)
            {
                windZoneObj = new GameObject("WindZone");
                WindZone windZone = windZoneObj.AddComponent<WindZone>();
                windZone.windMain = 0.2f;
                windZone.windTurbulence = 0.2f;
                windZone.windPulseMagnitude = 0.2f;
                windZone.windPulseFrequency = 0.01f;
                GameObject gaiaObj = GameObject.Find("Gaia");
                if (gaiaObj != null)
                {
                    windZoneObj.transform.parent = gaiaObj.transform;
                }
            }
            else
            {
                Debug.Log("You already have a WindZone in your scene!");
            }
        }


        /// <summary>
        /// Create water
        /// </summary>
        void CreateWater()
        {
            GameObject waterObj = GameObject.Find("Water");
            if (waterObj == null)
            {
                GameObject waterPrefab = GetAssetPrefab("Water4Advanced");
                if (waterPrefab != null)
                {
                    Vector3 location = Vector3.zero;
                    location.y = m_resources.m_seaLevel;
                    waterObj = Instantiate(waterPrefab, location, Quaternion.identity) as GameObject;
                    waterObj.transform.localScale = new Vector3(
                        (m_defaults.m_terrainSize * Mathf.Max(m_defaults.m_tilesX, m_defaults.m_tilesZ)) / 100 + 20,
                        1f,
                        (m_defaults.m_terrainSize * Mathf.Max(m_defaults.m_tilesX, m_defaults.m_tilesZ)) / 100 + 20);
                }
                else
                {
                    EditorUtility.DisplayDialog("OOPS!", "Unable to locate the Water4Advanced prefab!! Please Import Unity Standard Environment Assets and try again.", "OK");
                }
                GameObject gaiaObj = GameObject.Find("Gaia");
                if (gaiaObj != null && waterObj != null)
                {
                    waterObj.transform.parent = gaiaObj.transform;
                }
            }
            else
            {
                Debug.Log("You already have a Water Object in your scene!");
            }
        }


        /// <summary>
        /// Display a button that takes editor indentation into account
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        bool DisplayButton(GUIContent content)
        {
            TextAnchor oldalignment = GUI.skin.button.alignment;
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            Rect btnR = EditorGUILayout.BeginHorizontal();
            btnR.xMin += (EditorGUI.indentLevel * 18f);
            btnR.height += 20f;
            btnR.width -= 4f;
            bool result = GUI.Button(btnR, content);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(22);
            GUI.skin.button.alignment = oldalignment;
            return result;
        }

        /// <summary>
        /// Get a content label - look the tooltip up if possible
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        GUIContent GetLabel(string name)
        {
            string tooltip = "";
            if (m_tooltips.TryGetValue(name, out tooltip))
            {
                return new GUIContent(name, tooltip);
            }
            else
            {
                return new GUIContent(name);
            }
        }

        /// <summary>
        /// The tooltips
        /// </summary>
        static Dictionary<string, string> m_tooltips = new Dictionary<string, string>
        {
            { "Execution Mode", "The way this spawner runs. Design time : At design time only. Runtime Interval : At run time on a timed interval. Runtime Triggered Interval : At run time on a timed interval, and only when the tagged game object is closer than the trigger range from the center of the spawner." },
        };

   
        /// <summary>
        /// Get the asset path of the first thing that matches the name
        /// </summary>
        /// <param name="name">Name to search for</param>
        /// <returns></returns>
        string GetAssetPath(string name)
        {
            string[] assets = AssetDatabase.FindAssets(name, null);
            if (assets.Length > 0)
            {
                return AssetDatabase.GUIDToAssetPath(assets[0]);
            }
            return null;
        }

        /// <summary>
        /// Return the first prefab that matches the given name from within the current project
        /// </summary>
        /// <param name="name">Asset to searhc for</param>
        /// <returns>Returns the prefab or null</returns>
        GameObject GetAssetPrefab(string name)
        {
            string[] assets = AssetDatabase.FindAssets(name, null);
            for (int idx = 0; idx < assets.Length; idx++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
                if (path.Contains(".prefab"))
                {
                    return AssetDatabase.LoadAssetAtPath<GameObject>(path);
                }
            }
            return null;
        }

        /// <summary>
        /// Get the first scriptable objet that matches the name
        /// </summary>
        /// <param name="name">Name to search for</param>
        /// <returns>First scriptable object or null</returns>
        ScriptableObject GetScriptableObject(string name)
        {
            string[] assets = AssetDatabase.FindAssets(name, null);
            if (assets.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[0]);
                return AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            }
            return null;
        }
    }
}