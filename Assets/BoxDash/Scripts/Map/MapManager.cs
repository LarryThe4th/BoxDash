using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BoxDash.Utility;
using Random = UnityEngine.Random;

namespace BoxDash.Map {
    public class MapChunk {
        public Transform Root;
        public List<List<MapTile>> MapList = new List<List<MapTile>>();

        public MapTile GetTile(int row, int column)
        {
            if (row < 0 && column < 0) return null;
            return MapList[row][column];
        }

        public void AddRowOfTiles(int row, List<MapTile> tiles) {
            MapList.Add(new List<MapTile>(tiles));
        }

        public void ResetTiles() {
            foreach (var rows in MapList)
            {
                foreach (var tiles in rows)
                {
                    tiles.ResetTile(MapManager.Instance.GenerateObstacles());
                }
                // At lease one tile is safty for passing.
                rows[Random.Range(1, MapManager.MaxNumberOfTilesOnRow - 2)].ResetTile(TileTypes.Floor);
            }

        }
    }

    /// <summary>
    /// This class manager all the map content in game.
    /// Included map tile gameObject loading and danmacy map creation.
    /// </summary>
    public class MapManager : Singleton<MapManager>
    {
        #region Delegate and Events
        private void OnEnable()
        {
            GameManager.PlayerMovedOnMapEvent += PlayerPositionUpdated;
            GameManager.GameStartEvent += OnMapStartCollpase;
            GameManager.GameOverEvent += OnGameOver;
        }

        private void OnDisable() {
            GameManager.PlayerMovedOnMapEvent -= PlayerPositionUpdated;
            GameManager.GameStartEvent -= OnMapStartCollpase;
            GameManager.GameOverEvent -= OnGameOver;
        }
        #endregion

        #region Public varibales
        // ---------- Public varibales -----------
        [Tooltip("The main there color of the whole map.")]
        public Color32 MapThemeColor = Color.white;
        public Color32 PlayerTraceColor = Color.white;

        public const int MaxNumberOfTilesOnRow = 7;
        public const int LengthOfMapChunk = 20;
        public const int NumberOfMapChunk = 2;
        #endregion

        #region Private variables
        // ---------- Private variables ----------
        private GameObject m_MapTilePrefab = null;
        private GameObject m_WallTilePrefab = null;
        // Calualate the diagonal line across the tile qube, as long as the
        // tile is a square, the diagonal line's lenght should be (2^2 * (length of side)). 
        private static readonly float m_MapChunkOffset = LengthOfMapChunk * GameManager.TileOffset;
        private List<MapChunk> m_MapChunkList = new List<MapChunk>();

        private float m_CollapseSpeed = 0.1f;
        private int m_CollapsedRow = 0;
        private int m_CollapsingMapChunk = 0;
        private int m_TotalCollapedRowCount = 0;
        private bool m_KeepCollapsing = false;

        private int m_ChanceOfHole = 5;
        // private int m_ChanceOfSpike = 0;
        #endregion

        #region Private methods
        /// <summary>
        /// Use this for initialization.
        /// </summary>
        public void InitMap() {
            if (!m_MapTilePrefab) ResourcesLoader.Load("Tile_white", out m_MapTilePrefab, "Map");
            if (!m_WallTilePrefab) ResourcesLoader.Load("Wall_white", out m_WallTilePrefab, "Map");

            // Temp
            PlayerTraceColor = Color.yellow;

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

            // Reset the last created map chunk so there migth be obstacles on it.
            m_MapChunkList[m_MapChunkList.Count - 1].ResetTiles();
        }

        /// <summary>
        /// Create tiles on the even rows.
        /// </summary>
        /// <param name="numberOfTilesOnColumn">The number of tiles on column.</param>
        private List<MapTile> CreateTilesOnOddRow(Transform root, Color32 tileColor, Color32 wallColor, int rowIndex, int numberOfTilesOnColumn) {
            List<MapTile> column = new List<MapTile>();
            bool isWall = false;
            for (int columnIndex = 0; columnIndex < numberOfTilesOnColumn; columnIndex++)
            {
                isWall = (columnIndex == 0 || columnIndex == (numberOfTilesOnColumn - 1)) ? true : false;
                // Instantiate a new tile with position and rotatoin settings.
                GameObject tile = Instantiate(
                    // If current index is at the most left side of the map or most right side of the map, then its a wall.
                    isWall ? m_WallTilePrefab : m_MapTilePrefab,
                    // Set the tile position with offset.
                    new Vector3(columnIndex * GameManager.TileOffset, 0, rowIndex * GameManager.TileOffset),
                    // Change its rotation so it can facing the correct direction.
                    Quaternion.Euler(GameManager.TileRotation)) as GameObject;

                // Get the map tile script.
                MapTile mapTile = tile.GetComponent<MapTile>();

                mapTile.Init(isWall ? TileTypes.Wall : TileTypes.Floor);

                // Remenber the local position, use it when reseting the tiles.
                mapTile.OriginalLocalPosition = tile.transform.localPosition;

                // Note: if using MeshRenderer but not Renderer, the color setting will affect
                // on all the GameObjects which shares this same material.
                foreach (var render in tile.GetComponentsInChildren<Renderer>())
                {
                    // Change the apperence depend on its type.
                    render.material.color =
                        (columnIndex == 0 || columnIndex == (numberOfTilesOnColumn - 1)) ? wallColor : tileColor;
                    // Remenber its original color, it will comes in handy when the map reset itself.
                    mapTile.OriginalUpperMeshColor = render.material.color;
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
                    Quaternion.Euler(GameManager.TileRotation)) as GameObject;

                MapTile mapTile = tile.GetComponent<MapTile>();

                mapTile.Init(TileTypes.Floor);

                // Note: if using MeshRenderer but not Renderer, the color setting will affect
                // on all the GameObjects which shares this same material.
                foreach (var render in tile.GetComponentsInChildren<Renderer>())
                {
                    // Like befer, Since the even rows wont have a wall, set the normal color.
                    render.material.color = secondlyColor;
                    mapTile.OriginalUpperMeshColor = render.material.color;
                }
                tile.transform.SetParent(root);
                // Remenber the local position, use it when reseting the tiles.
                mapTile.OriginalLocalPosition = tile.transform.localPosition;
#if UNITY_EDITOR
                tile.name = "Even [ " + (rowIndex * 2 + 1) + ", " + columnIndex + " ]";
#endif
                column.Add(mapTile);
            }
            return column;
        }

