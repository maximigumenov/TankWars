using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gaia
{
    [System.Serializable]
    public class GaiaResource : ScriptableObject
    {
        [Tooltip("Resource name")]
        public string m_name = "Gaia Resource";
        [Tooltip("The absolute height of the sea or water table in meters. All spawn criteria heights are calculated relative to this. This can also be thought of as the water level.")]
        public float m_seaLevel = 100f;
        [Tooltip("The beach height in meters. Beaches are spawned at sea level and are extended for this height above sea level. This is used when creating default spawn rules in order to create a beach in the zone between water and land.")]
        public float m_beachHeight = 5f;
        [Tooltip("Terrain height.")]
        public float m_terrainHeight = 1000f;
        [Tooltip("Texture prototypes and fitness criteria.")]
        public ResourceProtoTexture[]       m_texturePrototypes = new ResourceProtoTexture[0];
        [Tooltip("Detail prototypes, dna and fitness criteria.")]
        public ResourceProtoDetail[]        m_detailPrototypes = new ResourceProtoDetail[0];
        [Tooltip("Tree prototypes, dna and fitness criteria.")]
        public ResourceProtoTree[]          m_treePrototypes = new ResourceProtoTree[0];
        [Tooltip("Game object prototypes, dna and fitness criteria.")]
        public ResourceProtoGameObject[]    m_gameObjectPrototypes = new ResourceProtoGameObject[0];
        //[Tooltip("Stamp prototypes, dna and fitness criteria.")]
        //public ResourceProtoStamp[]         m_stampPrototypes = new ResourceProtoStamp[0];

        /// <summary>
        /// Delete all the prototypes
        /// </summary>
        public void DeletePrototypes()
        {
            m_texturePrototypes = new ResourceProtoTexture[0];
            m_detailPrototypes = new ResourceProtoDetail[0];
            m_treePrototypes = new ResourceProtoTree[0];
            m_gameObjectPrototypes = new ResourceProtoGameObject[0];
            //m_stampPrototypes = new ResourceProtoStamp[0];
        }

        /// <summary>
        /// Pick up the prototypes being used in the terrain
        /// </summary>
        public void UpdatePrototypesFromTerrain()
        {
            Terrain terrain = Terrain.activeTerrain;
            if (terrain == null)
            {
                Debug.LogWarning("Can not update prototypes from the terrain as there is no terrain currently active in this scene.");
                return;
            }

            //Create some useful defaults
            m_terrainHeight = terrain.terrainData.size.y;

            //Alpha splats (textures)
            int idx;
            SpawnCritera criteria;
            SplatPrototype terrainTextureProto;
            ResourceProtoTexture resourceTextureProto;
            List<ResourceProtoTexture> resourceTexturePrototypes = new List<ResourceProtoTexture>(m_texturePrototypes);
            while (resourceTexturePrototypes.Count > terrain.terrainData.splatPrototypes.Length)
            {
                resourceTexturePrototypes.RemoveAt(resourceTexturePrototypes.Count - 1);
            }
            for (idx = 0; idx < terrain.terrainData.splatPrototypes.Length; idx++ )
            {
                terrainTextureProto = terrain.terrainData.splatPrototypes[idx];
                if (idx < resourceTexturePrototypes.Count)
                {
                    resourceTextureProto = resourceTexturePrototypes[idx];
                }
                else
                {
                    resourceTextureProto = new ResourceProtoTexture();
                    resourceTexturePrototypes.Add(resourceTextureProto);
                }
                resourceTextureProto.m_name = terrainTextureProto.texture.name;
                resourceTextureProto.m_texture = terrainTextureProto.texture;
                resourceTextureProto.m_normal = terrainTextureProto.normalMap;
                resourceTextureProto.m_offsetX = terrainTextureProto.tileOffset.x;
                resourceTextureProto.m_offsetY = terrainTextureProto.tileOffset.y;
                resourceTextureProto.m_sizeX = terrainTextureProto.tileSize.x;
                resourceTextureProto.m_sizeY = terrainTextureProto.tileSize.y;
                resourceTextureProto.m_metalic = terrainTextureProto.metallic;
                resourceTextureProto.m_smoothness = terrainTextureProto.smoothness;

                //Handle empty spawn criteria
                if (resourceTextureProto.m_spawnCriteria.Length == 0)
                {
                    resourceTextureProto.m_spawnCriteria = new SpawnCritera[1];
                    criteria = new SpawnCritera();
                    criteria.m_isActive = true;
                    criteria.m_virginTerrain = false;
                    criteria.m_checkType = Gaia.GaiaConstants.SpawnerLocationCheckType.PointCheck;

                    //Create some reasonable terrain based starting points
                    switch (idx)
                    {
                        case 0: //Base
                            {
                                criteria.m_checkHeight = true;
                                criteria.m_minHeight = m_seaLevel * -1f;
                                criteria.m_maxHeight = m_terrainHeight - m_seaLevel;
                                criteria.m_heightFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
                                criteria.m_checkSlope = false;
                                criteria.m_minSlope = 0f;
                                criteria.m_maxSlope = 90f;
                                criteria.m_slopeFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
                                criteria.m_checkProximity = false;
                                criteria.m_checkTexture = false;
                                break;
                            }

                        case 1: //Grass1
                            {
                                criteria.m_checkHeight = true;
                                criteria.m_minHeight = UnityEngine.Random.Range(m_beachHeight - (m_beachHeight / 4f), m_beachHeight * 2f);
                                criteria.m_maxHeight = m_terrainHeight - m_seaLevel;
                                criteria.m_heightFitness = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.03f, 1f), new Keyframe(1f, 1f));
                                criteria.m_checkSlope = false;
                                criteria.m_minSlope = 0f;
                                criteria.m_maxSlope = 90;
                                criteria.m_slopeFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
                                criteria.m_checkProximity = false;
                                criteria.m_checkTexture = false;
                                break;
                            }

                        case 2: //Grass2
                            {
                                criteria.m_checkHeight = true;
                                criteria.m_minHeight = UnityEngine.Random.Range(m_beachHeight - (m_beachHeight / 4f), m_beachHeight * 2f);
                                criteria.m_maxHeight = m_terrainHeight - m_seaLevel;
                                criteria.m_heightFitness = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.05f, 1f), new Keyframe(1f, 1f));
                                criteria.m_checkSlope = true;
                                criteria.m_minSlope = 0f;
                                criteria.m_maxSlope = 90f;
                                criteria.m_slopeFitness = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.2f, 1f), new Keyframe(1f, 1f));
                                criteria.m_checkProximity = false;
                                criteria.m_checkTexture = false;
                                break;
                            }

                        case 3: //Cliffs
                            {
                                criteria.m_checkHeight = false;
                                criteria.m_minHeight = m_seaLevel * -1f;
                                criteria.m_maxHeight = m_terrainHeight - m_seaLevel;
                                criteria.m_heightFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
                                criteria.m_checkSlope = true;
                                criteria.m_minSlope = 30f;
                                criteria.m_maxSlope = 90f;
                                criteria.m_slopeFitness = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.02f, 1f), new Keyframe(1f, 1f));
                                criteria.m_checkProximity = false;
                                criteria.m_checkTexture = false;
                                break;
                            }

                        default:
                            {
                                criteria.m_isActive = false;
                                criteria.m_checkHeight = false;
                                criteria.m_minHeight = UnityEngine.Random.Range(m_beachHeight - (m_beachHeight / 4f), m_beachHeight * 2f);
                                criteria.m_maxHeight = m_terrainHeight - m_seaLevel;
                                criteria.m_heightFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
                                criteria.m_checkSlope = false;
                                criteria.m_minSlope = 0f;
                                criteria.m_maxSlope = 90f;
                                criteria.m_slopeFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
                                criteria.m_checkProximity = false;
                                criteria.m_checkTexture = false;
                                break;
                            }
                    }
                    resourceTextureProto.m_spawnCriteria[0] = criteria;
                }
            }
            m_texturePrototypes = resourceTexturePrototypes.ToArray();

            //Detail prototypes
            idx = 0;
            DetailPrototype terrainDetailProto;
            ResourceProtoDetail resourceDetailProto;
            List<ResourceProtoDetail> resourceDetailPrototypes = new List<ResourceProtoDetail>(m_detailPrototypes);
            while (resourceDetailPrototypes.Count > terrain.terrainData.detailPrototypes.Length)
            {
                resourceDetailPrototypes.RemoveAt(resourceDetailPrototypes.Count - 1);
            }
            for (idx = 0; idx < terrain.terrainData.detailPrototypes.Length; idx++)
            {
                terrainDetailProto = terrain.terrainData.detailPrototypes[idx];
                if (idx < resourceDetailPrototypes.Count)
                {
                    resourceDetailProto = resourceDetailPrototypes[idx];
                }
                else
                {
                    resourceDetailProto = new ResourceProtoDetail();
                    resourceDetailPrototypes.Add(resourceDetailProto);
                }

                resourceDetailProto.m_renderMode = terrainDetailProto.renderMode;
                if (terrainDetailProto.renderMode == DetailRenderMode.VertexLit)
                {
                    resourceDetailProto.m_name = terrainDetailProto.prototype.name;
                    resourceDetailProto.m_detailProtoype = terrainDetailProto.prototype;
                }
                else
                {
                    resourceDetailProto.m_name = terrainDetailProto.prototypeTexture.name;
                    resourceDetailProto.m_detailTexture = terrainDetailProto.prototypeTexture;
                }

                resourceDetailProto.m_dryColour = terrainDetailProto.dryColor;
                resourceDetailProto.m_healthyColour = terrainDetailProto.healthyColor;
                resourceDetailProto.m_maxHeight = terrainDetailProto.maxHeight;
                resourceDetailProto.m_maxWidth = terrainDetailProto.maxWidth;
                resourceDetailProto.m_minHeight = terrainDetailProto.minHeight;
                resourceDetailProto.m_minWidth = terrainDetailProto.minWidth;
                resourceDetailProto.m_noiseSpread = terrainDetailProto.noiseSpread;
                resourceDetailProto.m_bendFactor = terrainDetailProto.bendFactor;

                //Handle missing dna
                if (resourceDetailProto.m_dna == null)
                {
                    resourceDetailProto.m_dna = new ResourceProtoDNA();
                }

                //Then reinitialise
                resourceDetailProto.m_dna.m_rndScaleInfluence = false;
                resourceDetailProto.m_dna.Update(idx, resourceDetailProto.m_maxWidth, resourceDetailProto.m_maxHeight, UnityEngine.Random.Range(0.05f, 0.2f), UnityEngine.Random.Range(0.3f, 1f));

                //Handle empty spawn criteria
                if (resourceDetailProto.m_spawnCriteria.Length == 0)
                {
                    resourceDetailProto.m_spawnCriteria = new SpawnCritera[1];
                    criteria = new SpawnCritera();
                    criteria.m_isActive = true;
                    criteria.m_virginTerrain = true;
                    criteria.m_checkType = Gaia.GaiaConstants.SpawnerLocationCheckType.PointCheck;

                    //Create some reasonable terrain based starting points
                    criteria.m_checkHeight = true;
                    criteria.m_minHeight = UnityEngine.Random.Range(m_beachHeight - (m_beachHeight / 4f), m_beachHeight * 2f);
                    criteria.m_maxHeight = m_terrainHeight - m_seaLevel;
                    criteria.m_heightFitness = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.05f, 1f), new Keyframe(1f, 0f));
                    criteria.m_checkSlope = true;
                    criteria.m_minSlope = UnityEngine.Random.Range(0f, 5f);
                    criteria.m_maxSlope = UnityEngine.Random.Range(35f, 55f);
                    criteria.m_slopeFitness = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.05f, 1f), new Keyframe(1f, 0f));
                    criteria.m_checkProximity = false;
                    criteria.m_checkTexture = false;
                    resourceDetailProto.m_spawnCriteria[0] = criteria;
                }

            }
            m_detailPrototypes = resourceDetailPrototypes.ToArray();

            //Tree prototypes
            idx = 0;
            TreePrototype terrainTreeProto;
            ResourceProtoTree resourceTreeProto;
            List<ResourceProtoTree> resourceTreePrototypes = new List<ResourceProtoTree>(m_treePrototypes);
            while (resourceTreePrototypes.Count > terrain.terrainData.treePrototypes.Length)
            {
                resourceTreePrototypes.RemoveAt(resourceTreePrototypes.Count - 1);
            }
            for (idx = 0; idx < terrain.terrainData.treePrototypes.Length; idx++)
            {
                terrainTreeProto = terrain.terrainData.treePrototypes[idx];
                if (idx < resourceTreePrototypes.Count)
                {
                    resourceTreeProto = resourceTreePrototypes[idx];
                }
                else
                {
                    resourceTreeProto = new ResourceProtoTree();
                    resourceTreePrototypes.Add(resourceTreeProto);
                }

                resourceTreeProto.m_name = terrainTreeProto.prefab.name;
                resourceTreeProto.m_desktopPrefab = resourceTreeProto.m_mobilePrefab = terrainTreeProto.prefab;
                resourceTreeProto.m_bendFactor = terrainTreeProto.bendFactor;

                //DNA
                if (resourceTreeProto.m_dna == null)
                {
                    resourceTreeProto.m_dna = new ResourceProtoDNA();
                    resourceTreeProto.m_dna.Update(idx);
                }
                UpdateDNA(terrainTreeProto.prefab, ref resourceTreeProto.m_dna);

                //Spawn criteria
                if (resourceTreeProto.m_spawnCriteria.Length == 0)
                {
                    resourceTreeProto.m_spawnCriteria = new SpawnCritera[1];
                    criteria = new SpawnCritera();
                    criteria.m_isActive = true;
                    criteria.m_virginTerrain = true;
                    criteria.m_checkType = GaiaConstants.SpawnerLocationCheckType.BoundedAreaCheck;

                    //Create some reasonable terrain based starting points
                    criteria.m_checkHeight = true;
                    criteria.m_minHeight = UnityEngine.Random.Range(m_beachHeight - (m_beachHeight / 4f), m_beachHeight * 2f);
                    criteria.m_maxHeight = m_terrainHeight - m_seaLevel;
                    criteria.m_heightFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
                    criteria.m_checkSlope = true;
                    criteria.m_minSlope = UnityEngine.Random.Range(0f, 5f);
                    criteria.m_maxSlope = UnityEngine.Random.Range(25f, 55f);
                    criteria.m_slopeFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
                    criteria.m_checkProximity = false;
                    criteria.m_checkTexture = false;
                    resourceTreeProto.m_spawnCriteria[0] = criteria;
                }
            }
            m_treePrototypes = resourceTreePrototypes.ToArray();
        }

        /// <summary>
        /// Update the DNA based on the physical size of the prefab
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="?"></param>
        void UpdateDNA(GameObject prefab, ref ResourceProtoDNA dna)
        {
            if (prefab != null)
            {
                GameObject go = Instantiate(prefab);
                Bounds bounds = new Bounds(go.transform.position, Vector3.zero);
                foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
                {
                    bounds.Encapsulate(r.bounds);
                }
                foreach (Collider c in go.GetComponentsInChildren<Collider>())
                {
                    bounds.Encapsulate(c.bounds);
                }
                DestroyImmediate(go);

                //Update dna
                dna.Update(dna.m_protoIdx, bounds.size.x, bounds.size.y);
            }
        }

        public void ChangeHeight(float oldHeight, float newHeight)
        {
            SpawnCritera[] criteria;
            SpawnCritera criterion;
            float oldMax = oldHeight - m_seaLevel;
            float newMax = newHeight - m_seaLevel;

            //Adjust textures
            for (int pidx = 0; pidx < m_texturePrototypes.Length; pidx++)
            {
                criteria = m_texturePrototypes[pidx].m_spawnCriteria;
                for (int cidx = 0; cidx < criteria.Length; cidx++)
                {
                    criterion = criteria[cidx];
                    if (criterion.m_maxHeight == oldMax)
                    {
                        criterion.m_maxHeight = newMax;
                    }
                }
            }

            //Adjust details
            for (int pidx = 0; pidx < m_detailPrototypes.Length; pidx++)
            {
                criteria = m_detailPrototypes[pidx].m_spawnCriteria;
                for (int cidx = 0; cidx < criteria.Length; cidx++)
                {
                    criterion = criteria[cidx];
                    if (criterion.m_maxHeight == oldMax)
                    {
                        criterion.m_maxHeight = newMax;
                    }
                }
            }

            //Adjust trees
            for (int pidx = 0; pidx < m_treePrototypes.Length; pidx++)
            {
                criteria = m_treePrototypes[pidx].m_spawnCriteria;
                for (int cidx = 0; cidx < criteria.Length; cidx++)
                {
                    criterion = criteria[cidx];
                    if (criterion.m_maxHeight == oldMax)
                    {
                        criterion.m_maxHeight = newMax;
                    }
                }
            }

            //Adjust gameobjects
            for (int pidx = 0; pidx < m_gameObjectPrototypes.Length; pidx++)
            {
                criteria = m_gameObjectPrototypes[pidx].m_spawnCriteria;
                for (int cidx = 0; cidx < criteria.Length; cidx++)
                {
                    criterion = criteria[cidx];
                    if (criterion.m_maxHeight == oldMax)
                    {
                        criterion.m_maxHeight = newMax;
                    }
                }
            }

            /*
            //Adjust stamps
            for (int pidx = 0; pidx < m_stampPrototypes.Length; pidx++)
            {
                criteria = m_stampPrototypes[pidx].m_spawnCriteria;
                for (int cidx = 0; cidx < criteria.Length; cidx++)
                {
                    criterion = criteria[cidx];
                    if (criterion.m_maxHeight == oldMax)
                    {
                        criterion.m_maxHeight = newMax;
                    }
                }
            }
             */
        }

        /// <summary>
        /// Update the sea level - and auto update any terrain criteria - only focus on extremities tho
        /// </summary>
        /// <param name="newSeaLevel">New sea level</param>
        public void ChangeSeaLevel(float oldSeaLevel, float newSeaLevel)
        {
            SpawnCritera[] criteria;
            SpawnCritera criterion;
            float oldMin = oldSeaLevel * -1f;
            float newMin = newSeaLevel * -1f;
            float oldMax = m_terrainHeight - oldSeaLevel;
            float newMax = m_terrainHeight - newSeaLevel;

            //Adjust textures
            for (int pidx = 0; pidx < m_texturePrototypes.Length; pidx++)
            {
                criteria = m_texturePrototypes[pidx].m_spawnCriteria;
                for (int cidx = 0; cidx < criteria.Length; cidx++)
                {
                    criterion = criteria[cidx];
                    if (criterion.m_minHeight == oldMin)
                    {
                        criterion.m_minHeight = newMin;
                    }
                    if (criterion.m_maxHeight == oldMax)
                    {
                        criterion.m_maxHeight = newMax;
                    }
                }
            }

            //Adjust details
            for (int pidx = 0; pidx < m_detailPrototypes.Length; pidx++)
            {
                criteria = m_detailPrototypes[pidx].m_spawnCriteria;
                for (int cidx = 0; cidx < criteria.Length; cidx++)
                {
                    criterion = criteria[cidx];
                    if (criterion.m_minHeight == oldMin)
                    {
                        criterion.m_minHeight = newMin;
                    }
                    if (criterion.m_maxHeight == oldMax)
                    {
                        criterion.m_maxHeight = newMax;
                    }
                }
            }

            //Adjust trees
            for (int pidx = 0; pidx < m_treePrototypes.Length; pidx++)
            {
                criteria = m_treePrototypes[pidx].m_spawnCriteria;
                for (int cidx = 0; cidx < criteria.Length; cidx++)
                {
                    criterion = criteria[cidx];
                    if (criterion.m_minHeight == oldMin)
                    {
                        criterion.m_minHeight = newMin;
                    }
                    if (criterion.m_maxHeight == oldMax)
                    {
                        criterion.m_maxHeight = newMax;
                    }
                }
            }

            //Adjust gameobjects
            for (int pidx = 0; pidx < m_gameObjectPrototypes.Length; pidx++)
            {
                criteria = m_gameObjectPrototypes[pidx].m_spawnCriteria;
                for (int cidx = 0; cidx < criteria.Length; cidx++)
                {
                    criterion = criteria[cidx];
                    if (criterion.m_minHeight == oldMin)
                    {
                        criterion.m_minHeight = newMin;
                    }
                    if (criterion.m_maxHeight == oldMax)
                    {
                        criterion.m_maxHeight = newMax;
                    }
                }
            }

            /*
            //Adjust stamps
            for (int pidx = 0; pidx < m_stampPrototypes.Length; pidx++)
            {
                criteria = m_stampPrototypes[pidx].m_spawnCriteria;
                for (int cidx = 0; cidx < criteria.Length; cidx++)
                {
                    criterion = criteria[cidx];
                    if (criterion.m_minHeight == oldMin)
                    {
                        criterion.m_minHeight = newMin;
                    }
                    if (criterion.m_maxHeight == oldMax)
                    {
                        criterion.m_maxHeight = newMax;
                    }
                }
            }
             */

            //Update to new sea level
            m_seaLevel = newSeaLevel;
        }


        /// <summary>
        /// Set these assets into all terrains
        /// </summary>
        public void ApplyPrototypesToTerrain()
        {
            foreach (Terrain t in Terrain.activeTerrains)
            {
                ApplyPrototypesToTerrain(t);
            }
        }

        /// <summary>
        /// Set these assets into the terrain provided
        /// </summary>
        /// <param name="terrain"></param>
        public void ApplyPrototypesToTerrain(Terrain terrain)
        {
            // Do a terrain check
            if (terrain == null)
            {
                Debug.LogWarning("Can not apply assets to terrain no terrain has been supplied.");
                return;
            }

            //Alpha splats
            SplatPrototype newSplat;
            List<SplatPrototype> terrainSplats = new List<SplatPrototype>();
            foreach (ResourceProtoTexture splat in m_texturePrototypes)
            {
                newSplat = new SplatPrototype();
                newSplat.normalMap = splat.m_normal;
                newSplat.tileOffset = new Vector2(splat.m_offsetX, splat.m_offsetY);
                newSplat.tileSize = new Vector2(splat.m_sizeX, splat.m_sizeY);
                newSplat.texture = splat.m_texture;
                terrainSplats.Add(newSplat);
            }
            terrain.terrainData.splatPrototypes = terrainSplats.ToArray();

            //Detail prototypes
            DetailPrototype newDetail;
            List<DetailPrototype> terrainDetails = new List<DetailPrototype>();
            foreach (ResourceProtoDetail detail in m_detailPrototypes)
            {
                newDetail = new DetailPrototype();
                newDetail.renderMode = detail.m_renderMode;
                if (detail.m_detailProtoype != null)
                {
                    newDetail.usePrototypeMesh = true;
                    newDetail.prototype = detail.m_detailProtoype;
                }
                else
                {
                    newDetail.usePrototypeMesh = false;
                    newDetail.prototypeTexture = detail.m_detailTexture;
                }
                newDetail.dryColor = detail.m_dryColour;
                newDetail.healthyColor = detail.m_healthyColour;
                newDetail.maxHeight = detail.m_maxHeight;
                newDetail.maxWidth = detail.m_maxWidth;
                newDetail.minHeight = detail.m_minHeight;
                newDetail.minWidth = detail.m_minWidth;
                newDetail.noiseSpread = detail.m_noiseSpread;
                newDetail.bendFactor = detail.m_bendFactor;
                terrainDetails.Add(newDetail);
            }
            terrain.terrainData.detailPrototypes = terrainDetails.ToArray();

            //Tree prototypes
            TreePrototype newTree;
            List<TreePrototype> terrainTrees = new List<TreePrototype>();
            foreach (ResourceProtoTree tree in m_treePrototypes)
            {
                newTree = new TreePrototype();
                newTree.bendFactor = tree.m_bendFactor;
                newTree.prefab = tree.m_desktopPrefab;
                terrainTrees.Add(newTree);
            }
            terrain.terrainData.treePrototypes = terrainTrees.ToArray();

            terrain.Flush();
        }

        /*
        public void UpdateDNA()
        {
            //Bounds
            Bounds bounds;

            //Details
            ResourceProtoDetail detailProto;
            for (int detailIdx = 0; detailIdx < m_detailPrototypes.Length; detailIdx++)
            {
                detailProto = m_detailPrototypes[detailIdx];
                if (detailProto.m_dna == null)
                {
                    detailProto.m_dna = new ResourceProtoDNA();
                    detailProto.m_dna.Initialise();
                }
                detailProto.m_dna.m_width = detailProto.m_maxWidth;
                detailProto.m_dna.m_height = detailProto.m_maxHeight;
                detailProto.m_dna.m_boundsRadius = detailProto.m_dna.m_width;
            }

            ResourceProtoTree treeProto;
            for (int treeIdx = 0; treeIdx < m_treePrototypes.Length; treeIdx++)
            {
                treeProto = m_treePrototypes[treeIdx];
                if (treeProto.m_dna == null)
                {
                    treeProto.m_dna = new ResourceProtoDNA();
                    treeProto.m_dna.Initialise();
                }
                if (treeProto.m_desktopPrefab != null)
                {
                    GameObject go = Instantiate(treeProto.m_desktopPrefab);
                    bounds = new Bounds(go.transform.position, Vector3.zero);
                    foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
                    {
                        bounds.Encapsulate(r.bounds);
                    }
                    foreach (Collider c in go.GetComponentsInChildren<Collider>())
                    {
                        bounds.Encapsulate(c.bounds);
                    }
                    treeProto.m_dna.m_width = bounds.size.x;
                    treeProto.m_dna.m_height = bounds.size.y;
                    treeProto.m_dna.m_boundsRadius = treeProto.m_dna.m_width;
                    DestroyImmediate(go);
                }
            }

            ResourceProtoGameObject gameProto;
            for (int goIdx = 0; goIdx < m_gameObjectPrototypes.Length; goIdx++)
            {
                gameProto = m_gameObjectPrototypes[goIdx];
                if (gameProto.m_dna == null)
                {
                    gameProto.m_dna = new ResourceProtoDNA();
                    gameProto.m_dna.Initialise();
                }
                if (gameProto.m_instances.Length > 0 && gameProto.m_instances[0].m_desktopPrefab != null)
                {
                    GameObject go = Instantiate(gameProto.m_instances[0].m_desktopPrefab);
                    bounds = new Bounds(go.transform.position, Vector3.zero);
                    foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
                    {
                        bounds.Encapsulate(r.bounds);
                    }
                    foreach (Collider c in go.GetComponentsInChildren<Collider>())
                    {
                        bounds.Encapsulate(c.bounds);
                    }
                    gameProto.m_dna.m_width = bounds.size.x;
                    gameProto.m_dna.m_height = bounds.size.y;
                    gameProto.m_dna.m_boundsRadius = gameProto.m_dna.m_width;
                    DestroyImmediate(go);
                }
            }
        }
        */

        #region Cache Helpers

        /// <summary>
        /// Return true if any of these resources do texture based lookups
        /// </summary>
        /// <returns></returns>
        public bool ChecksTextures()
        {
            int idx;
            for (idx = 0; idx < m_texturePrototypes.Length; idx++)
            {
                if (m_texturePrototypes[idx].ChecksTextures())
                {
                    return true;
                }
            }
            for (idx = 0; idx < m_detailPrototypes.Length; idx++)
            {
                if (m_detailPrototypes[idx].ChecksTextures())
                {
                    return true;
                }
            }
            for (idx = 0; idx < m_treePrototypes.Length; idx++)
            {
                if (m_treePrototypes[idx].ChecksTextures())
                {
                    return true;
                }
            }
            for (idx = 0; idx < m_gameObjectPrototypes.Length; idx++)
            {
                if (m_gameObjectPrototypes[idx].ChecksTextures())
                {
                    return true;
                }
            }
            /*
            for (idx = 0; idx < m_stampPrototypes.Length; idx++)
            {
                if (m_stampPrototypes[idx].ChecksTextures())
                {
                    return true;
                }
            }
             */
            return false;
        }

        /// <summary>
        /// Return true if any of these resources do proximity based lookups
        /// </summary>
        /// <returns></returns>
        public bool ChecksProximity()
        {
            int idx;
            for (idx = 0; idx < m_texturePrototypes.Length; idx++)
            {
                if (m_texturePrototypes[idx].ChecksProximity())
                {
                    return true;
                }
            }
            for (idx = 0; idx < m_detailPrototypes.Length; idx++)
            {
                if (m_detailPrototypes[idx].ChecksProximity())
                {
                    return true;
                }
            }
            for (idx = 0; idx < m_treePrototypes.Length; idx++)
            {
                if (m_treePrototypes[idx].ChecksProximity())
                {
                    return true;
                }
            }
            for (idx = 0; idx < m_gameObjectPrototypes.Length; idx++)
            {
                if (m_gameObjectPrototypes[idx].ChecksProximity())
                {
                    return true;
                }
            }
            /*
            for (idx = 0; idx < m_stampPrototypes.Length; idx++)
            {
                if (m_stampPrototypes[idx].ChecksProximity())
                {
                    return true;
                }
            }
             */
            return false;
        }

        #endregion

        #region Add Resources

        /// <summary>
        /// Add a new game object resource, and make some assumptions based on current terrain settings
        /// </summary>
        /// <param name="prefab"></param>
        public void AddGameObject(GameObject prefab)
        {
            if (prefab == null)
            {
                Debug.LogWarning("Can't add null game object");
            }

            #if UNITY_EDITOR
            if (PrefabUtility.GetPrefabType(prefab) != PrefabType.None)
            {
                //Create space for larger array
                ResourceProtoGameObject[] pgos = new ResourceProtoGameObject[m_gameObjectPrototypes.Length + 1];

                //Copy existing items across
                for (int idx = 0; idx < m_gameObjectPrototypes.Length; idx++)
                {
                    pgos[idx] = m_gameObjectPrototypes[idx];
                }

                //Create the new game object prototype
                ResourceProtoGameObject pgo = new ResourceProtoGameObject();
                pgo.m_name = prefab.name;

                //Create and store prefab in instances
                ResourceProtoGameObjectInstance pgi = new ResourceProtoGameObjectInstance();
                pgi.m_conformToSlope = true;
                pgi.m_desktopPrefab = prefab;
                pgi.m_name = prefab.name;
                pgi.m_rndRotateX = false;
                pgi.m_rndRotateY = true;
                pgi.m_rndRotateZ = false;
                pgi.m_terrainOffset = -0.1f;
                pgo.m_instances = new ResourceProtoGameObjectInstance[1];
                pgo.m_instances[0] = pgi;

                //Update dna
                pgo.m_dna.Update(m_gameObjectPrototypes.Length);
                UpdateDNA(prefab, ref pgo.m_dna);

                //Create spawn criteria
                pgo.m_spawnCriteria = new SpawnCritera[1];
                SpawnCritera criteria = new SpawnCritera();
                criteria.m_isActive = true;
                criteria.m_virginTerrain = true;
                criteria.m_checkType = GaiaConstants.SpawnerLocationCheckType.BoundedAreaCheck;
                //Create some reasonable terrain based starting points
                criteria.m_checkHeight = true;
                criteria.m_minHeight = UnityEngine.Random.Range(m_beachHeight - (m_beachHeight / 4f), m_beachHeight * 2f);
                criteria.m_maxHeight = m_terrainHeight - m_seaLevel;
                criteria.m_heightFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
                criteria.m_checkSlope = true;
                criteria.m_minSlope = UnityEngine.Random.Range(0f, 1.5f);
                criteria.m_maxSlope = UnityEngine.Random.Range(1.6f, 5f);
                criteria.m_slopeFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
                criteria.m_checkProximity = false;
                criteria.m_checkTexture = false;
                pgo.m_spawnCriteria[0] = criteria;

                pgos[pgos.Length - 1] = pgo;
                m_gameObjectPrototypes = pgos;
            }
            #endif
        }

        #endregion

        #region Create Spawners

        public GameObject CreateTextureSpawner(float range)
        {
            GameObject gaiaObj = GameObject.Find("Gaia");
            if (gaiaObj == null)
            {
                gaiaObj = new GameObject("Gaia");
            }
            GameObject spawnerObj = new GameObject("Texture Spawner");
            spawnerObj.AddComponent<Spawner>();
            Spawner spawner = spawnerObj.GetComponent<Spawner>();
            spawner.m_resources = this;
            spawnerObj.transform.parent = gaiaObj.transform;

            //Do basic setup
            spawner.m_resources = this;
            spawner.m_mode = GaiaConstants.OperationMode.DesignTime;
            spawner.m_spawnerShape = GaiaConstants.SpawnerShape.Box;
            spawner.m_rndGenerator = new System.Random(spawner.m_seed);
            spawner.m_spawnRange = range;
            spawner.m_spawnFitnessAttenuator = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
            spawner.m_spawnRuleSelector = GaiaConstants.SpawnerRuleSelector.All;
            spawner.m_spawnLocationAlgorithm = GaiaConstants.SpawnerLocation.EveryLocation;
            spawner.m_spawnCollisionLayers = 1 << LayerMask.NameToLayer("Default");
            spawner.m_locationIncrement = 1f;

            //If we have a terrain, then lets look at it, we may want to change the increment
            if (Terrain.activeTerrain != null)
            {
                spawner.m_locationIncrement =
                    Mathf.Clamp(Mathf.Min(Terrain.activeTerrain.terrainData.size.x, Terrain.activeTerrain.terrainData.size.z) / (float)Terrain.activeTerrain.terrainData.alphamapWidth, 0.05f, 1f);
            }

            //Iterate thru all the textures and add them. Assume the first one is the base.
            SpawnRule rule;
            for (int resIdx = 0; resIdx < m_texturePrototypes.Length; resIdx++ )
            {
                rule = new SpawnRule();
                rule.m_name = m_texturePrototypes[resIdx].m_name;
                rule.m_resourceType = GaiaConstants.SpawnerResourceType.TerrainTexture;
                rule.m_resourceIdx = resIdx;
                rule.m_minViableFitness = 0f;
                rule.m_failureRate = 0f;
                rule.m_maxInstances = (int)((range * 2f) * (range * 2f));
                rule.m_isActive = true;
                rule.m_isFoldedOut = false;
                rule.m_useExtendedFitness = false;
                rule.m_useExtendedSpawn = false;
                spawner.m_activeRuleCnt++;
                spawner.m_spawnerRules.Add(rule);
            }
            return spawnerObj;
        }

        public GameObject CreateDetailSpawner(float range)
        {
            GameObject gaiaObj = GameObject.Find("Gaia");
            if (gaiaObj == null)
            {
                gaiaObj = new GameObject("Gaia");
            }
            GameObject spawnerObj = new GameObject("Detail Spawner");
            spawnerObj.AddComponent<Spawner>();
            Spawner spawner = spawnerObj.GetComponent<Spawner>();
            spawner.m_resources = this;
            spawnerObj.transform.parent = gaiaObj.transform;

            //Do basic setup
            spawner.m_resources = this;
            spawner.m_mode = GaiaConstants.OperationMode.DesignTime;
            spawner.m_spawnerShape = GaiaConstants.SpawnerShape.Box;
            spawner.m_rndGenerator = new System.Random(spawner.m_seed);
            spawner.m_spawnRange = range;
            spawner.m_spawnFitnessAttenuator = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
            spawner.m_spawnRuleSelector = GaiaConstants.SpawnerRuleSelector.All;
            spawner.m_spawnLocationAlgorithm = GaiaConstants.SpawnerLocation.EveryLocationJittered;
            spawner.m_spawnCollisionLayers = 1 << LayerMask.NameToLayer("Default");
            spawner.m_locationIncrement = 1.5f;

            //Iterate thru all the details and add them.
            SpawnRule rule;
            for (int resIdx = 0; resIdx < m_detailPrototypes.Length; resIdx++)
            {
                rule = new SpawnRule();
                rule.m_name = m_detailPrototypes[resIdx].m_name;
                rule.m_resourceType = GaiaConstants.SpawnerResourceType.TerrainDetail;
                rule.m_resourceIdx = resIdx;
                rule.m_minViableFitness = Random.Range(0.2f, 0.5f);
                rule.m_failureRate = Random.Range(0.7f, 0.95f);
                rule.m_maxInstances = (int)((range * 2f) * (range * 2f));
                rule.m_isActive = true;
                rule.m_isFoldedOut = false;
                rule.m_useExtendedFitness = false;
                rule.m_useExtendedSpawn = false;
                spawner.m_activeRuleCnt++;
                spawner.m_spawnerRules.Add(rule);
            }

            return spawnerObj;
        }

        public GameObject CreateTreeSpawner(float range)
        {
            GameObject gaiaObj = GameObject.Find("Gaia");
            if (gaiaObj == null)
            {
                gaiaObj = new GameObject("Gaia");
            }
            GameObject spawnerObj = new GameObject("Tree Spawner");
            spawnerObj.AddComponent<Spawner>();
            Spawner spawner = spawnerObj.GetComponent<Spawner>();
            spawner.m_resources = this;
            spawnerObj.transform.parent = gaiaObj.transform;

            //Do basic setup
            spawner.m_resources = this;
            spawner.m_mode = GaiaConstants.OperationMode.DesignTime;
            spawner.m_spawnerShape = GaiaConstants.SpawnerShape.Box;
            spawner.m_rndGenerator = new System.Random(spawner.m_seed);
            spawner.m_spawnRange = range;
            spawner.m_spawnFitnessAttenuator = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
            spawner.m_spawnRuleSelector = GaiaConstants.SpawnerRuleSelector.WeightedFittest;
            spawner.m_spawnLocationAlgorithm = GaiaConstants.SpawnerLocation.RandomLocationClustered;
            spawner.m_spawnCollisionLayers = 1 << LayerMask.NameToLayer("Default");
            spawner.m_locationChecksPerInt = (int)range * 2;
            spawner.m_maxRandomClusterSize = 30;

            //Iterate thru all the trees and add them.
            SpawnRule rule;
            for (int resIdx = 0; resIdx < m_treePrototypes.Length; resIdx++)
            {
                rule = new SpawnRule();
                rule.m_name = m_treePrototypes[resIdx].m_name;
                rule.m_resourceType = GaiaConstants.SpawnerResourceType.TerrainTree;
                rule.m_resourceIdx = resIdx;
                rule.m_minViableFitness = 0.25f;
                rule.m_failureRate = 0f;
                rule.m_maxInstances = (int)((range * range) / 5f);
                rule.m_isActive = true;
                rule.m_isFoldedOut = false;
                rule.m_useExtendedFitness = false;
                rule.m_useExtendedSpawn = false;
                spawner.m_activeRuleCnt++;
                spawner.m_spawnerRules.Add(rule);
            }

            return spawnerObj;
        }

        public GameObject CreateGameObjectSpawner(float range)
        {
            GameObject gaiaObj = GameObject.Find("Gaia");
            if (gaiaObj == null)
            {
                gaiaObj = new GameObject("Gaia");
            }
            GameObject spawnerObj = new GameObject("GameObject Spawner");
            spawnerObj.AddComponent<Spawner>();
            Spawner spawner = spawnerObj.GetComponent<Spawner>();
            spawner.m_resources = this;
            spawnerObj.transform.parent = gaiaObj.transform;

            //Do basic setup
            spawner.m_resources = this;
            spawner.m_mode = GaiaConstants.OperationMode.DesignTime;
            spawner.m_spawnerShape = GaiaConstants.SpawnerShape.Box;
            spawner.m_rndGenerator = new System.Random(spawner.m_seed);
            spawner.m_spawnRange = range;
            spawner.m_spawnFitnessAttenuator = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
            spawner.m_spawnRuleSelector = GaiaConstants.SpawnerRuleSelector.Random;
            spawner.m_spawnLocationAlgorithm = GaiaConstants.SpawnerLocation.RandomLocationClustered;
            spawner.m_spawnCollisionLayers = 1 << LayerMask.NameToLayer("Default"); 
            spawner.m_locationChecksPerInt = 2000;
            spawner.m_maxRandomClusterSize = 20;

            //Iterate thru all the trees and add them.
            SpawnRule rule;
            for (int resIdx = 0; resIdx < m_gameObjectPrototypes.Length; resIdx++)
            {
                rule = new SpawnRule();
                rule.m_name = m_gameObjectPrototypes[resIdx].m_name;
                rule.m_resourceType = GaiaConstants.SpawnerResourceType.GameObject;
                rule.m_resourceIdx = resIdx;
                rule.m_minViableFitness = 0.25f;
                rule.m_failureRate = 0f;
                rule.m_maxInstances = (int)range * 2;
                rule.m_isActive = true;
                rule.m_isFoldedOut = false;
                rule.m_useExtendedFitness = false;
                rule.m_useExtendedSpawn = false;
                spawner.m_activeRuleCnt++;
                spawner.m_spawnerRules.Add(rule);
            }

            return spawnerObj;
        }

        /*
        public GameObject CreateStampSpawner(float range)
        {
            GameObject gaiaObj = GameObject.Find("Gaia");
            if (gaiaObj == null)
            {
                gaiaObj = new GameObject("Gaia");
            }
            GameObject spawnerObj = new GameObject("Stamp Spawner");
            spawnerObj.AddComponent<Spawner>();
            Spawner spawner = spawnerObj.GetComponent<Spawner>();
            spawner.m_resources = this;
            spawnerObj.transform.parent = gaiaObj.transform;

            //Do basic setup
            spawner.m_resources = this;
            spawner.m_mode = Constants.OperationMode.DesignTime;
            spawner.m_spawnerShape = Constants.SpawnerShape.Box;
            spawner.m_rndGenerator = new System.Random(spawner.m_seed);
            spawner.m_spawnRange = range;
            spawner.m_spawnRangeAttenuator = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
            spawner.m_spawnRuleSelector = Constants.SpawnerRuleSelector.Random;
            spawner.m_spawnLocationAlgorithm = Constants.SpawnerLocation.RandomLocation;
            spawner.m_spawnerLayerMask = 1 << LayerMask.NameToLayer("Default");
            spawner.m_locationChecksPerInt = 5;
            spawner.m_maxRandomClusterSize = 1;

            //Iterate thru all the stamps and add them.
            SpawnRule rule;
            for (int resIdx = 0; resIdx < m_stampPrototypes.Length; resIdx++)
            {
                rule = new SpawnRule();
                rule.m_name = m_stampPrototypes[resIdx].m_name;
                rule.m_resourceType = Constants.SpawnerResourceType.Stamp;
                rule.m_resourceIdx = resIdx;
                rule.m_minViableFitness = 0.25f;
                rule.m_failureRate = 0f;
                rule.m_maxInstances = (int)range / 5;
                rule.m_isActive = true;
                rule.m_isFoldedOut = false;
                rule.m_useExtendedFitness = false;
                rule.m_useExtendedSpawn = false;
                spawner.m_activeRuleCnt++;
                spawner.m_spawnerRules.Add(rule);
            }

            return spawnerObj;
        }
        */

        public GameObject CreateSpawnerGroup()
        {
            GameObject gaiaObj = GameObject.Find("Gaia");
            if (gaiaObj == null)
            {
                gaiaObj = new GameObject("Gaia");
            }
            GameObject spawnerObj = new GameObject("Group Spawner");
            spawnerObj.AddComponent<SpawnerGroup>();
            spawnerObj.transform.parent = gaiaObj.transform;
            return spawnerObj;
        }

        #endregion 

        #region Exporters

        /// <summary>
        /// This routine will export a texture from the current terrain - experimental only at moment and not supported.
        /// </summary>
        public void ExportTexture()
        {
            //Look here for some asset database stuff on read write
            //https://github.com/Pixelstudio/TerrainPaint/blob/master/Assets/Editor/TerrainPaint/TerrainPaint.cs


            Terrain terrain;
            Texture2D exportTexture;
            Color pixel;
            int width, height, layers;
            float aR, aG, aB, aA;

            //Now iterate through the terrains and export them
            for (int terrIdx = 0; terrIdx < Terrain.activeTerrains.Length; terrIdx++)
            {
                terrain = Terrain.activeTerrains[terrIdx];
                width = terrain.terrainData.alphamapWidth;
                height = terrain.terrainData.alphamapHeight;
                layers = terrain.terrainData.alphamapLayers;
                exportTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
                float[, ,] splatMaps = terrain.terrainData.GetAlphamaps(0, 0, width, height);

                //Iterate thru the terrain
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < height; z++)
                    {
                        aR = aG = aB = aA = 0f;
                        for (int t = 0; t < layers; t++)
                        {
                            pixel = m_texturePrototypes[t].m_texture.GetPixel(
                                x % ((int)m_texturePrototypes[t].m_sizeX / m_texturePrototypes[t].m_texture.width),
                                z % ((int)m_texturePrototypes[t].m_sizeY / m_texturePrototypes[t].m_texture.height)
                                );
                            aR += splatMaps[x, z, t] * pixel.r;
                            aG += splatMaps[x, z, t] * pixel.g;
                            aB += splatMaps[x, z, t] * pixel.b;
                            aA += splatMaps[x, z, t] * pixel.a;
                        }
                        exportTexture.SetPixel(x, z, new Color(aR, aG, aB, aA));
                    }
                }

                //Now export / save the texture
                Gaia.Utils.ExportPNG(terrain.name + " - Export", exportTexture);

                //And destroy it
                DestroyImmediate(exportTexture);
            }
            Debug.LogError("Attempted to export textures on terrain that does not exist!");
        }

        #endregion  
    }
}