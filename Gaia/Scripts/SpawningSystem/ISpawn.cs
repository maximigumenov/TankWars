using UnityEngine;
using System.Collections;

namespace Gaia
{
    public interface ISpawn
    {
        //Call this to see if it can do fitness
        bool HasSpawn();

        /// <summary>
        /// Spawn an instance at the location provided.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        SpawnInstance Spawn(ref SpawnInfo spawnInfo);
    }
}