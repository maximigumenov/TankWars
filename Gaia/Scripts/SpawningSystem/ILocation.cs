using UnityEngine;
using System.Collections;

namespace Gaia
{
    /// <summary>
    /// Create a class that implements this functionality in order to be able to extend and change how spawner location is determined.
    /// </summary>
    public interface ILocation
    {
        /// <summary>
        /// Call this to see if it can do location
        /// </summary>
        /// <returns></returns>
        bool HasLocation();

        /// <summary>
        /// Call this to get the next location to spawn
        /// </summary>
        /// <param name="startLocation">Starting point</param>
        /// <param name="spawner">The spawner that is calling us</param>
        /// <returns>New location</returns>
        Vector3 GetLocation(Vector3 startLocation, ref Spawner spawner);
    }
}