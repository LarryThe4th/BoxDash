using UnityEngine;
using System.Collections;

namespace BoxDash.Utility
{
    public static class ResourcesLoader
    {
        /// <summary>
        /// A overload method of the LoadGameResource<T>(...),
        /// T will be set as "Object" as default.
        /// </summary>
        /// <param name="resourceName">The name of the resource.</param>
        /// <param name="resouce">The resouce object you pass in, return NULL if load failed.</param>
        /// <param name="resourceFolder">The specific folder under resources folder.</param>
        public static void  Load(string resourceName, out GameObject resouce, string resourceFolder = "")
        {
            Load<Object>(resourceName, out resouce, resourceFolder);
        }

        /// <summary>
        /// Use this method to load resources from the unity3D specific resources folder
        /// </summary>
        /// <typeparam name="T">The type of object this you wanna load.</typeparam>
        /// <param name="resourceName">The name of the resource.</param>
        /// <param name="resouce">The resouce object you pass in, return NULL if load failed.</param>
        /// <param name="resourceFolder">The specific folder under resources folder.</param>
        public static void Load<T>(string resourceName, out GameObject resouce, string resourceFolder = "") where T : Object
        {
            if (!string.IsNullOrEmpty(resourceFolder))
            {
                resouce = Resources.Load<T>(resourceFolder + "/" + resourceName) as GameObject;
            }
            else
            {
                resouce = Resources.Load<T>(resourceName) as GameObject;
            }
#if UNITY_EDITOR
            if (resouce == null)
            {
                if (string.IsNullOrEmpty(resourceFolder))
                {
                    Debug.LogError("Failed to load resouce " + resourceName +
                            " from Resources Folder.");
                }
                else
                {
                    Debug.LogError("Failed to load resouce " + resourceName +
                            " from " + resourceFolder + " Folder.");
                }
            }
#endif
        }
    }

}
