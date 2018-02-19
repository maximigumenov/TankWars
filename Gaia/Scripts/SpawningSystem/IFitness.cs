using UnityEngine;
using System.Collections;

namespace Gaia
{
    /// <summary>
    /// Create a class that implements this functionality in order to be able to extend and change how fitness is implemented.
    /// </summary>
    public interface IFitness
    {
        /// <summary>
        /// Call this to see if it can do fitness.
        /// </summary>
        /// <returns></returns>
        bool HasFitness();

        /// <summary>
        /// Call this to get the fitness of the location defined in spawnInfo, and update spawnInfo as well
        /// </summary>
        /// <param name="spawnInfo">Use this to get details about the location and update the fitness</param>
        /// <returns>Return the fitness of the location</returns>
        float GetFitness(ref SpawnInfo spawnInfo);
    }
}