using UnityEngine;
using System.Collections;

namespace Gaia
{
    /// <summary>
    /// Create a class that implements this functionality in order to be able to extend and change how spawner completion is progressing.
    /// </summary>
    public interface ICompletion 
    {
        /// <summary>
        /// Call this to see if it can do completion
        /// </summary>
        /// <returns></returns>
        bool HasCompletion();

        /// <summary>
        /// Call this to get the degree of completion in range 0..1.
        /// </summary>
        /// <param name="spawner">The spawner this is for</param>
        /// <returns>Return the completion in range of 0 (not started) .. 1 (completed)</returns>
        float GetCompletion(ref Spawner spawner);
    }
}