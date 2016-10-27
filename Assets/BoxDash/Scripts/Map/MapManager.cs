using UnityEngine;
using System.Collections;
using System.Linq;
using Utility;

namespace BoxDash.Map {
    /// <summary>
    /// This class manager all the map content in game.
    /// Included map tile gameObject loading and danmacy map creation.
    /// </summary>
    public class MapManager : Singleton<MapManager>
    {
        #region Private variables
        // ---------- Private variables ----------
        private GameObject m_MapTilePrefab = null;
        private GameObject m_WallTilePrefab = null;
        private Vector3 m_TileRotation = new Vector3(-90, 45, 0);
        private static readonly float m_TileLengthInHalf = 0.254f;
        // Calualate the diagonal line across the tile qube, as long as the
        // tile is a square, the diagonal line's lenght should be (2^2 * (length of side)). 
        private static readonly float m_TileOffset = Mathf.Sqrt(2) * m_TileLengthInHalf;
        private int m_MaxNumberOfTilesOnRow = 5;
        #endregion

        #region Public varibales
        // ---------- Public varibales -----------
        [Tooltip("The main there color of the whole map.")]
        public Color32 MapThemeColor = Color.white;
        #endregion

        #region Private methods
        /// <summary>
        /// Use this for initialization.
        /// </summary>
        private void Start() {
            LoadGameResource("Tile_white", out m_MapTilePrefab);
            LoadGameResource("Wall_white", out m_WallTilePrefab);
            CreateMap();
        }

        private void CreateMap() {
#if UNITY_EDITOR
            if (!m_MapTilePrefab || !m_WallTilePrefab) {
                Debug.Log("Tile prefab is null.");
                return;
            }
#endif
            CreateTilesOnEvenRow(m_MaxNumberOfTilesOnRow);
            CreateTilesOnOddRow(m_MaxNumberOfTilesOnRow - 1);
        }

        private void CreateTilesOnEvenRow(int numberOfTilesOnColumn) {
            for (int rowIndex = 0; rowIndex < 10; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < numberOfTilesOnColumn; columnIndex++)
                {
                    // Instantiate a new tile with position and rotatoin settings.
                    GameObject tile = Instantiate(
                        (columnIndex == 0 || columnIndex == (numberOfTilesOnColumn - 1)) ? m_WallTilePrefab : m_MapTilePrefab,
                        new Vector3(columnIndex * m_TileOffset, 0, rowIndex * m_TileOffset),
                        Quaternion.Euler(m_TileRotation)) as GameObject;

                    // Note: if using MeshRenderer but not Renderer, the color setting will affect
                    // on all the GameObjects which shares this same material.
                    foreach (var render in tile.GetComponentsInChildren<Renderer>())
                    {
                        render.material.color = MapThemeColor;
                    }

                    tile.transform.SetParent(this.transform);
                }
            }
        }

        private void CreateTilesOnOddRow(int numberOfTilesOnColumn)
        {
            // Get a lighter color for the tiles.
            Color32 secondlyColor = LighterColor(0.1f, MapThemeColor);
            for (int rowIndex = 0; rowIndex < 10; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < numberOfTilesOnColumn; columnIndex++)
                {
                    // Instantiate a new tile with position and rotatoin settings.
                    GameObject tile = Instantiate(
                        m_MapTilePrefab,
                        new Vector3(columnIndex * m_TileOffset + (m_TileOffset / 2), 0, rowIndex * m_TileOffset + (m_TileOffset / 2)),
                        Quaternion.Euler(m_TileRotation)) as GameObject;
                    // Note: if using MeshRenderer but not Renderer, the color setting will affect
                    // on all the GameObjects which shares this same material.
                    foreach (var render in tile.GetComponentsInChildren<Renderer>())
                    {
                        render.material.color = secondlyColor;
                    }
                    tile.transform.SetParent(this.transform);
                }
            }
        }

        /// <summary>
        /// Generate a lighter color based on the base color.
        /// </summary>
        /// <param name="whitelessFactor">How much ligter you want (From 0.1 to 1).</param>
        /// <param name="baseColor"></param>
        /// <returns>If passed in whitelessFactor is less then 0.1 or greater than 1, return the baseColor.</returns>
        private Color32 LighterColor(float whitelessFactor, Color32 baseColor) {
            if (whitelessFactor < 0.1 || whitelessFactor > 1) {
                return baseColor;
            }
            return new Color32(
                (byte)((255 - baseColor.r) * whitelessFactor + baseColor.r),
                (byte)((255 - baseColor.g) * whitelessFactor + baseColor.g),
                (byte)((255 - baseColor.b) * whitelessFactor + baseColor.b),
                baseColor.a
            );
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

