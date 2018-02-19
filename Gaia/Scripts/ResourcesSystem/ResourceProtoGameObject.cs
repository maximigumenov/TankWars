using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gaia
{
    [System.Serializable]
    public class ResourceProtoGameObjectInstance
    {
        [Tooltip("Instance name.")]
        public string m_name;
        [Tooltip("Desktop prefab.")]
        public GameObject m_desktopPrefab;
        [Tooltip("Mobile prefab - future proofing here - not currently used.")]
        public GameObject m_mobilePrefab;
        [Tooltip("Offset from terrain in meters to intantiate at. Can use this to embed objects into the terrain or raise them above the terrain.")]
        public float m_terrainOffset = -0.25f;
        [Tooltip("Rotate the object to conform it to the terrain normal. Allows natural slope following. Great for things like trees to give them a little more variation in your scene.")]
        public bool m_conformToSlope = false;
        [Tooltip("Randomly rotate the object in the X axis.")]
        public bool m_rndRotateX = true;
        [Tooltip("Randomly rotate the object in the Y axis.")]
        public bool m_rndRotateY = true;
        [Tooltip("Randomly rotate the object in the Z axis.")]
        public bool m_rndRotateZ = true;
    }

    [System.Serializable]
    public class ResourceProtoGameObject
    {
        [Tooltip("Resource name.")]
        public string m_name;
        [Tooltip("The game objects that will be instantiated when this is spawned.")]
        public ResourceProtoGameObjectInstance[] m_instances = new ResourceProtoGameObjectInstance[0];
        [Tooltip("DNA - Used by the spawner to control how and where the game objects will be spawned.")]
        public ResourceProtoDNA m_dna = new ResourceProtoDNA();
        [Tooltip("SPAWN CRITERIA - Spawn criteria are run against the terrain to assess its fitness in a range of 0..1 for use by this resource. If you add multiple criteria then the fittest one will be selected.")]
        public SpawnCritera[] m_spawnCriteria = new SpawnCritera[0];

        /// <summary>
        /// Initialise the game object
        /// </summary>
        /// <param name="spawner">The spawner it belongs to</param>
        public void Initialise(Spawner spawner)
        {
            foreach (SpawnCritera criteria in m_spawnCriteria)
            {
                criteria.Initialise(spawner);
            }
        }

        /// <summary>
        /// Determine whether this has active criteria
        /// </summary>
        /// <returns>True if has actrive criteria</returns>
        public bool HasActiveCriteria()
        {
            for (int idx = 0; idx < m_spawnCriteria.Length; idx++)
            {
                if (m_spawnCriteria[idx].m_isActive)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determine whether this has active criteria that checks textures
        /// </summary>
        /// <returns>True if has active criteria that checks textures</returns>
        public bool ChecksTextures()
        {
            for (int idx = 0; idx < m_spawnCriteria.Length; idx++)
            {
                if (m_spawnCriteria[idx].m_isActive && m_spawnCriteria[idx].m_checkTexture)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determine whether this has active criteria that checks proximity
        /// </summary>
        /// <returns>True if has active criteria that checks proximity</returns>
        public bool ChecksProximity()
        {
            for (int idx = 0; idx < m_spawnCriteria.Length; idx++)
            {
                if (m_spawnCriteria[idx].m_isActive && m_spawnCriteria[idx].m_checkProximity)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Add tags to the list if they are not already there
        /// </summary>
        /// <param name="tagList">The list to add the tags to</param>
        public void AddTags(ref List<string> tagList)
        {
            for (int idx = 0; idx < m_spawnCriteria.Length; idx++)
            {
                if (m_spawnCriteria[idx].m_isActive && m_spawnCriteria[idx].m_checkProximity)
                {
                    if (!tagList.Contains(m_spawnCriteria[idx].m_proximityTag))
                    {
                        tagList.Add(m_spawnCriteria[idx].m_proximityTag);
                    }
                }
            }
        }

    }
}