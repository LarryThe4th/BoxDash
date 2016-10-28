using UnityEngine;
using System.Collections.Generic;
using BoxDash.Utility;

namespace BoxDash.Map {
    public class MapChunk {
        public Transform Root;
        public List<List<MapTile>> m_MapList = new List<List<MapTile>>();

        public MapTile GetTile(int row, int column)
        {
            if (row < 0 && column < 0) return null;
            return m_MapList[row][column];
        }

        public void AddRowOfTiles(int row, List<MapTile> tiles) {
            m_MapList.Add(new List<MapTile>(tiles));
        }
    }

    /// <summary>
    /// This class manager all the map content in game.
    /// Included map tile gameObject loading and danmacy map creation.
    /// </summary>
    public class MapManager : Singleton<MapManager>
    {
        #region Delegate and Events
        public delegate void ResetMapChunk(int playerAtRow);
        public static ResetMapChunk ResetMapChunkEvent;
        public static void OnCorssHalfOfTheMapChunck(int playerAtRow)
        {
            if (ResetMapChunkEvent != null) ResetMapChunkEvent(playerAtRow);
        }

        private void OnEnable()
        {
            ResetMapChunkEvent += ResetMapChunkPosition;
        }

        private void OnDisable() {
            ResetMapChunkEvent -= ResetMapChunkPosition;
        }
        #endregion

        #region Public varibales
        // ---------- Public varibales -----------
        [Tooltip("The main there color of the whole map.")]
        public Color32 MapThemeColor = Color.white;

        public const int MaxNumberOfTilesOnRow = 7;
        public const int LengthOfMapChunk = 15;
        public const int NumberOfMapChunk = 2;

        #endregion

        #region Private variables
        // ---------- Private variables ----------
        private GameObject m_MapTilePrefab = null;
        private GameObject m_WallTilePrefab = null;

        private Vector3 m_TileRotation = new Vector3(-90, 45, 0);

        // Calualate the diagonal line across the tile qube, as long as the
        // tile is a square, the diagonal line's lenght should be (2^2 * (length of side)). 
        private static readonly float m_MapChunkOffset = LengthOfMapChunk * GameManager.TileOffset;
        private List<MapChunk> m_MapChunkList = new List<MapChunk>();
        #endregion

        #region Private methods
        /// <summary>
        /// Use this for initialization.
        /// </summary>
        public void InitMap() {
            if (!m_MapTilePrefab) ResourcesLoader.Load("Tile_white", out m_MapTilePrefab, "Map");
            if (!m_WallTilePrefab) ResourcesLoader.Load("Wall_white", out m_WallTilePrefab, "Map");

            CreateMapChunk();
        }

        /// <summary>
        /// Create a chunk of game map.
        /// The actul game map is combined by two chunk of map, each map chunck contents fix amount of tiles.
        /// When player move near to the eage of one of map chunk, other chunk will reset its position so it can be
        /// connect with current chunk of map.
        /// Repeat this process everytime when the player get near the eage.
        /// </summary>
        private void CreateMapChunk() {
            // Caluate the color of the wall.
            Color32 wallColor = MapThemeColor.ChangeColorBrightness(-0.3f);

            for (int chunkIndex = 0; chunkIndex < NumberOfMapChunk; chunkIndex++)
            {
                // Create a new gameObject as a root transform of the tile group.
                GameObject chunkRoot = new GameObject();
#if UNITY_EDITOR
                // Change its name for easies bugging in editor.
                chunkRoot.name = "MapChunkRoot " + chunkIndex;
#endif

                // Create a new chunk for store all the map tile data.
                MapChunk chunk = new MapChunk();
                // Notice that each loop creates two rows of tiles.
                for (int rowIndex = 0; rowIndex < LengthOfMapChunk; rowIndex++)
                {
                    // Add tiles on the odd row.
                    chunk.AddRowOfTiles(rowIndex, CreateTilesOnOddRow(chunkRoot.transform, MapThemeColor, wallColor, rowIndex, MaxNumberOfTilesOnRow));
                    // Add tiles on the even row.
                    chunk.AddRowOfTiles(rowIndex, CreateTilesOnEvenRow(chunkRoot.transform, MapThemeColor, rowIndex, MaxNumberOfTilesOnRow - 1));
                }

                // Reset its parent transform to the map manager for better management.
                chunkRoot.transform.SetParent(this.transform);
                // Set its offset position based on its index in the list.
                chunkRoot.transform.position = new Vector3(0, 0, chunkIndex * m_MapChunkOffset);
                // Apply the new create transform to this map chunk.
                chunk.Root = chunkRoot.transform;
                // Add all the datachunkRoot into the chunk.
                m_MapChunkList.Add(chunk);
            }
        }

