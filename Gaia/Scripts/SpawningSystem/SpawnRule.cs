using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Gaia
{
    [Serializable]
    public class SpawnRule
    {
        public string m_name;
        public bool m_useExtendedFitness = false;
        public IFitness m_extendedFitness = null;
        public bool m_useExtendedSpawn = false;
        public ISpawn m_extendedSpawn = null;
        public float m_minViableFitness = 0.25f;
        public float m_failureRate = 0f;
        public int m_maxInstances = 4000000;
        public Gaia.GaiaConstants.SpawnerResourceType m_resourceType;
        public int m_resourceIdx;
        public bool m_isActive = true;
        public bool m_isFoldedOut = false;
        public int m_currInstanceCnt = 0;
        public int m_activeInstanceCnt = 0;
        public int m_inactiveInstanceCnt = 0;


        /// <summary>
        /// Initilise the rule
        /// </summary>
        /// <param name="spawner"></param>
        public void Initialise(Spawner spawner)
        {
            foreach (ResourceProtoTexture texture in spawner.m_resources.m_texturePrototypes)
            {
                texture.Initialise(spawner);
            }
            foreach (ResourceProtoDetail detail in spawner.m_resources.m_detailPrototypes)
            {
                detail.Initialise(spawner);
            }
            foreach (ResourceProtoTree tree in spawner.m_resources.m_treePrototypes)
            {
                tree.Initialise(spawner);
            }
            foreach (ResourceProtoGameObject go in spawner.m_resources.m_gameObjectPrototypes)
            {
                go.Initialise(spawner);
            }
            /*
            foreach (ResourceProtoStamp stamp in spawner.m_resources.m_stampPrototypes)
            {
                stamp.Initialise(spawner);
            }
             */
        }
        
        /// <summary>
        /// Return the value of the fittest object in the spawn criteria
        /// </summary>
        /// <param name="spawner">The spawner we belong to</param>
        /// <param name="location">The location we are checking</param>
        /// <param name="slope"></param>
        /// <returns>Fitness in range 0..1f</returns>
        public float GetFitness(ref SpawnInfo spawnInfo)
        {
            //Check to see if we are active
            if (!m_isActive)
            {
                return 0f;
            }

            //Get the filters
            SpawnCritera[] filters = null;
            switch (m_resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    {
                        filters = spawnInfo.m_spawner.m_resources.m_detailPrototypes[m_resourceIdx].m_spawnCriteria;
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    {
                        filters = spawnInfo.m_spawner.m_resources.m_texturePrototypes[m_resourceIdx].m_spawnCriteria;
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    {
                        filters = spawnInfo.m_spawner.m_resources.m_treePrototypes[m_resourceIdx].m_spawnCriteria;
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.GameObject:
                    {
                        filters = spawnInfo.m_spawner.m_resources.m_gameObjectPrototypes[m_resourceIdx].m_spawnCriteria;
                        break;
                    }
                    /*
                default:
                    {
                        filters = spawnInfo.m_spawner.m_resources.m_stampPrototypes[m_resourceIdx].m_spawnCriteria;
                        break;
                    }
                     */
            }

            //Drop out if we have no filters
            if (filters == null || filters.Length == 0)
            {
                return 0f;
            }

            //Now calculate fitness
            float maxFitness = float.MinValue;
            int filterIdx;
            SpawnCritera filter;
            float fitness = 0f;

            for (filterIdx = 0; filterIdx < filters.Length; filterIdx++)
            {
                filter = filters[filterIdx];
                //Check to see of this filter needs a bounds check
                if (filter.m_checkType == GaiaConstants.SpawnerLocationCheckType.BoundedAreaCheck)
                {
                    if (!spawnInfo.m_spawner.CheckLocationBounds(ref spawnInfo, GetMaxScaledRadius(ref spawnInfo)))
                    {
                        return 0f;
                    }
                }
                //Now calculate and process fitness
                fitness = filter.GetFitness(ref spawnInfo);
                if (fitness > maxFitness)
                {
                    maxFitness = fitness;
                    if (maxFitness >= 1f)
                    {
                        return maxFitness;
                    }
                }
            }
            
            if (maxFitness == float.MinValue)
            {
                return 0f;
            }
            else
            {
                return maxFitness;
            }
        }


        /// <summary>
        /// Return the radius of the thing referred to by the rule
        /// </summary>
        /// <param name="spawnInfo">Spawner information</param>
        /// <returns>Radius</returns>
        public float GetRadius(ref SpawnInfo spawnInfo)
        {
            switch (m_resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    {
                        return 1f; //Makes no sense, so return arbitrary value
                    }
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    {
                        return spawnInfo.m_spawner.m_resources.m_detailPrototypes[m_resourceIdx].m_dna.m_boundsRadius;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    {
                        return spawnInfo.m_spawner.m_resources.m_treePrototypes[m_resourceIdx].m_dna.m_boundsRadius;
                    }
                case GaiaConstants.SpawnerResourceType.GameObject:
                    {
                        return spawnInfo.m_spawner.m_resources.m_gameObjectPrototypes[m_resourceIdx].m_dna.m_boundsRadius;
                    }
                    /*
                default:
                    {
                        return spawnInfo.m_spawner.m_resources.m_stampPrototypes[m_resourceIdx].m_dna.m_boundsRadius;
                    }
                     */
            }
            return 0f;
        }

        /// <summary>
        /// Return the maximum scaled radius of the thing referred to by the rule
        /// </summary>
        /// <param name="spawnInfo">Spawner information</param>
        /// <returns>Maximum scaled radius</returns>
        public float GetMaxScaledRadius(ref SpawnInfo spawnInfo)
        {
            switch (m_resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    {
                        return 1f; //Makes no sense, so return arbitrary value
                    }
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    {
                        return spawnInfo.m_spawner.m_resources.m_detailPrototypes[m_resourceIdx].m_dna.m_boundsRadius * spawnInfo.m_spawner.m_resources.m_detailPrototypes[m_resourceIdx].m_dna.m_maxScale;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    {
                        return spawnInfo.m_spawner.m_resources.m_treePrototypes[m_resourceIdx].m_dna.m_boundsRadius * spawnInfo.m_spawner.m_resources.m_treePrototypes[m_resourceIdx].m_dna.m_maxScale;
                    }
                case GaiaConstants.SpawnerResourceType.GameObject:
                    {
                        return spawnInfo.m_spawner.m_resources.m_gameObjectPrototypes[m_resourceIdx].m_dna.m_boundsRadius * spawnInfo.m_spawner.m_resources.m_gameObjectPrototypes[m_resourceIdx].m_dna.m_maxScale;
                    }
                    /*
                default:
                    {
                        return spawnInfo.m_spawner.m_resources.m_stampPrototypes[m_resourceIdx].m_dna.m_boundsRadius * spawnInfo.m_spawner.m_resources.m_stampPrototypes[m_resourceIdx].m_dna.m_maxScale;
                    } */
            }
            return 0f;
        }


        /// <summary>
        /// Return the seed throw range of the thing referred to by the rule
        /// </summary>
        /// <param name="spawnInfo">Spawner information</param>
        /// <returns>Seed throw range</returns>
        public float GetSeedThrowRange(ref SpawnInfo spawnInfo)
        {
            switch (m_resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    {
                        return 1f; //Makes no sense, so return arbitrayr value
                    }
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    {
                        return spawnInfo.m_spawner.m_resources.m_detailPrototypes[m_resourceIdx].m_dna.m_seedThrow;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    {
                        return spawnInfo.m_spawner.m_resources.m_treePrototypes[m_resourceIdx].m_dna.m_seedThrow;
                    }
                case GaiaConstants.SpawnerResourceType.GameObject:
                    {
                        return spawnInfo.m_spawner.m_resources.m_gameObjectPrototypes[m_resourceIdx].m_dna.m_seedThrow;
                    }
                    /*
                default:
                    {
                        return spawnInfo.m_spawner.m_resources.m_stampPrototypes[m_resourceIdx].m_dna.m_seedThrow;
                    }
                     */
            }
            return 0f;
        }

        /// <summary>
        /// Spawn this rule
        /// </summary>
        /// <param name="spawnInfo"></param>
        public void Spawn(ref SpawnInfo spawnInfo)
        {
            //Check to see if we are active
            if (!m_isActive)
            {
                return;
            }

            //Check to see that we dont exceed our own instance count
            if (m_activeInstanceCnt >= m_maxInstances - 1)
            {
                return;
            }

            //TODO - Need to work out how to deal with instance counts because of the management overhead - for now will just increment counter

            //Update the instance counter
            m_activeInstanceCnt++;

            //Now spawn it baby!
            switch (m_resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    {
                        //Only interested in increasing values
                        if (spawnInfo.m_fitness > spawnInfo.m_textureStrengths[m_resourceIdx])
                        {
                            float delta = spawnInfo.m_fitness - spawnInfo.m_textureStrengths[m_resourceIdx];
                            float theRest = 1f - spawnInfo.m_textureStrengths[m_resourceIdx];
                            float adjustment = 0f;
                            if (theRest != 0f)
                            {
                                 adjustment = 1f - (delta / theRest);
                            }
    
                            for (int idx = 0; idx < spawnInfo.m_textureStrengths.Length; idx++)
                            {
                                if (idx == m_resourceIdx)
                                {
                                    spawnInfo.m_textureStrengths[idx] = spawnInfo.m_fitness;
                                }
                                else
                                {
                                    spawnInfo.m_textureStrengths[idx] *= adjustment;
                                }
                            }

                            spawnInfo.m_spawner.SetTextureMapsDirty();
                        }
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    {
                        HeightMap detailMap = spawnInfo.m_spawner.GetDetailMap(spawnInfo.m_hitTerrain.GetInstanceID(), m_resourceIdx);
                        int newStrength = 1;
                        if (spawnInfo.m_spawner.m_resources.m_detailPrototypes[m_resourceIdx].m_dna.m_rndScaleInfluence == true)
                        {
                            newStrength = (int)Mathf.Clamp(15f * spawnInfo.m_spawner.m_resources.m_detailPrototypes[m_resourceIdx].m_dna.GetScale(spawnInfo.m_fitness, spawnInfo.m_spawner.GetRandomFloat(0f, 1f)), 1f, 15f);
                        }
                        else
                        {
                            newStrength = (int)Mathf.Clamp(15f * spawnInfo.m_spawner.m_resources.m_detailPrototypes[m_resourceIdx].m_dna.GetScale(spawnInfo.m_fitness), 1f, 15f);
                        }

                        //Handle non cached scenario
                        if (detailMap == null)
                        {
                            int x = (int)(spawnInfo.m_hitLocationNU.x * spawnInfo.m_hitTerrain.terrainData.detailWidth);
                            int z = (int)(spawnInfo.m_hitLocationNU.z * spawnInfo.m_hitTerrain.terrainData.detailHeight);
                            int[,] detail = spawnInfo.m_hitTerrain.terrainData.GetDetailLayer(x, z, 1, 1, m_resourceIdx);
                            if (detail[0, 0] < newStrength)
                            {
                                detail[0, 0] = newStrength;
                                spawnInfo.m_hitTerrain.terrainData.SetDetailLayer(x, z, m_resourceIdx, detail);
                            }
                        }
                        else
                        //Handle cached scenario
                        {
                            if (detailMap[spawnInfo.m_hitLocationNU.z, spawnInfo.m_hitLocationNU.x] < newStrength)
                            {
                                detailMap[spawnInfo.m_hitLocationNU.z, spawnInfo.m_hitLocationNU.x] = newStrength;
                            }
                        }

                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    {
                        TreeInstance t = new TreeInstance();
                        t.prototypeIndex = m_resourceIdx;
                        t.position = spawnInfo.m_hitLocationNU;
                        if (spawnInfo.m_spawner.m_resources.m_treePrototypes[m_resourceIdx].m_dna.m_rndScaleInfluence == true)
                        {
                            t.widthScale = spawnInfo.m_spawner.m_resources.m_treePrototypes[m_resourceIdx].m_dna.GetScale(spawnInfo.m_fitness, spawnInfo.m_spawner.GetRandomFloat(0f, 1f));
                        }
                        else
                        {
                            t.widthScale = spawnInfo.m_spawner.m_resources.m_treePrototypes[m_resourceIdx].m_dna.GetScale(spawnInfo.m_fitness);
                        }
                        t.heightScale = t.widthScale;
                        t.rotation = spawnInfo.m_spawner.GetRandomFloat(0f, Mathf.PI * 2f);
                        t.color = spawnInfo.m_spawner.m_resources.m_treePrototypes[m_resourceIdx].m_healthyColour;
                        t.lightmapColor = Color.white;
                        spawnInfo.m_hitTerrain.AddTreeInstance(t);
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.GameObject:
                    {
                        ResourceProtoGameObject gameProto = spawnInfo.m_spawner.m_resources.m_gameObjectPrototypes[m_resourceIdx];
                        float scale = 1f;
                        if (gameProto.m_dna.m_rndScaleInfluence == true)
                        {
                            scale = gameProto.m_dna.GetScale(spawnInfo.m_fitness, spawnInfo.m_spawner.GetRandomFloat(0f, 1f));
                        }
                        else
                        {
                            scale = gameProto.m_dna.GetScale(spawnInfo.m_fitness);
                        }
                        Vector3 scaleVect = new Vector3(scale, scale, scale);
                        Vector3 location = spawnInfo.m_hitLocationWU;
                        for(int idx = 0; idx < gameProto.m_instances.Length; idx++)
                        {
                            location = spawnInfo.m_hitLocationWU;
                            location.y += (gameProto.m_instances[idx].m_terrainOffset * scale);
                            GameObject go = GameObject.Instantiate(gameProto.m_instances[idx].m_desktopPrefab) as GameObject;
                            go.name = "_Sp_" + go.name;
                            go.transform.parent = spawnInfo.m_spawner.transform;
                            go.transform.position = location;
                            go.transform.rotation = gameProto.m_instances[idx].m_desktopPrefab.transform.rotation;
                            go.transform.localScale = scaleVect;
                            if (gameProto.m_instances[idx].m_conformToSlope == true)
                            {
                                go.transform.rotation = Quaternion.FromToRotation(go.transform.up, spawnInfo.m_hitNormal) * go.transform.rotation;
                            }
                            if (gameProto.m_instances[idx].m_rndRotateX)
                            {
                                go.transform.rotation = Quaternion.AngleAxis(spawnInfo.m_spawner.GetRandomFloat(0f, 360f), Vector3.left) * go.transform.rotation;
                            }
                            if (gameProto.m_instances[idx].m_rndRotateY)
                            {
                                go.transform.rotation = Quaternion.AngleAxis(spawnInfo.m_spawner.GetRandomFloat(0f, 360f), Vector3.up) * go.transform.rotation;
                            }
                            if (gameProto.m_instances[idx].m_rndRotateZ)
                            {
                                go.transform.rotation = Quaternion.AngleAxis(spawnInfo.m_spawner.GetRandomFloat(0f, 360f), Vector3.forward) * go.transform.rotation;
                            }

                            //Tag it so we can use it later as it will now reference the pool
                            //go.AddComponent<SpawnInstance>();
                            //tag = go.GetComponent<SpawnInstance>();
                            //tag.m_transform = go.transform;
                            //tag.m_spawner = this;
                            //tag.m_spawnRule = rule;
                        }
                        break;
                    }
                    /*
                case Constants.SpawnerResourceType.Stamp:
                    {
                        //We will stamp into the cached terrain height map
                        break;
                    }
                     */

            }
        }


        /// <summary>
        /// Whether or not this rule needs a heightmap cache
        /// </summary>
        /// <returns>True if the rule needs a heightmap cache</returns>
        public bool CacheHeightMaps(Spawner spawner)
        {
            //Exit if not active
            if (!m_isActive)
            {
                return false;
            }

            /*
            //Always cache if we are a stamp
            if (m_resourceType == Constants.SpawnerResourceType.Stamp)
            {
                return true;
            }
            */

            return false;
        }


        /// <summary>
        /// Whether or not this rule needs a texture cache
        /// </summary>
        /// <returns>True if the rule needs a texture cache</returns>
        public bool CacheTextures(Spawner spawner)
        {
            //Exit if not active
            if (!m_isActive)
            {
                return false;
            }

            //Always cache if we are a texture
            if (m_resourceType == GaiaConstants.SpawnerResourceType.TerrainTexture)
            {
                return true;
            }

            //Cache if this rule refers to a texture
            switch (m_resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    {
                        return spawner.m_resources.m_texturePrototypes[m_resourceIdx].ChecksTextures();
                    }
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    {
                        return spawner.m_resources.m_detailPrototypes[m_resourceIdx].ChecksTextures();
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    {
                        return spawner.m_resources.m_treePrototypes[m_resourceIdx].ChecksTextures();
                    }
                case GaiaConstants.SpawnerResourceType.GameObject:
                    {
                        return spawner.m_resources.m_gameObjectPrototypes[m_resourceIdx].ChecksTextures();
                    }
                    /*
                case Constants.SpawnerResourceType.Stamp:
                    {
                        return spawner.m_resources.m_stampPrototypes[m_resourceIdx].ChecksTextures();
                    } */
            }

            //Exit nothing found
            return false;
        }

        /// <summary>
        /// Whether or not this rule needs a detail cache
        /// </summary>
        /// <returns>True if the rule needs a detail cache</returns>
        public bool CacheDetails()
        {
            //Exit if not active
            if (!m_isActive)
            {
                return false;
            }

            if (m_resourceType == GaiaConstants.SpawnerResourceType.TerrainDetail)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Whether or not this rule needs a proximity tag cache
        /// </summary>
        /// <returns>True if the rule needs a proximity tag cache</returns>
        public bool CacheProximity(Spawner spawner)
        {
            //Exit if not active
            if (!m_isActive)
            {
                return false;
            }

            //Cache if this rule refers to a texture
            switch (m_resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    {
                        return spawner.m_resources.m_texturePrototypes[m_resourceIdx].ChecksProximity();
                    }
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    {
                        return spawner.m_resources.m_detailPrototypes[m_resourceIdx].ChecksProximity();
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    {
                        return spawner.m_resources.m_treePrototypes[m_resourceIdx].ChecksProximity();
                    }
                case GaiaConstants.SpawnerResourceType.GameObject:
                    {
                        return spawner.m_resources.m_gameObjectPrototypes[m_resourceIdx].ChecksProximity();
                    }
                    /*
                case Constants.SpawnerResourceType.Stamp:
                    {
                        return spawner.m_resources.m_stampPrototypes[m_resourceIdx].ChecksProximity();
                    }
                     */
            }

            //Exit nothing found
            return false;
        }

        /// <summary>
        /// Add proximity tags to the tag list if they are not already on there
        /// </summary>
        /// <param name="spawner">Spawner that has the resources</param>
        /// <param name="tagList">Proximity tag list being added to</param>
        /// <returns></returns>
        public void AddProximityTags(Spawner spawner, ref List<string> tagList)
        {
            //Exit if not active
            if (!m_isActive)
            {
                return;
            }

            //Add tags
            switch (m_resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    {
                        spawner.m_resources.m_texturePrototypes[m_resourceIdx].AddTags(ref tagList);
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    {
                        spawner.m_resources.m_detailPrototypes[m_resourceIdx].AddTags(ref tagList);
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    {
                        spawner.m_resources.m_treePrototypes[m_resourceIdx].AddTags(ref tagList);
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.GameObject:
                    {
                        spawner.m_resources.m_gameObjectPrototypes[m_resourceIdx].AddTags(ref tagList);
                        break;
                    }
                    /*
                case Constants.SpawnerResourceType.Stamp:
                    {
                        spawner.m_resources.m_stampPrototypes[m_resourceIdx].AddTags(ref tagList);
                        break;
                    } */
            }
        }

        /*
        /// <summary>
        /// Add stamp paths to the stamp list if they are not already on there
        /// </summary>
        /// <param name="spawner">Spawner that has the resources</param>
        /// <param name="stampList">Stamp list being added to</param>
        /// <returns></returns>
        public void AddStamps(Spawner spawner, ref List<string> stampList)
        {
            //Exit if not active
            if (!m_isActive)
            {
                return;
            }

            //Add stamps
            if (m_resourceType == Constants.SpawnerResourceType.Stamp)
            {
                spawner.m_resources.m_stampPrototypes[m_resourceIdx].AddStamps(ref stampList);
            }
        }
        */
    }
}