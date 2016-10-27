using UnityEngine;
using System.Collections;
using Utility;

namespace BoxDash.Map {
    /// <summary>
    /// This class manager all the map content in game.
    /// Included map tile gameObject loading and danmacy map creation.
    /// </summary>
    public class MapManager : Singleton<MapManager>
    {
        #region Private variable
        // ---------- Private variable ----------
        private GameObject m_MapTilePrefab;
        private static readonly float m_TileLengthInHalf = 0.254f;
        // Calualate the diagonal line across the tile qube, as long as the
        // tile is a square, the diagonal line's lenght should be (2^2 * (length of side)). 
        private static readonly float m_TileOffset = Mathf.Sqrt(2) * m_TileLengthInHalf;

        #endregion

        #region Private methods
        /// <summary>
        /// Use this for initialization.
        /// </summary>
        private void Start() {
            LoadGameResource("Tile_white", out m_MapTilePrefab);

            CreateMap();
        }

        private void CreateMap() {
            Vector3 rotation = new Vector3(-90, 45, 0);
            for (int rowIndex = 0; rowIndex < 10; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < 5; columnIndex++)
                {
                    GameObject tile =Instantiate(
                        m_MapTilePrefab, 
                        new Vector3(columnIndex * m_TileOffset, 0, rowIndex * m_TileOffset), 
                        Quaternion.Euler(rotation)) as GameObject;
                    tile.transform.SetParent(this.transform);
                }
            }
        }

        /// <summary>
        /// A overload method of the LoadGameResource<T>(...),
        /// T will be set as "Object" as default.
        /// </summary>
        /// <param name="resourceName">The name of the resource.</param>
        /// <param name="resouce">The resouce object you pass in, return NULL if load failed.</param>
        /// <param name="resourceFolder">The specific folder under resources folder.</param>
        private void LoadGameResource(string resourceName, out GameObject resouce, string resourceFolder = "")
        {
            LoadGameResource<Object>(resourceName, out resouce, resourceFolder);
        }

        /// <summary>
        /// Use this method to load resources from the unity3D specific resources folder
        /// </summary>
        /// <typeparam name="T">The type of object this you wanna load.</typeparam>
        /// <param name="resourceName">The name of the resource.</param>
        /// <param name="resouce">The resouce object you pass in, return NULL if load failed.</param>
        /// <param name="resourceFolder">The specific folder under resources folder.</param>
        private void LoadGameResource<T>(string resourceName, out GameObject resouce, string resourceFolder = "") where T : Object {
            if (!string.IsNullOrEmpty(resourceFolder))
            {
                resouce = Resources.Load<T>(resourceFolder + "/" + resourceName) as GameObject;
            }
            else {
                resouce = Resources.Load<T>(resourceName) as GameObject;
            }
#if UNITY_EDITOR
            if (resouce == null) {
                if (string.IsNullOrEmpty(resourceFolder))
                {
                    Debug.LogError("Failed to load resouce " + resourceName +
                            " from Resources Folder.");
                }
                else {
                    Debug.LogError("Failed to load resouce " + resourceName +
                            " from " + resourceFolder + " Folder.");
                }
            }
#endif
        }
        #endregion
    }
}

