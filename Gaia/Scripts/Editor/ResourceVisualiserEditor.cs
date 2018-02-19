using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace Gaia
{
    /// <summary>
    /// Editor for reource visualiser
    /// </summary>
    [CustomEditor(typeof(ResourceVisualiser))]
    public class ResourceVisualiserEditor : Editor
    {
        GUIStyle m_boxStyle;
        GUIStyle m_wrapStyle;
        ResourceVisualiser m_visualiser;

        void OnEnable()
        {
            //Grab the active terrain height map
            if (Terrain.activeTerrain != null)
            {
                //Get our manager
                m_visualiser = (ResourceVisualiser)target;
                if (m_visualiser.m_fitnessCollisionLayers.value == 0)
                {
                    m_visualiser.m_fitnessCollisionLayers.value = 1 << LayerMask.NameToLayer("Default");
                }

                m_visualiser.Visualise();
            }
        }

        public override void OnInspectorGUI()
        {
            //Get our manager
            m_visualiser = (ResourceVisualiser)target;

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
                m_wrapStyle.wordWrap = true;
            }

            //Create a nice text intro
            GUILayout.BeginVertical("Resource Visualiser", m_boxStyle);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("The resource visualiser allows you to visualise and edit your resources spawn criteria.", m_wrapStyle);
            GUILayout.EndVertical();

            EditorGUI.BeginChangeCheck();

            DrawDefaultInspector();

            //Create a nice text intro
            GUILayout.BeginVertical("Resource Editor", m_boxStyle);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Select and edit your resource here. Changes will update in real time.", m_wrapStyle);
            GUILayout.EndVertical();

            m_visualiser.m_selectedResourceType = (Gaia.GaiaConstants.SpawnerResourceType)EditorGUILayout.EnumPopup(GetLabel("Resource Type"), m_visualiser.m_selectedResourceType);

            GUIContent[] assetChoices;
            switch (m_visualiser.m_selectedResourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    {
                        assetChoices = new GUIContent[m_visualiser.m_resources.m_texturePrototypes.Length];
                        for (int assetIdx = 0; assetIdx < m_visualiser.m_resources.m_texturePrototypes.Length; assetIdx++)
                        {
                            assetChoices[assetIdx] = new GUIContent(m_visualiser.m_resources.m_texturePrototypes[assetIdx].m_name);
                        }
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    {
                        assetChoices = new GUIContent[m_visualiser.m_resources.m_detailPrototypes.Length];
                        for (int assetIdx = 0; assetIdx < m_visualiser.m_resources.m_detailPrototypes.Length; assetIdx++)
                        {
                            assetChoices[assetIdx] = new GUIContent(m_visualiser.m_resources.m_detailPrototypes[assetIdx].m_name);
                        }
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    {
                        assetChoices = new GUIContent[m_visualiser.m_resources.m_treePrototypes.Length];
                        for (int assetIdx = 0; assetIdx < m_visualiser.m_resources.m_treePrototypes.Length; assetIdx++)
                        {
                            assetChoices[assetIdx] = new GUIContent(m_visualiser.m_resources.m_treePrototypes[assetIdx].m_name);
                        }
                        break;
                    }
                default:
                    {
                        assetChoices = new GUIContent[m_visualiser.m_resources.m_gameObjectPrototypes.Length];
                        for (int assetIdx = 0; assetIdx < m_visualiser.m_resources.m_gameObjectPrototypes.Length; assetIdx++)
                        {
                            assetChoices[assetIdx] = new GUIContent(m_visualiser.m_resources.m_gameObjectPrototypes[assetIdx].m_name);
                        }
                        break;
                    }
            }


            if (assetChoices.Length > 0)
            {
                m_visualiser.m_selectedResourceIdx = EditorGUILayout.Popup(GetLabel("Selected Resource"), m_visualiser.m_selectedResourceIdx, assetChoices);

                //Then select and display the editor
                switch (m_visualiser.m_selectedResourceType)
                {
                    case GaiaConstants.SpawnerResourceType.TerrainTexture:
                        {
                            if (m_visualiser.m_selectedResourceIdx >= m_visualiser.m_resources.m_texturePrototypes.Length)
                            {
                                m_visualiser.m_selectedResourceIdx = 0;
                            }
                            ResourceProtoTextureSO so = ScriptableObject.CreateInstance<ResourceProtoTextureSO>();
                            so.m_texture = m_visualiser.m_resources.m_texturePrototypes[m_visualiser.m_selectedResourceIdx];
                            var ed = Editor.CreateEditor(so);
                            ed.OnInspectorGUI();
                            break;
                        }
                    case GaiaConstants.SpawnerResourceType.TerrainDetail:
                        {
                            //Fix up indexes
                            if (m_visualiser.m_selectedResourceIdx >= m_visualiser.m_resources.m_detailPrototypes.Length)
                            {
                                m_visualiser.m_selectedResourceIdx = 0;
                            }
                            ResourceProtoDetailSO so = ScriptableObject.CreateInstance<ResourceProtoDetailSO>();
                            so.m_detail = m_visualiser.m_resources.m_detailPrototypes[m_visualiser.m_selectedResourceIdx];
                            var ed = Editor.CreateEditor(so);
                            ed.OnInspectorGUI();
                            break;
                        }
                    case GaiaConstants.SpawnerResourceType.TerrainTree:
                        {
                            //Fix up indexes
                            if (m_visualiser.m_selectedResourceIdx >= m_visualiser.m_resources.m_treePrototypes.Length)
                            {
                                m_visualiser.m_selectedResourceIdx = 0;
                            }
                            ResourceProtoTreeSO so = ScriptableObject.CreateInstance<ResourceProtoTreeSO>();
                            so.m_tree = m_visualiser.m_resources.m_treePrototypes[m_visualiser.m_selectedResourceIdx];
                            var ed = Editor.CreateEditor(so);
                            ed.OnInspectorGUI();
                            break;
                        }
                    default:
                        {
                            //Fix up indexes
                            if (m_visualiser.m_selectedResourceIdx >= m_visualiser.m_resources.m_gameObjectPrototypes.Length)
                            {
                                m_visualiser.m_selectedResourceIdx = 0;
                            }
                            ResourceProtoGameObjectSO so = ScriptableObject.CreateInstance<ResourceProtoGameObjectSO>();
                            so.m_gameObject = m_visualiser.m_resources.m_gameObjectPrototypes[m_visualiser.m_selectedResourceIdx];
                            var ed = Editor.CreateEditor(so);
                            ed.OnInspectorGUI();
                            break;
                        }
                }
            }

            //Check for changes, make undo record, make changes and let editor know we are dirty
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_visualiser.m_resources, "Made resource changes");
                EditorUtility.SetDirty(m_visualiser.m_resources);
            }

            //Update some key fields in the spawner
            m_visualiser.m_spawner.m_spawnCollisionLayers = m_visualiser.m_fitnessCollisionLayers;
            m_visualiser.m_spawner.m_spawnRange = m_visualiser.m_range;

            //Terrain info
            //Create a nice text intro
            GUILayout.BeginVertical("Terrain Info", m_boxStyle);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Hit control key to view detailed terrain information at current mouse position.", m_wrapStyle);
            GUILayout.EndVertical();

            EditorGUILayout.Vector3Field(GetLabel("Location"), m_visualiser.m_lastHitPoint);

            if (!string.IsNullOrEmpty(m_visualiser.m_lastHitObjectname))
            {
                EditorGUILayout.TextField(GetLabel("Hit Object"), m_visualiser.m_lastHitObjectname);
            }
            EditorGUILayout.Toggle(GetLabel("Virgin"), m_visualiser.m_lastHitWasVirgin);
            EditorGUILayout.FloatField(GetLabel("Hit Height"), m_visualiser.m_lastHitHeight);
            EditorGUILayout.FloatField(GetLabel("Terrain Height"), m_visualiser.m_lastHitTerrainHeight);
            EditorGUILayout.FloatField(GetLabel("Terrain Slope"), m_visualiser.m_lastHitTerrainSlope);
            EditorGUILayout.FloatField(GetLabel("Area Slope"), m_visualiser.m_lastHitAreaSlope);
            EditorGUILayout.FloatField(GetLabel("Fitness"), m_visualiser.m_lastHitFitness);
        }

        void OnSceneGUI()
        {
            if (Event.current.control == true)
            {
                //Work out where the mouse is and get fitness
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, 10000f))
                {
                    if (hitInfo.point != m_visualiser.m_lastHitPoint)
                    {
                        m_visualiser.m_lastHitPoint = hitInfo.point;
                        SpawnInfo spawnInfo = m_visualiser.GetSpawnInfo(hitInfo.point);
                        m_visualiser.m_lastHitObjectname = spawnInfo.m_hitObject.name;
                        m_visualiser.m_lastHitFitness = spawnInfo.m_fitness;
                        m_visualiser.m_lastHitHeight = spawnInfo.m_hitLocationWU.y;
                        m_visualiser.m_lastHitTerrainHeight = spawnInfo.m_terrainHeightWU;
                        m_visualiser.m_lastHitTerrainSlope = spawnInfo.m_terrainSlopeWU;
                        m_visualiser.m_lastHitAreaSlope = spawnInfo.m_areaHitSlopeWU;
                        m_visualiser.m_lastHitWasVirgin = spawnInfo.m_wasVirginTerrain;
                    }
                }
            }
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
            { "Ground", "Position the spawner at ground level on the terrain." },
            { "Reset", "Reset the spawner back to its initial state. Starting spawn again will generate exact same result provided that nothing else in the terrain has changed." },
            { "Spawn", "Run a single spawn interation." },
        };

    }
}