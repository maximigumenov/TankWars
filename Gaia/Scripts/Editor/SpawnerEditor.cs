using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Gaia
{
    [CustomEditor(typeof(Spawner))]
    public class SpawnerEditor : Editor
    {
        GUIStyle m_boxStyle;
        GUIStyle m_wrapStyle;
        Spawner m_spawner;
        DateTime m_timeSinceLastUpdate = DateTime.Now;
        bool m_startedUpdates = false;

        void OnEnable()
        {
            //Get our spawner
            m_spawner = (Spawner)target;

            //Clean up any rules that relate to missing resources
            CleanUpRules();

            //Do some simple sanity checks
            if (m_spawner.m_rndGenerator == null)
            {
                m_spawner.m_rndGenerator = new System.Random(m_spawner.m_seed);
            }

            if (m_spawner.m_spawnFitnessAttenuator == null)
            {
                m_spawner.m_spawnFitnessAttenuator = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1.0f), new Keyframe(1f, 0.0f));
            }

            StartEditorUpdates();
        }

        void OnDisable()
        {
        }


        /// <summary>
        /// Start editor updates
        /// </summary>
        public void StartEditorUpdates()
        {
            if (!m_startedUpdates)
            {
                m_startedUpdates = true;
                EditorApplication.update += EditorUpdate;
            }
        }

        /// <summary>
        /// Stop editor updates
        /// </summary>
        public void StopEditorUpdates()
        {
            if (m_startedUpdates)
            {
                m_startedUpdates = false;
                EditorApplication.update -= EditorUpdate;
            }
        }

        /// <summary>
        /// This is used just to force the editor to repaint itself
        /// </summary>
        void EditorUpdate()
        {
            if (m_spawner != null)
            {
                if (m_spawner.m_updateCoroutine != null)
                {
                    if ((DateTime.Now - m_timeSinceLastUpdate).TotalMilliseconds > 500)
                    {
                        //Debug.Log("Active repainting spawner " + m_spawner.gameObject.name);
                        m_timeSinceLastUpdate = DateTime.Now;
                        Repaint();
                    }
                }
                else
                {
                    if ((DateTime.Now - m_timeSinceLastUpdate).TotalSeconds > 5)
                    {
                        //Debug.Log("Inactive repainting spawner " + m_spawner.gameObject.name);
                        m_timeSinceLastUpdate = DateTime.Now;
                        Repaint();
                    }
                }
            }
        }

        /// <summary>
        /// Draw the UI
        /// </summary>
        public override void OnInspectorGUI()
        {
            //Get our spawner
            m_spawner = (Spawner)target;

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

            //Hide the transform
            //spawner.transform.hideFlags = HideFlags.HideInInspector;

            //Create a nice text intro
            GUILayout.BeginVertical("Spawner", m_boxStyle);
                GUILayout.Space(20);
                EditorGUILayout.LabelField("The Spawner allows you to place features into your terrain. Spawner chooses location and Resource calculates fitness. If fitness is good enough then the resource is spawned.", m_wrapStyle);
            GUILayout.EndVertical();

            //Disable if spawning
            if (m_spawner.m_spawnProgress > 0f)
            {
                GUI.enabled = false;
            }

            EditorGUI.BeginChangeCheck();

            GUILayout.BeginVertical("Spawner Settings", m_boxStyle);

                GUILayout.Space(20);

                GaiaResource resources = (GaiaResource)EditorGUILayout.ObjectField(GetLabel("Resources"), m_spawner.m_resources, typeof(GaiaResource), false);

                int seed = EditorGUILayout.IntField(GetLabel("Seed"), m_spawner.m_seed);

                float spawnRange = EditorGUILayout.FloatField(GetLabel("Range"), m_spawner.m_spawnRange);
            
                Gaia.GaiaConstants.SpawnerShape spawnerShape = (Gaia.GaiaConstants.SpawnerShape)EditorGUILayout.EnumPopup(GetLabel("Shape"), m_spawner.m_spawnerShape);

                LayerMask spawnerLayerMask = LayerMaskField(GetLabel("Collision Layers"), m_spawner.m_spawnCollisionLayers);

                Gaia.GaiaConstants.OperationMode mode = (Gaia.GaiaConstants.OperationMode)EditorGUILayout.EnumPopup(GetLabel("Execution Mode"), m_spawner.m_mode);

                float spawnerInterval;
                if (mode == GaiaConstants.OperationMode.DesignTime)
                {
                    spawnerInterval = m_spawner.m_spawnInterval;
                }
                else
                {
                    spawnerInterval = EditorGUILayout.FloatField(GetLabel("Spawn Interval"), m_spawner.m_spawnInterval);
                }

                float triggerRange = 0f;
                string triggerTags;
                if (mode != GaiaConstants.OperationMode.RuntimeTriggeredInterval)
                {
                    triggerRange = m_spawner.m_triggerRange;
                    triggerTags = m_spawner.m_triggerTags;
                }
                else
                {
                    triggerRange = EditorGUILayout.FloatField(GetLabel("Trigger Range"), m_spawner.m_triggerRange);
                    triggerTags = EditorGUILayout.TextField(GetLabel("Trigger Tags"), m_spawner.m_triggerTags);
                }


                Gaia.GaiaConstants.SpawnerRuleSelector spawnRuleSelector = (Gaia.GaiaConstants.SpawnerRuleSelector)EditorGUILayout.EnumPopup(GetLabel("Rule Selector"), m_spawner.m_spawnRuleSelector);
                Gaia.GaiaConstants.SpawnerLocation spawnLocationAlgorithm = (Gaia.GaiaConstants.SpawnerLocation)EditorGUILayout.EnumPopup(GetLabel("Location Selector"), m_spawner.m_spawnLocationAlgorithm);

                float locationIncrement = m_spawner.m_locationIncrement;
                float maxJitteredLocationOffsetPct = m_spawner.m_maxJitteredLocationOffsetPct;
                int locationChecksPerInt = m_spawner.m_locationChecksPerInt;
                int maxSeededClusterSize = m_spawner.m_maxRandomClusterSize;
                if (spawnLocationAlgorithm == GaiaConstants.SpawnerLocation.EveryLocation || spawnLocationAlgorithm == GaiaConstants.SpawnerLocation.EveryLocationJittered)
                {
                    locationIncrement = EditorGUILayout.FloatField(GetLabel("Location Increment"), m_spawner.m_locationIncrement);
                    if (spawnLocationAlgorithm == GaiaConstants.SpawnerLocation.EveryLocationJittered)
                    {
                        maxJitteredLocationOffsetPct = EditorGUILayout.Slider(GetLabel("Max Jitter Percent"), m_spawner.m_maxJitteredLocationOffsetPct, 0f, 1f);
                    }
                }
                else
                {
                    locationChecksPerInt = EditorGUILayout.IntSlider(GetLabel("Locations Per Spawn"), m_spawner.m_locationChecksPerInt, 1, 10000);
                    if (spawnLocationAlgorithm == GaiaConstants.SpawnerLocation.RandomLocationClustered)
                    {
                        maxSeededClusterSize = EditorGUILayout.IntSlider(GetLabel("Max Cluster Size"), m_spawner.m_maxRandomClusterSize, 1, 1000);
                    }
                }

                AnimationCurve spawnRangeAttenuator = EditorGUILayout.CurveField(GetLabel("Distance Mask"), m_spawner.m_spawnFitnessAttenuator);
                Gaia.GaiaConstants.ImageFitnessFilterMode areaMaskMode = (Gaia.GaiaConstants.ImageFitnessFilterMode)EditorGUILayout.EnumPopup(GetLabel("Area Mask"), m_spawner.m_areaMaskMode);
                Texture2D imageMask = m_spawner.m_imageMask;
                bool imageMaskInvert = m_spawner.m_imageMaskInvert;
                bool imageMaskNormalise = m_spawner.m_imageMaskNormalise;
                bool imageMaskFlip = m_spawner.m_imageMaskFlip;
                int imageMaskSmoothIterations = m_spawner.m_imageMaskSmoothIterations;
                if (areaMaskMode == GaiaConstants.ImageFitnessFilterMode.ImageAlphaChannel ||
                    areaMaskMode == GaiaConstants.ImageFitnessFilterMode.ImageBlueChannel ||
                    areaMaskMode == GaiaConstants.ImageFitnessFilterMode.ImageGreenChannel ||
                    areaMaskMode == GaiaConstants.ImageFitnessFilterMode.ImageGreyScale ||
                    areaMaskMode == GaiaConstants.ImageFitnessFilterMode.ImageRedChannel)
                {
                    imageMask = (Texture2D)EditorGUILayout.ObjectField(GetLabel("Image Mask"), m_spawner.m_imageMask, typeof(Texture2D), false);
                }
                if (areaMaskMode != GaiaConstants.ImageFitnessFilterMode.None)
                {
                    imageMaskSmoothIterations = EditorGUILayout.IntSlider(GetLabel("Smooth Mask"), m_spawner.m_imageMaskSmoothIterations, 0, 20);
                    imageMaskNormalise = EditorGUILayout.Toggle(GetLabel("Normalise Mask"), m_spawner.m_imageMaskNormalise);
                    imageMaskInvert = EditorGUILayout.Toggle(GetLabel("Invert Mask"), m_spawner.m_imageMaskInvert);
                    imageMaskFlip = EditorGUILayout.Toggle(GetLabel("Flip Mask"), m_spawner.m_imageMaskFlip);
                }


            GUILayout.EndVertical();

            //Back the rules up
            SpawnRule rule, newRule;
            List<SpawnRule> ruleBackup = new List<SpawnRule>();
            for (int idx = 0; idx < m_spawner.m_spawnerRules.Count; idx++)
            {
                rule = m_spawner.m_spawnerRules[idx];
                newRule = new SpawnRule();
                newRule.m_activeInstanceCnt = rule.m_activeInstanceCnt;
                newRule.m_currInstanceCnt = rule.m_currInstanceCnt;
                newRule.m_failureRate = rule.m_failureRate;
                newRule.m_inactiveInstanceCnt = rule.m_inactiveInstanceCnt;
                newRule.m_isActive = rule.m_isActive;
                newRule.m_isFoldedOut = rule.m_isFoldedOut;
                newRule.m_maxInstances = rule.m_maxInstances;
                newRule.m_minViableFitness = rule.m_minViableFitness;
                newRule.m_name = rule.m_name;
                newRule.m_resourceType = rule.m_resourceType;
                newRule.m_resourceIdx = rule.m_resourceIdx;
                ruleBackup.Add(newRule);
            }

            //Display the rules
            GUILayout.BeginVertical("Spawner Rules", m_boxStyle);
            GUILayout.Space(21);

            if (resources != null)
            {
                Rect addRect = EditorGUILayout.BeginVertical();
                addRect.y -= 20f;
                addRect.x = addRect.width - 10;
                addRect.width = 25;
                addRect.height = 20;
                if (GUI.Button(addRect, "+"))
                {
                    ruleBackup.Add(new SpawnRule());
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUI.indentLevel++;
            for (int ruleIdx = 0; ruleIdx < ruleBackup.Count; ruleIdx++ )
            {
                rule = ruleBackup[ruleIdx];
                if (rule.m_isActive)
                {
                    rule.m_isFoldedOut = EditorGUILayout.Foldout(rule.m_isFoldedOut, rule.m_name);
                }
                else
                {
                    rule.m_isFoldedOut = EditorGUILayout.Foldout(rule.m_isFoldedOut, rule.m_name + " [inactive]");
                }
                if (rule.m_isFoldedOut)
                {
                    rule.m_resourceType = (Gaia.GaiaConstants.SpawnerResourceType)EditorGUILayout.EnumPopup(GetLabel("Resource Type"), rule.m_resourceType);

                    GUIContent[] assetChoices = null;
                    switch (rule.m_resourceType)
                    {
                        case GaiaConstants.SpawnerResourceType.TerrainTexture:
                            {
                                assetChoices = new GUIContent[m_spawner.m_resources.m_texturePrototypes.Length];
                                for (int assetIdx = 0; assetIdx < m_spawner.m_resources.m_texturePrototypes.Length; assetIdx++)
                                {
                                    assetChoices[assetIdx] = new GUIContent(m_spawner.m_resources.m_texturePrototypes[assetIdx].m_name);
                                }
                                break;
                            }
                        case GaiaConstants.SpawnerResourceType.TerrainDetail:
                            {
                                assetChoices = new GUIContent[m_spawner.m_resources.m_detailPrototypes.Length];
                                for (int assetIdx = 0; assetIdx < m_spawner.m_resources.m_detailPrototypes.Length; assetIdx++)
                                {
                                    assetChoices[assetIdx] = new GUIContent(m_spawner.m_resources.m_detailPrototypes[assetIdx].m_name);
                                }
                                break;
                            }
                        case GaiaConstants.SpawnerResourceType.TerrainTree:
                            {
                                assetChoices = new GUIContent[m_spawner.m_resources.m_treePrototypes.Length];
                                for (int assetIdx = 0; assetIdx < m_spawner.m_resources.m_treePrototypes.Length; assetIdx++)
                                {
                                    assetChoices[assetIdx] = new GUIContent(m_spawner.m_resources.m_treePrototypes[assetIdx].m_name);
                                }
                                break;
                            }
                        case GaiaConstants.SpawnerResourceType.GameObject:
                            {
                                assetChoices = new GUIContent[m_spawner.m_resources.m_gameObjectPrototypes.Length];
                                for (int assetIdx = 0; assetIdx < m_spawner.m_resources.m_gameObjectPrototypes.Length; assetIdx++)
                                {
                                    assetChoices[assetIdx] = new GUIContent(m_spawner.m_resources.m_gameObjectPrototypes[assetIdx].m_name);
                                }
                                break;
                            }
                            /*
                        default:
                            {
                                assetChoices = new GUIContent[m_spawner.m_resources.m_stampPrototypes.Length];
                                for (int assetIdx = 0; assetIdx < m_spawner.m_resources.m_stampPrototypes.Length; assetIdx++)
                                {
                                    assetChoices[assetIdx] = new GUIContent(m_spawner.m_resources.m_stampPrototypes[assetIdx].m_name);
                                }
                                break;
                            } */
                    }

                    if (assetChoices.Length <= 0)
                    {

                    }
                    else
                    {
                        rule.m_resourceIdx = EditorGUILayout.Popup(GetLabel("Selected Resource"), rule.m_resourceIdx, assetChoices);
                        switch (rule.m_resourceType)
                        {
                            case GaiaConstants.SpawnerResourceType.TerrainTexture:
                                {
                                    rule.m_name = m_spawner.m_resources.m_texturePrototypes[rule.m_resourceIdx].m_name;
                                    break;
                                }
                            case GaiaConstants.SpawnerResourceType.TerrainDetail:
                                {
                                    rule.m_name = m_spawner.m_resources.m_detailPrototypes[rule.m_resourceIdx].m_name;
                                    break;
                                }
                            case GaiaConstants.SpawnerResourceType.TerrainTree:
                                {
                                    rule.m_name = m_spawner.m_resources.m_treePrototypes[rule.m_resourceIdx].m_name;
                                    break;
                                }
                            case GaiaConstants.SpawnerResourceType.GameObject:
                                {
                                    rule.m_name = m_spawner.m_resources.m_gameObjectPrototypes[rule.m_resourceIdx].m_name;

                                    //See if we can find a custom fitness
                                    if (m_spawner.m_resources.m_gameObjectPrototypes[rule.m_resourceIdx].m_instances.Length > 0)
                                    {
                                        GameObject go = m_spawner.m_resources.m_gameObjectPrototypes[rule.m_resourceIdx].m_instances[0].m_desktopPrefab;
                                        bool gotFitness = false;
                                        if (go.GetComponent<IFitness>() != null)
                                        {
                                            gotFitness = true;
                                        }
                                        else
                                        {
                                            if (go.GetComponentInChildren<IFitness>() != null)
                                            {
                                                gotFitness = true;
                                            }
                                        }
                                        if (gotFitness)
                                        {
                                            Debug.Log("Got a fitness filter on " + go.name);
                                        }
                                    }
                                    break;
                                }
                                /*
                            default:
                                {
                                    rule.m_name = m_spawner.m_resources.m_stampPrototypes[rule.m_resourceIdx].m_name;
                                    break;
                                } */
                        }

                        //Check to see if we can use extended fitness and spawner

                        rule.m_minViableFitness = EditorGUILayout.Slider(GetLabel("Min Viable Fitness"), rule.m_minViableFitness, 0f, 1f);
                        rule.m_failureRate = EditorGUILayout.Slider(GetLabel("Failure Rate"), rule.m_failureRate, 0f, 1f);
                        rule.m_maxInstances = EditorGUILayout.IntField(GetLabel("Max Instances"), rule.m_maxInstances);
                        rule.m_isActive = EditorGUILayout.Toggle(GetLabel("Active"), rule.m_isActive);

                        if (m_spawner.m_showStatistics)
                        {
                            //EditorGUILayout.LabelField(GetLabel("Curr Inst Count"), new GUIContent(rule.m_currInstanceCnt.ToString()));
                            EditorGUILayout.LabelField(GetLabel("Instances Spawned"), new GUIContent(rule.m_activeInstanceCnt.ToString()));
                            //EditorGUILayout.LabelField(GetLabel("Inactive Inst Count"), new GUIContent(rule.m_inactiveInstanceCnt.ToString()));
                        }
                    }


                    GUILayout.Space(20);
                    Rect delRect = EditorGUILayout.BeginHorizontal();
                    delRect.x += 17;
                    delRect.y -= 20;
                    delRect.width = 17;
                    delRect.height += 18;
                    if (GUI.Button(delRect, "-"))
                    {
                        ruleBackup.Remove(rule);
                    }

                    delRect.x += 25f;
                    delRect.width += 50f;
                    if (GUI.Button(delRect, "Visualise"))
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
                        visualiser.m_resources = m_spawner.m_resources;
                        visualiser.m_selectedResourceType = rule.m_resourceType;
                        visualiser.m_selectedResourceIdx = rule.m_resourceIdx;
                        Selection.activeGameObject = visualiserObj;
                    }
                    EditorGUILayout.EndHorizontal();

                }
            }
            EditorGUI.indentLevel--;
            GUILayout.EndVertical();

            //Show statistics or not
            if (m_spawner.m_showStatistics)
            {
                GUILayout.BeginVertical("Statistics", m_boxStyle);
                GUILayout.Space(20);
                EditorGUILayout.LabelField(GetLabel("Active Rules"), GetLabel(m_spawner.m_activeRuleCnt.ToString()));
                EditorGUILayout.LabelField(GetLabel("Inactive Rules"), GetLabel(m_spawner.m_inactiveRuleCnt.ToString()));
                //EditorGUILayout.LabelField(GetLabel("TOTAL Rules"), GetLabel(m_spawner.m_totalRuleCnt.ToString()));
                GUILayout.Space(8);
                //EditorGUILayout.LabelField(GetLabel("Active Instances"), GetLabel(m_spawner.m_activeInstanceCnt.ToString()));
                //EditorGUILayout.LabelField(GetLabel("Inactive Instances"), GetLabel(m_spawner.m_inactiveInstanceCnt.ToString()));
                EditorGUILayout.LabelField(GetLabel("TOTAL Instances"), GetLabel(m_spawner.m_totalInstanceCnt.ToString()));
                EditorGUILayout.LabelField(GetLabel("MAX INSTANCES"), GetLabel(m_spawner.m_maxInstanceCnt.ToString()));
                GUILayout.EndVertical();
            }

            GUILayout.BeginVertical("Layout Helpers", m_boxStyle);
            GUILayout.Space(20);
            bool showGizmos = EditorGUILayout.Toggle(GetLabel("Show Gizmos"), m_spawner.m_showGizmos);
            bool showStatistics = m_spawner.m_showStatistics = EditorGUILayout.Toggle(GetLabel("Show Statistics"), m_spawner.m_showStatistics);
            bool showTerrainHelper = m_spawner.m_showTerrainHelper = EditorGUILayout.Toggle(GetLabel("Show Terrain Helper"), m_spawner.m_showTerrainHelper);
            GUILayout.EndVertical();

            //Check for changes, make undo record, make changes and let editor know we are dirty
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_spawner, "Made changes");
                m_spawner.m_mode = mode;
                m_spawner.m_seed = seed;
                m_spawner.m_spawnerShape = spawnerShape;
                m_spawner.m_spawnRuleSelector = spawnRuleSelector;
                m_spawner.m_spawnLocationAlgorithm = spawnLocationAlgorithm;
                m_spawner.m_spawnCollisionLayers = spawnerLayerMask;
                m_spawner.m_locationIncrement = locationIncrement;
                m_spawner.m_maxJitteredLocationOffsetPct = maxJitteredLocationOffsetPct;
                m_spawner.m_locationChecksPerInt = locationChecksPerInt;
                m_spawner.m_maxRandomClusterSize = maxSeededClusterSize;
                m_spawner.m_spawnRange = spawnRange;
                m_spawner.m_spawnFitnessAttenuator = spawnRangeAttenuator;
                m_spawner.m_areaMaskMode = areaMaskMode;
                m_spawner.m_imageMask = imageMask;
                m_spawner.m_imageMaskInvert = imageMaskInvert;
                m_spawner.m_imageMaskFlip = imageMaskFlip;
                m_spawner.m_imageMaskSmoothIterations = imageMaskSmoothIterations;
                m_spawner.m_imageMaskNormalise = imageMaskNormalise;
                m_spawner.m_triggerRange = triggerRange;
                m_spawner.m_triggerTags = triggerTags;
                m_spawner.m_spawnInterval = spawnerInterval;
                m_spawner.m_resources = resources;
                m_spawner.m_spawnerRules = ruleBackup;
                m_spawner.m_showGizmos = showGizmos;
                m_spawner.m_showStatistics = showStatistics;
                m_spawner.m_showTerrainHelper = showTerrainHelper;

                if (m_spawner.m_imageMask != null)
                {
                    Gaia.Utils.MakeTextureReadable(m_spawner.m_imageMask);
                }

                EditorUtility.SetDirty(m_spawner);
            }

            //Terrain control
            if (showTerrainHelper)
            {
                GUILayout.BeginVertical("Terrain Helper", m_boxStyle);
                GUILayout.Space(20);
                GUILayout.BeginHorizontal();

                if (GUILayout.Button(GetLabel("Flatten")))
                {
                    if (EditorUtility.DisplayDialog("Flatten Terrain tiles ?", "Are you sure you want to flatten all terrain tiles - this can not be undone ?", "Yes", "No"))
                    {
                        TerrainHelper.Flatten();
                    }
                }
                if (GUILayout.Button(GetLabel("Smooth")))
                {
                    if (EditorUtility.DisplayDialog("Smooth Terrain tiles ?", "Are you sure you want to smooth all terrain tiles - this can not be undone ?", "Yes", "No"))
                    {
                        TerrainHelper.Smooth(1);
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button(GetLabel("Clear Trees")))
                {
                    if (EditorUtility.DisplayDialog("Clear Terrain trees ?", "Are you sure you want to clear all terrain trees - this can not be undone ?", "Yes", "No"))
                    {
                        TerrainHelper.ClearTrees();
                    }
                }
                if (GUILayout.Button(GetLabel("Clear Details")))
                {
                    if (EditorUtility.DisplayDialog("Clear Terrain details ?", "Are you sure you want to clear all terrain details - this can not be undone ?", "Yes", "No"))
                    {
                        TerrainHelper.ClearDetails();
                    }
                }

                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.EndVertical();
            }

            //Ragardless, re-enable the spawner controls
            GUI.enabled = true;
            
            //Display progress
            if (m_spawner.m_spawnProgress > 0f && m_spawner.m_spawnProgress < 1f)
            {
                GUILayout.BeginVertical("Spawn Controller", m_boxStyle);
                GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(GetLabel("Cancel")))
                {
                    m_spawner.CancelSpawn();
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.EndVertical();
                ProgressBar(string.Format("Progress ({0:0.0}%)", m_spawner.m_spawnProgress * 100f), m_spawner.m_spawnProgress);
            }
            else
            {
                GUILayout.BeginVertical("Spawn Controller", m_boxStyle);
                GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(GetLabel("Ground")))
                {
                    m_spawner.GroundToTerrain();
                }
                if (GUILayout.Button(GetLabel("Fit To Terrain")))
                {
                    m_spawner.FitToTerrain();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button(GetLabel("Reset")))
                {
                    m_spawner.ResetSpawner();
                }
                if (GUILayout.Button(GetLabel("Spawn")))
                {
                    //Check that they are not using terrain based mask - this can give unexpected results
                    if (areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture0 ||
                        areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture1 ||
                        areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture2 ||
                        areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture3
                        )
                    {
                        //Do an alert and fix if necessary
                        if (!m_spawner.IsFitToTerrain())
                        {
                            if (EditorUtility.DisplayDialog("WARNING!", "This feature requires your Spawner to be Fit To Terrain in order to guarantee correct placement.", "Spawn Anyway", "Cancel"))
                            {
                                m_spawner.RunSpawnerIteration();
                            }
                        }
                        else
                        {
                            m_spawner.RunSpawnerIteration();
                        }
                    }
                    else
                    {
                        m_spawner.RunSpawnerIteration();
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.EndVertical();
            }

            //if (GUILayout.Button("Deactivate"))
            //{
            //    spawner.DeactivateAllInstances();
            //}

            GUILayout.Space(5f);
        }

        /// <summary>
        /// Delete any old rules left over from previous resources / changes to resources
        /// </summary>
        void CleanUpRules()
        {
            //Drop out if no spawner or resources
            if (m_spawner == null || m_spawner.m_resources == null)
            {
                return;
            }

            //Drop out if spawner doesnt have resources
            int idx = 0;
            SpawnRule rule;
            bool deleted = false;
            while (idx < m_spawner.m_spawnerRules.Count)
            {
                rule = m_spawner.m_spawnerRules[idx];

                switch (rule.m_resourceType)
                {
                    case GaiaConstants.SpawnerResourceType.TerrainTexture:
                        {
                            if (rule.m_resourceIdx >= m_spawner.m_resources.m_texturePrototypes.Length)
                            {
                                m_spawner.m_spawnerRules.RemoveAt(idx);
                                deleted = true;
                            }
                            else
                            {
                                idx++;
                            }
                            break;
                        }
                    case GaiaConstants.SpawnerResourceType.TerrainDetail:
                        {
                            if (rule.m_resourceIdx >= m_spawner.m_resources.m_detailPrototypes.Length)
                            {
                                m_spawner.m_spawnerRules.RemoveAt(idx);
                                deleted = true;
                            }
                            else
                            {
                                idx++;
                            }
                            break;
                        }
                    case GaiaConstants.SpawnerResourceType.TerrainTree:
                        {
                            if (rule.m_resourceIdx >= m_spawner.m_resources.m_treePrototypes.Length)
                            {
                                m_spawner.m_spawnerRules.RemoveAt(idx);
                                deleted = true;
                            }
                            else
                            {
                                idx++;
                            }
                            break;
                        }
                    case GaiaConstants.SpawnerResourceType.GameObject:
                        {
                            if (rule.m_resourceIdx >= m_spawner.m_resources.m_gameObjectPrototypes.Length)
                            {
                                m_spawner.m_spawnerRules.RemoveAt(idx);
                                deleted = true;
                            }
                            else
                            {
                                idx++;
                            }
                            break;
                        }
                        /*
                    default:
                        {
                            if (rule.m_resourceIdx >= m_spawner.m_resources.m_stampPrototypes.Length)
                            {
                                m_spawner.m_spawnerRules.RemoveAt(idx);
                                deleted = true;
                            }
                            else
                            {
                                idx++;
                            }
                            break;
                        } */
                }
            }
            //Mark it as dirty if we deleted something
            if (deleted)
            {
                EditorUtility.SetDirty(m_spawner);
            }
        }

        /// <summary>
        /// Draw a progress bar
        /// </summary>
        /// <param name="label"></param>
        /// <param name="value"></param>

        void ProgressBar(string label, float value)
        {
            // Get a rect for the progress bar using the same margins as a textfield:
            Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
            EditorGUI.ProgressBar(rect, value, label);
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Handy layer mask interface
        /// </summary>
        /// <param name="label"></param>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        static LayerMask LayerMaskField(GUIContent label, LayerMask layerMask)
        {
            List<string> layers = new List<string>();
            List<int> layerNumbers = new List<int>();

            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                if (layerName != "")
                {
                    layers.Add(layerName);
                    layerNumbers.Add(i);
                }
            }
            int maskWithoutEmpty = 0;
            for (int i = 0; i < layerNumbers.Count; i++)
            {
                if (((1 << layerNumbers[i]) & layerMask.value) > 0)
                    maskWithoutEmpty |= (1 << i);
            }
            maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers.ToArray());
            int mask = 0;
            for (int i = 0; i < layerNumbers.Count; i++)
            {
                if ((maskWithoutEmpty & (1 << i)) > 0)
                    mask |= (1 << layerNumbers[i]);
            }
            layerMask.value = mask;
            return layerMask;
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
            { "Resources", "The object that contains the resources that these rules will apply to." },
            { "Execution Mode", "The way this spawner runs. Design time : At design time only. Runtime Interval : At run time on a timed interval. Runtime Triggered Interval : At run time on a timed interval, and only when the tagged game object is closer than the trigger range from the center of the spawner." },
            { "Shape", "The shape of the spawn area. The spawner will only spawn within this area." },
            { "Range","Distance in meters from the centre of the spawner that the spawner can spawn in. Shown as a red box or sphere in the gizmos." },
            { "Spawn Interval", "The time in seconds between spawn iterations." },
            { "Trigger Range","Distance in meters from the centre of the spawner that the trigger will activate." },
            { "Trigger Tags","The tags of the game objects that will set the spawner off. Multiple tags can be separated by commas eg Player,Minion etc." },
            { "Rule Selector", "The way a rule is selected to be spawned. \nAll : All rules are selected. \nFittest : Only the rule with the fittest spawn criteria is selected. If multiple rules have the same fitness then one will be randomly selected.\nWeighted Fittest : The chance of a rule being selected is directly proportional to its fitness. Fitter rules have more chance of selection. Use this to create more natural blends between objects.\nRandom : Rule selection is random." },
            { "Collision Layers", "Controls which layers are checked for collisions when spawning. Must at least include the layer the terrain is on. Add additional layers if other collisions need to be detected as well. Influences terrain detection, tree detection and game object detection." },
            { "Location Selector", "How the spawner selects locations to spawn in. \nEvery Location: The spawner will attempt to spawn at every location. \nEvery Location Jittered: The spawner will attempt to spawn at every location, but will offset the location by a random jitter factor. Use this to break up lines.\nRandom Location: The spawner will attempt to spawn at random locations.\nRandom Location Clustered: The spawner will attempt to spawn clusters at random locations." },
            { "Location Increment", "The distance from the last location that every new location will be incremented in meters." },
            { "Max Jitter Percent", "Every new location will be offset by a random distance up to a maximum of the jitter percentage multiplied by the location increment." },
            { "Locations Per Spawn", "The number of locations that will be checked every Spawn interval. This does not guarantee that something will be spawned at that location, because lack of fitness may preclude that location from being used." },
            { "Max Cluster Size", "The maximum individuals in a cluster before a new cluster is started." },

            { "Distance Mask", "Mask fitness over distance. Left hand side of curve represents the centre of the spawner. Use this to alter spawn success away from centre e.g. peter out towards edges."},
            { "Area Mask", "Mask fitness over area. None - Don't apply image filter. Grey Scale - apply image filter using greys scale. R - Apply filter from red channel. G - Apply filter from green channel. B - Apply filter from blue channel. A - Apply filter from alpha channel. Terrain Texture Slot - apply mask from texture painted on terrain."},
            { "Image Mask", "The texure to use as the source of the area mask."},
            { "Smooth Mask", "Smooth the mask before applying it. This is a nice way to clean noise up in the mask, or to soften the edges of the mask."},
            { "Normalise Mask", "Normalise the mask before applying it. Ensures that the full dynamic range of the mask is used."},
            { "Invert Mask", "Invert the mask before applying it."},
            { "Flip Mask", "Flip the mask on its x and y axis mask before applying it. Useful sometimes to match the unity terrain as this is flipped internally."},
            { "Seed", "The unique seed for this spawner. If the environment, resources or rules dont change, then hitting Reset and respawning will always regenerate the same result." },

            { "Name", "Rule name - purely for convenience" },
            { "Resource Type", "The type of resource this rule will apply to." },
            { "Selected Resource", "The resource this rule applies to. To modify how the resource interprets terrain fitness change its spawn criteria." },
            { "Min Viable Fitness", "The minimum fitness needed to be considered viable to spawn." },
            { "Failure Rate", "The amount of the time that the rule will fail even if fit enough. 0 means never fail, and 1 means always fail. Use this to thin things out." },
            { "Max Instances", "The maximum number of resource instances this rule can spawn. Use this to stop over population." },
            { "Active", "Whether this rule is active or not. Use this to disable the rule."},
            { "Curr Inst Count", "The number of instances of this rule that have been spawned."},
            { "Instances Spawned", "The number of times this resource has been spawned." },
            { "Inactive Inst Count", "The number of inactive instances that have been spawned, but are now inactive and in the pool for re-use. Only relevant when game objects have been spawned" },
         
            { "Active Rules", "The number of active rules being managed by the spawner."},
            { "Inactive Rules", "The number of inactive rules being managed by the spawner."},
            { "TOTAL Rules", "The total number of rules being managed by the spawner."},
            { "MAX INSTANCES", "The maximum number of instances that can be managed by the spawner."},
            { "Active Instances", "The number of active instances being managed by the spawner."},
            { "Inactive Instances", "The number inactive instances being managed by the spawner."},
            { "TOTAL Instances", "The total number of active and inactive instances being managed by the spawner."},

            { "Ground Level", "Ground level for this feature, used to make positioning easier." },
            { "Show Ground Level", "Show ground level." },
            { "Stick To Ground", "Stick to ground level." },
            { "Show Gizmos", "Show the spawners gizmos." },
            { "Show Rulers", "Show rulers." },
            { "Show Statistics", "Show spawner statistics." },
            { "Flatten", "Flatten the entrie terrain - use with care!" },
            { "Smooth", "Smooth the entire terrain - removes jaggies and increases frame rate - run multiple times to increase effect - use with care!" },
            { "Clear Trees", "Clear trees from entire terrain - use with care!" },
            { "Clear Details", "Clear details / grass from entire terrain - use with care!" },
            { "Ground", "Position the spawner at ground level on the terrain." },
            { "Fit To Terrain", "Fits and aligns the spawner to the terrain." },
            { "Reset", "Resets the spawner, deletes any spawned game objects, and resets the random number generator." },
            { "Spawn", "Run a single spawn iteration. You can run as many spawn iterations as you like." },
        };
    }
}
