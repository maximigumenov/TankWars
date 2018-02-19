using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gaia
{
    /// <summary>
    /// Prototype for textures and their fitness
    /// </summary>
    [System.Serializable]
    public class ResourceProtoTexture
    {
        [Tooltip("Resource name.")]
        public string m_name;
        [Tooltip("Resource texture.")]
        public Texture2D m_texture;
        [Tooltip("Resource normal.")]
        public Texture2D m_normal;
        [Tooltip("The width over which the image will stretch on the terrain’s surface.")]
        public float m_sizeX = 10;
        [Tooltip("The height over which the image will stretch on the terrain’s surface.")]
        public float m_sizeY = 10;
        [Tooltip("How far from the terrain’s anchor point the tiling will start.")]
        public float m_offsetX = 0;
        [Tooltip("How far from the terrain’s anchor point the tiling will start.")]
        public float m_offsetY = 0;
        [Tooltip("Controls the overall metalness of the surface."), Range(0f, 1f)]
        public float m_metalic = 0f;
        [Tooltip("Controls the overall smoothness of the surface."), Range(0f, 1f)]
        public float m_smoothness = 0f;
        [Tooltip("SPAWN CRITERIA - Spawn criteria are run against the terrain to assess its fitness in a range of 0..1 for use by this resource. If you add multiple criteria then the fittest one will be selected.")]
        public SpawnCritera[] m_spawnCriteria = new SpawnCritera[0];

        /// <summary>
        /// Initialise the texture
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