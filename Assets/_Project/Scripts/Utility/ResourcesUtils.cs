using UnityEngine;
using UnityEngine.Rendering;

namespace NJG.Utilities 
{
    public static class ResourcesUtils {
        /// <summary>
        /// Load volume profile from given path.
        /// </summary>
        /// <param name="path">Path from where volume profile should be loaded.</param>
        public static void LoadVolumeProfile(this Volume volume, string path) 
        {
            VolumeProfile profile = Resources.Load<VolumeProfile>(path);
            volume.profile = profile;
        }
    }
}