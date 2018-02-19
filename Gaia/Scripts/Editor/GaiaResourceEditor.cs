using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Gaia {

    /// <summary>
    /// Editor for reource manager
    /// </summary>
    [CustomEditor(typeof(GaiaResource))]
    public class GaiaResourceEditor : Editor {

        GUIStyle m_boxStyle;
        GUIStyle m_wrapStyle;
        GaiaResource m_resource;

        void OnEnable()
        {
        }

        public override void OnInspectorGUI()
        {
            //Get our resource
            m_resource = (GaiaResource)target;

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
            GUILayout.BeginVertical("Gaia Resource", m_boxStyle);
            GUILayout.Space(20);
            //EditorGUILayout.LabelField("The resource manager allows you to manage the resources used by your terrain and spawner. To see what every setting does you can hover over it.\n\nGet From Terrain - Pick up resources and settings from the current terrain.\n\nUpdate DNA - Updates DNA for all resources and automatically calculate sizes.\n\nApply To Terrain - Apply terrain specific settings such as texture, detail and tree prototypes back into the terrain. Prefab this settings to save time creating your next terrain.", m_wrapStyle);
            EditorGUILayout.LabelField("These are the resources used the Spawning & Stamping system. Create a terrain, add textures, details and trees, then press Get From Terrain to load. To see how the settings influence the system you can hover over them.", m_wrapStyle);
            GUILayout.EndVertical();

            float oldSeaLevel = m_resource.m_seaLevel;
            float oldHeight = m_resource.m_terrainHeight;

            EditorGUI.BeginChangeCheck();

            DrawDefaultInspector();

            DropAreaGUI();

            GUILayout.BeginVertical("Resource Controller", m_boxStyle);
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(GetLabel("Get From Terrain")))
            {
                if (EditorUtility.DisplayDialog("Update Resource Prototypes from Terrain ?", "Are you sure you want to get / update your resource prototypes from the terrain ? This will update your settings and can not be undone !", "Yes", "No"))
                {
                    m_resource.UpdatePrototypesFromTerrain();
                    EditorUtility.SetDirty(m_resource);
                }
            }
            if (GUILayout.Button(GetLabel("Apply To Terrains")))
            {
                if (EditorUtility.DisplayDialog("Apply Resources to ALL Terrains ?", "Are you sure you want to apply your resource prototypes to ALL terrains ? This can not be undone !", "Yes", "No"))
                {
                    m_resource.ApplyPrototypesToTerrain();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(GetLabel("Visualise")))
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
                visualiser.m_resources = m_resource;
                Selection.activeGameObject = visualiserObj;
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
            GUILayout.EndVertical();

            //Check for changes, make undo record, make changes and let editor know we are dirty
            if (EditorGUI.EndChangeCheck())
            {
                if (oldHeight != m_resource.m_terrainHeight)
                {
                    m_resource.ChangeHeight(oldHeight, m_resource.m_terrainHeight);
                }

                if (oldSeaLevel != m_resource.m_seaLevel)
                {
                    m_resource.ChangeSeaLevel(oldSeaLevel, m_resource.m_seaLevel);
                }

                Undo.RecordObject(m_resource, "Made resource changes");
                EditorUtility.SetDirty(m_resource);
            }
        }

        public void DropAreaGUI()
        {
            //Drop out if no resource selected
            if (m_resource == null)
            {
                return;
            }

            //Ok - set up for drag and drop
            Event evt = Event.current;
            Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            GUI.Box(drop_area, "Drop Game Object Prefabs Here", m_boxStyle);

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!drop_area.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        foreach (Object dragged_object in DragAndDrop.objectReferences)
                        {
                            if (PrefabUtility.GetPrefabType(dragged_object) != PrefabType.None)
                            {
                                m_resource.AddGameObject(dragged_object as GameObject);
                            }
                            else
                            {
                                Debug.LogWarning("You may only add prefabs!");
                            }
                        }
                    }
                    break;
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
            { "Get From Terrain", "Get or update the resource prototypes from the current terrain." },
            { "Apply To Terrains", "Apply the resource prototypes into all existing terrains." },
            { "Visualise", "Visualise the fitness of resource prototypes." },
        };

    }
}
