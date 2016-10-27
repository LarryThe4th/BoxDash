using UnityEngine;
using System.Collections.Generic;
using BoxDash.Utility;

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
        private int m_MaxNumberOfTilesOnRow = 7;
        private int m_LengthOfMapChunk = 20;

        // Calualate the diagonal line across the tile qube, as long as the
        // tile is a square, the diagonal line's lenght should be (2^2 * (length of side)). 
        private static readonly float m_TileOffset = Mathf.Sqrt(2) * GameManager.TileSideLength;


        private List<List<GameObject>> m_MapList = new List<List<GameObject>>();
        #endregion

        #region Public varibales
        // ---------- Public varibales -----------
        [Tooltip("The main there color of the whole map.")]
        public Color32 MapThemeColor = Color.white;

        public Transform GetPlayerRespawnLocation { 
            get { return m_MapList[5][Mathf.RoundToInt(m_MaxNumberOfTilesOnRow / 2)].transform; }
            }
        #endregion

        #region Private methods
        /// <summary>
        /// Use this for initialization.
        /// </summary>
        public void InitMap() {
            if (!m_MapTilePrefab) ResourcesLoader.Load("Tile_white", out m_MapTilePrefab, "Map");
            if (!m_WallTilePrefab) ResourcesLoader.Load("Wall_white", out m_WallTilePrefab, "Map");

            CreateMap();
        }

        /// <summary>
        /// Create game map.
        /// </summary>
        private void CreateMap() {
#if UNITY_EDITOR
            if (!m_MapTilePrefab || !m_WallTilePrefab) {
                Debug.Log("Tile prefab is null.");
                return;
            }
#endif
            // Caluate the color of the wall.
            Color32 wallColor = MapThemeColor.ChangeColorBrightness(-0.3f);
            // Notice that each loop creates two row of tiles.
            for (int rowIndex = 0; rowIndex < m_LengthOfMapChunk; rowIndex++)
            {
                // Add tiles on the odd row.
                m_MapList.Add(CreateTilesOnOddRow(MapThemeColor, wallColor, rowIndex, m_MaxNumberOfTilesOnRow));
                // Add tiles on the even row.
                m_MapList.Add(CreateTilesOnEvenRow(MapThemeColor, rowIndex, m_MaxNumberOfTilesOnRow - 1));
            }

            // temp
            Camera.main.transform.position = new Vector3(
                m_TileOffset * Mathf.FloorToInt((m_MaxNumberOfTilesOnRow - 1) / 2),
                Camera.main.transform.position.y,
                Camera.main.transform.position.z);
        }

        /// <summary>
        /// Create tiles on the even rows.
        /// </summary>
        /// <param name="numberOfTilesOnColumn">The number of tiles on column.</param>
        private List<GameObject> CreateTilesOnOddRow(Color32 tileColor, Color32 wallColor, int rowIndex, int numberOfTilesOnColumn) {
            List<GameObject> column = new List<GameObject>();
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
                    render.material.color =
                        (columnIndex == 0 || columnIndex == (numberOfTilesOnColumn - 1)) ? wallColor : tileColor;
                }
                tile.transform.SetParent(this.transform);
                column.Add(tile);
            }
            return column;
        }

        /// <summary>
        /// Create tiles on the odd rows.
        /// </summary>
        /// <param name="numberOfTilesOnColumn">The number of tiles on column.</param>
        private List<GameObject> CreateTilesOnEvenRow(Color32 tileColor, int rowIndex, int numberOfTilesOnColumn)
        {
            List<GameObject> column = new List<GameObject>();
            // Get a lighter color for the tiles.
            Color32 secondlyColor = tileColor.ChangeColorBrightness(-.2f);
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
                column.Add(tile);
            }
            return column;
        }
        #endregion
    }
}

