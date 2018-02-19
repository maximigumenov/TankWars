using UnityEngine;
using System.Collections;

namespace Gaia
{
    /// <summary>
    /// An instance of a spawned prefab. It enables spawned objects to be extended and managed. 
    /// If not there already it will be added to every object that the spawner creates.
    /// </summary>
    public class SpawnInstance : MonoBehaviour
    {
        public Transform    m_transform;    //The transform we belong to
        public Spawner     m_spawner;      //The spawner we belong to
        public SpawnRule    m_spawnRule;    //The rule we belong to (this is valid only while active)

        /// <summary>
        /// Some handy helper classes that will enable more sophisticated instance behavoiur.
        /// </summary>
        /// <returns></returns>

        public virtual bool CanSpawn(Vector3 location) 
        {
            return false;
        }

        public virtual SpawnInstance Spawn(Vector3 location)
        {
            return null;
        }

        public virtual bool Grow()
        {
            return false;
        }

        public virtual bool Die()
        {
            return false;
        }
    }
}