        /// <summary>
        /// Called when player position updated.
        /// </summary>
        /// <param name="playerAtRow">The current player position on Row.</param>
        /// <param name="playerAtColumn">The current player position on column.</param>
        /// <param name="traceColor">The trace color that player will left behind.</param>
        private void PlayerPositionUpdated(int playerAtRow, int playerAtColumn) {
            // Leave a trace.
            ChangeTileUpperMeshColor(playerAtRow, playerAtColumn, PlayerTraceColor);

            // Check if the map needs update.
            MapUpdateCheck(playerAtRow);
        }

        /// <summary>
        /// Check if the player passed a specific point so that the map needs update itself
        /// to create a endless map.
        /// </summary>
        /// <param name="playerPositionOnY">The current player location.</param>
        private void MapUpdateCheck(int playerPositionOnY)
        {
            // First in sure the player get pass the first map chunck. 
            if (playerPositionOnY > LengthOfMapChunk * 2)
            {
                // Then give it a offset of half of the map chunk (Once again, LengthOfMapChunk is actul the half
                // of the map), everytime player pass a full chunk (LengthOfMapChunk * 2) call the 
                // mapManager to generate new map ahead.
                if ((playerPositionOnY + LengthOfMapChunk) % (LengthOfMapChunk * 2) == 0)
                {
                    // This means that player is now standing on the half of the map chunk
                    ResetMapChunk(playerPositionOnY);
                }
            }
        }

        private void ChangeTileUpperMeshColor(int playerAtRow, int playerAtColumn, Color32 color)
        {
            GetTile(playerAtRow, playerAtColumn).UpperMesh.material.color =
                playerAtRow % 2 != 0 ? color.ChangeColorBrightness(-0.3f) : color;
        }

        private void ResetMapChunk(int playerAtRow)
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
            m_MapChunkList[mapChunkIndexInList].ResetTiles();

            // Everytime the map chunk been reseted, obstacles 
            // will have a higher chance to be generate. 
            AddObstaclesGenerateChance();
        }

        private void OnMapStartCollpase() {
            m_KeepCollapsing = true;
            StartCoroutine(TileCollpase());
        }

        private IEnumerator TileCollpase() {
            while (m_KeepCollapsing) {
                m_TotalCollapedRowCount = m_CollapsedRow + (LengthOfMapChunk * 2 * m_CollapsingMapChunk);

                if (GameManager.Player.PlayerOnY - m_TotalCollapedRowCount <= 5)
                    m_CollapseSpeed = 0.18f;
                else
                    m_CollapseSpeed = 0.12f;


                yield return new WaitForSeconds(m_CollapseSpeed);
                // If current map chunk has not been reseted by player passing the reset point, 
                // trigger the tiles to collapse.
                // The reasome for doing this check is becuase some time the player go too fast,
                // and the map chunk is not yet fully collapse befer reset to new location, after
                // reset to the new location some of the tiles will keep been triggerd by this loop.
                foreach (var tile in m_MapChunkList[m_CollapsingMapChunk].MapList[m_CollapsedRow])
                {
                    tile.StartCollapse();
                    // Check if player is standing on the collapsed tile.
                    if (GameManager.Player.PlayerOnY <= m_TotalCollapedRowCount) {
                        // Hide the tile right under the player box so user can clear see how it falls.
                        m_MapChunkList[m_CollapsingMapChunk].MapList[m_CollapsedRow][GameManager.Player.PlayerOnX].DisplayTile(false);

                        GameManager.OnGameOver(CauseOfGameOver.OnCollapsedTile);
                        yield return null;
                    }
                }

                m_CollapsedRow++;

                // If a map chunk is fully collpased.
                if (m_CollapsedRow == LengthOfMapChunk * 2) {
                    // Reset its state.
                    m_CollapsingMapChunk++;
                    m_CollapsingMapChunk = m_CollapsingMapChunk % NumberOfMapChunk;
                    m_CollapsedRow = 0;
                }
            }
        }

        private void AddObstaclesGenerateChance() {
            m_ChanceOfHole += 2;
            if (m_ChanceOfHole >= 20) m_ChanceOfHole = 20;
        }

        private void OnGameOver(CauseOfGameOver cause)
        {
            // Stop tile from collapsing.
            m_KeepCollapsing = false;
            StopCoroutine(TileCollpase());
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

        public TileTypes GenerateObstacles()
        {
            int randomResult = Random.Range(0, 100);
            if (m_ChanceOfHole > randomResult) {
                return TileTypes.Hole;
            }
            return TileTypes.Floor;
        }
        #endregion
    }
}