        /// <summary>
        /// Create tiles on the even rows.
        /// </summary>
        /// <param name="numberOfTilesOnColumn">The number of tiles on column.</param>
        private List<MapTile> CreateTilesOnOddRow(Transform root, Color32 tileColor, Color32 wallColor, int rowIndex, int numberOfTilesOnColumn) {
            List<MapTile> column = new List<MapTile>();
            for (int columnIndex = 0; columnIndex < numberOfTilesOnColumn; columnIndex++)
            {
                // Instantiate a new tile with position and rotatoin settings.
                GameObject tile = Instantiate(
                    // If current index is at the most left side of the map or most right side of the map, then its a wall.
                    (columnIndex == 0 || columnIndex == (numberOfTilesOnColumn - 1)) ? m_WallTilePrefab : m_MapTilePrefab,
                    // Set the tile position with offset.
                    new Vector3(columnIndex * GameManager.TileOffset, 0, rowIndex * GameManager.TileOffset),
                    // Change its rotation so it can facing the correct direction.
                    Quaternion.Euler(m_TileRotation)) as GameObject;

                // Get the map tile script.
                MapTile mapTile = tile.GetComponent<MapTile>();

                // Note: if using MeshRenderer but not Renderer, the color setting will affect
                // on all the GameObjects which shares this same material.
                foreach (var render in tile.GetComponentsInChildren<Renderer>())
                {
                    // Change the apperence depend on its type.
                    render.material.color =
                        (columnIndex == 0 || columnIndex == (numberOfTilesOnColumn - 1)) ? wallColor : tileColor;
                    // Remenber its original color, it will comes in handy when the map reset itself.
                    mapTile.UpperMeshColor = render.material.color;
                }
                // Reset it parent transform so when we want to move this chunk of tiles we only
                // need to move this parent object.
                tile.transform.SetParent(root);
#if UNITY_EDITOR
                tile.name = "Odd [ " + rowIndex * 2 + ", " + columnIndex + " ]";
#endif
                // Finaly, add these tiles into the list.
                column.Add(mapTile);
            }
            return column;
        }

        /// <summary>
        /// Create tiles on the odd rows.
        /// </summary>
        /// <param name="numberOfTilesOnColumn">The number of tiles on column.</param>
        private List<MapTile> CreateTilesOnEvenRow(Transform root, Color32 tileColor, int rowIndex, int numberOfTilesOnColumn)
        {
            List<MapTile> column = new List<MapTile>();
            // Get a lighter color for the tiles.
            Color32 secondlyColor = tileColor.ChangeColorBrightness(-.2f);
            for (int columnIndex = 0; columnIndex < numberOfTilesOnColumn; columnIndex++)
            {
                // Instantiate a new tile with position and rotatoin settings.
                GameObject tile = Instantiate(
                    // Since the even rows wont have a wall, just pass in the normal map tile.
                    m_MapTilePrefab,
                    new Vector3(
                        columnIndex * GameManager.TileOffset + (GameManager.TileOffset / 2), 
                        0, 
                        rowIndex * GameManager.TileOffset + (GameManager.TileOffset / 2)),
                    Quaternion.Euler(m_TileRotation)) as GameObject;

                MapTile mapTile = tile.GetComponent<MapTile>();

                // Note: if using MeshRenderer but not Renderer, the color setting will affect
                // on all the GameObjects which shares this same material.
                foreach (var render in tile.GetComponentsInChildren<Renderer>())
                {
                    // Like befer, Since the even rows wont have a wall, set the normal color.
                    render.material.color = secondlyColor;
                    mapTile.UpperMeshColor = render.material.color;
                }
                tile.transform.SetParent(root);
#if UNITY_EDITOR
                tile.name = "Even [ " + (rowIndex * 2 + 1) + ", " + columnIndex + " ]";
#endif
                column.Add(mapTile);
            }
            return column;
        }

        private void ResetMapChunkPosition(int playerAtRow)
        {
            // First get the logical index (AKA. the N.th map chunk player has passed) of the map chunk.
            int currentAtIndex = playerAtRow / (LengthOfMapChunk * 2);
            // Then clamp ot into betweem 0 and total number of map chunls.
            int currentChunkListIndex = currentAtIndex % NumberOfMapChunk;

            int previousMapChunkIndex = currentChunkListIndex == 0 ? NumberOfMapChunk - 1 : currentChunkListIndex - 1;

            // Reset the previous map change to further location
            m_MapChunkList[previousMapChunkIndex].Root.position =
                new Vector3(0, 0, (currentAtIndex + 1) * m_MapChunkOffset);

            // Reset all the contents left in previous map chunk
            ResetMapChunkContent(previousMapChunkIndex);
        }

        /// <summary>
        /// Leave no trace behind.
        /// </summary>
        private void ResetMapChunkContent(int mapChunkIndexInList)
        {
            for (int row = 0; row < m_MapChunkList[mapChunkIndexInList].m_MapList.Count; row++)
            {
                for (int column = 0; column < m_MapChunkList[mapChunkIndexInList].m_MapList[row].Count; column++)
                {
                    // Reset its color so that the player's trace now gone.
                    m_MapChunkList[mapChunkIndexInList].m_MapList[row][column].UpperMesh.material.color =
                        m_MapChunkList[mapChunkIndexInList].m_MapList[row][column].UpperMeshColor;
                }
            }
        }

        #endregion

        #region Public methods
        public MapTile GetTile(int playerAtRow, int playerAtColumn) {
            // For example, if the length of the map chunk is 10 and the player is at (4, 6),
            // so we check sreach for the tile where the player in standing on in first chunk of map, 
            // if the player is at (12, 6) than we look into the second chunk of map.
            // Since the map chunck will reposition itself once is fully out of screen, when player
            // is at (22, 5) than the actul tile should be in the first list of chunk (2, 5).
            // ------------------------------------------------------------------------------
            // The index of the rows will always clamp betweem 0 and the length of per-chunk,
            // and the the player at column will always between 0 and maximun number of tiles in a row.
            return m_MapChunkList[(playerAtRow / (LengthOfMapChunk * 2)) % NumberOfMapChunk].GetTile(playerAtRow % (LengthOfMapChunk * 2), playerAtColumn);
        }

        public void ChangeTileUpperMeshColor(int playerAtRow, int playerAtColumn, Color32 color)
        {
            GetTile(playerAtRow, playerAtColumn).UpperMesh.material.color =
                playerAtRow % 2 != 0 ? color.ChangeColorBrightness(-0.3f) : color;
        }
        #endregion
    }
}

