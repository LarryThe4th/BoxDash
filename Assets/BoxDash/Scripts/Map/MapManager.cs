using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using BoxDash.Tile;
using BoxDash.Player;
using BoxDash.Utility;

namespace BoxDash
{
    /// <summary>
    /// This class manager all the map content in game.
    /// Included map tile gameObject loading and danmacy map creation.
    /// </summary>
    public class MapManager : Singleton<MapManager>
    {
        #region Events
        private void OnEnable()
        {
            EventCenter.GameStartEvent += OnGameStarted;
            EventCenter.PlayerMovedOnMapEvent += OnPlayerMoved;
        }

        private void OnDisable()
        {
            EventCenter.GameStartEvent -= OnGameStarted;
            EventCenter.PlayerMovedOnMapEvent -= OnPlayerMoved;
        }
        #endregion

        #region Public varibales
        public GameObject m_FloorTilePrefab = null;
        public GameObject m_WallTilePrefab = null;
        public GameObject m_HoleTilePrefab = null;
        public GameObject m_FloorSpikesTilePrefab = null;
        public GameObject m_SkySpikesTilePrefab = null;

        // The main theme color of the map.
        [Tooltip("The main there color of the whole map.")]
        public Color32 MapThemeColor = Color.white;
        // The color of the trace that will be left behind on 
        // those tiles where the player step on.
        public Color32 PlayerTraceColor = Color.white;
        public Color32 TrapTileColor = Color.white;

        public int GetMaximunTilesOnColnum {
            get { return MaxNumberOfTilesOnColumn; }
        }
        #endregion

        #region Private variables
        #region Track color relate
        // Since the wall tiles are also a part of the map, its color
        // will be depende on the theme color.
        private Color32 m_MapWallTileColor = Color.white;
        // The sceondly theme color of the tiles.
        private Color32 m_SecondlyThemeColor = Color.white;
        #endregion

        #region Track relate
        // The maximun number of tiles on column.
        private const int MaxNumberOfTilesOnColumn = 7;
        private const int LengthOfPreTrack = 30;
        private const int NumberOfPreGenerateTracks = 2;
        // Keep tracking the player progress.
        private int m_PlayerPassedTrackCount = 0;
        // A pool of 2 tracks contains 20 rows of tiles each.
        private List<TileBase>[,] m_TilePool = new List<TileBase>[NumberOfPreGenerateTracks, LengthOfPreTrack];
        // We have to ensure that at least one tile is 
        // save to pass into the exit row for the player. 
        private int m_SafePathTileColumnIndex = 0;
        #endregion

        #region Tile collapse function relate
        private bool m_KeepCollapsing = false;
        // The count down timer.
        private int m_Timer = 0;
        // The time limit for the timer. 
        // Samller the number, faster the track will collapse,
        private int m_TimeBetweenCollapse = 10;
        // The collapsing row's index in the tile pool.
        private int m_CollapseRowIndex = 0;
        // Keep tracking how many tracks are collapsed.
        private int m_CurrentCollapsingTrackCount = 0;
        #endregion

        #region Trap and obstacles generate chance relate
        // The chance of a tile being a hole on the ground.
        private int m_ChanceOfHole = 5;
        private const int m_ChanceIncreasePreUpdate_Hole = 1;
        private const int m_MaximunChanceOfHole = 30;
        private int m_ChanceOfFloorSpikes = m_MaximunChanceOfHole;
        private const int m_ChanceIncreasePreUpdate_FloorSpikes = 1;
        private const int m_MaximunChanceOfFloorSpikes = 50;
        private int m_ChanceOfSkySpikes = m_MaximunChanceOfFloorSpikes;
        private const int m_ChanceIncreasePreUpdate_SkySpikes = 1;
        private const int m_MaximunChanceOfSkySpikes = 70;
        #endregion
        #endregion

        #region Private methods
        /// <summary>
        /// Use this to init the map manager when game start.
        /// </summary>
        public void Init() {
            InitGameResources();
            // Set the color of the wall tiles.
            m_MapWallTileColor = MapThemeColor.ChangeColorBrightness(-0.3f);
            m_SecondlyThemeColor = MapThemeColor.ChangeColorBrightness(-.2f);

            CreateTrack();
        }

        /// <summary>
        /// Load all the game resources form the resource folder.
        /// </summary>
        private void InitGameResources()
        {
            // Calculate how many objects we need in the pool.
            int needPoolSize = LengthOfPreTrack * MaxNumberOfTilesOnColumn * NumberOfPreGenerateTracks;
            const string prefabFolderName = "Tiles";

            if (!m_FloorTilePrefab)
                ResourcesLoader.Load("Floor", out m_FloorTilePrefab, prefabFolderName);
            CreateObjectPool(m_FloorTilePrefab, needPoolSize - LengthOfPreTrack * 3);

            if (!m_WallTilePrefab)
                ResourcesLoader.Load("Wall", out m_WallTilePrefab, prefabFolderName);
            CreateObjectPool(m_WallTilePrefab, LengthOfPreTrack * 2);

            if (!m_HoleTilePrefab)
                ResourcesLoader.Load("Hole", out m_HoleTilePrefab, prefabFolderName);
            CreateObjectPool(m_HoleTilePrefab, LengthOfPreTrack * MaxNumberOfTilesOnColumn);

            if (!m_FloorSpikesTilePrefab)
                ResourcesLoader.Load("FloorSpikes", out m_FloorSpikesTilePrefab, prefabFolderName);
            CreateObjectPool(m_FloorSpikesTilePrefab, LengthOfPreTrack * MaxNumberOfTilesOnColumn);

            if (!m_SkySpikesTilePrefab)
                ResourcesLoader.Load("SkySpikes", out m_SkySpikesTilePrefab, prefabFolderName);
            CreateObjectPool(m_SkySpikesTilePrefab, LengthOfPreTrack * MaxNumberOfTilesOnColumn);
        }

        private void CreateObjectPool(GameObject prefab, int poolSize) {
            ObjectPoolManager.Instance.CreaterPool(
                prefab.GetComponent<TileBase>().GetTileType().ToString(),
                prefab.GetComponent<PoolObject>(),
                this.transform, poolSize);
        }

        private void OnGameStarted()
        {
            m_KeepCollapsing = true;
        }

        /// <summary>
        /// Create a chunck of track for the game map, 
        /// once the player passed a specific checkpoint the map
        /// will keep generate new track ahead.
        /// </summary>
        private void CreateTrack()
        {
            // Pick a random tile (Wall is not included) as the start of the safe path.
            m_SafePathTileColumnIndex = Random.Range(1, MaxNumberOfTilesOnColumn - 1);
            for (int trackIndex = 0; trackIndex < NumberOfPreGenerateTracks; trackIndex++)
            {
                for (int rowIndex = 0; rowIndex < LengthOfPreTrack; rowIndex++)
                {
                    // Create a new row of slots for those tiles.
                    m_TilePool[trackIndex, rowIndex] = new List<TileBase>();
                    ResetRowOfTiles(
                        m_TilePool[trackIndex, rowIndex],
                        rowIndex + trackIndex * LengthOfPreTrack,
                        // Check if current row is a odd or even.
                        ((rowIndex % 2 == 0) ? true : false));
                }
            }
        }

        /// <summary>
        /// After the player moved, check where the player is standing on.
        /// </summary>
        /// <param name="currentTile">The tile where the player box is standing on.</param>
        private void OnPlayerMoved(TileBase currentTile) {
            switch (currentTile.GetTileType()) {
                case TileTypes.Floor:
                    currentTile.UseTile(
                        currentTile.CurrentLocation.Y % 2 == 0 ?
                        PlayerTraceColor.ChangeColorBrightness(-0.2f) : PlayerTraceColor);
                    break;
                case TileTypes.Hole:
                    // Whpoos..
                    EventCenter.OnGameOver(CauseOfGameOver.FallInHole);
                    return;
                case TileTypes.FloorSpikes:
                    currentTile.UseTile(
                        currentTile.CurrentLocation.Y % 2 == 0 ?
                        PlayerTraceColor.ChangeColorBrightness(-0.2f) : PlayerTraceColor);
                    break;
                case TileTypes.SkySpikes:
                    currentTile.UseTile(
                        currentTile.CurrentLocation.Y % 2 == 0 ?
                        PlayerTraceColor.ChangeColorBrightness(-0.2f) : PlayerTraceColor);
                    break;
                default:
                    break;
            }

            UpdateTrackCheck(currentTile);
        }

        private void UpdateTrackCheck(TileBase currentTile) {
            // If player passed half of the track.
            if (currentTile.CurrentLocation.Y % LengthOfPreTrack == LengthOfPreTrack / 3)
            {

                m_PlayerPassedTrackCount = currentTile.CurrentLocation.Y / LengthOfPreTrack;
                // We don't need to update the track when player 
                // is still at the first part of the track.
                if (m_PlayerPassedTrackCount != 0)
                {
                    // The rise the difficulty.
                    RaisTrapGenerateChance();

                    // Reset the track behind this track.
                    ResetOldTrack(m_PlayerPassedTrackCount);
                }
            }
        }

        private void RaisTrapGenerateChance() {
            if (m_ChanceOfHole <= m_MaximunChanceOfHole) {
                m_ChanceOfHole += m_ChanceIncreasePreUpdate_Hole;
            }
            if (m_ChanceOfFloorSpikes <= m_MaximunChanceOfFloorSpikes) {
                m_ChanceOfFloorSpikes += m_ChanceIncreasePreUpdate_FloorSpikes;
            }
            if (m_ChanceOfSkySpikes <= m_MaximunChanceOfSkySpikes) {
                m_ChanceOfSkySpikes += m_ChanceIncreasePreUpdate_SkySpikes;
            }
        }

        private void ResetOldTrack(int trackPassedCount) {
            int oldTrackIndex = (trackPassedCount % 2 == 0) ? 1 : 0;
            for (int rowIndexInTrack = 0; rowIndexInTrack < LengthOfPreTrack; rowIndexInTrack++)
            {
                ResetRowOfTiles(
                    m_TilePool[oldTrackIndex, rowIndexInTrack],
                    rowIndexInTrack + ((trackPassedCount + 1) * LengthOfPreTrack),
                    (rowIndexInTrack % 2 == 0) ? true : false);
            }
        }

        /// <summary>
        /// As long as this tile is not a wall,
        /// we can random choice a tile type for this new tile.
        /// </summary>
        private TileTypes RandomTileType(int currentColumnIndex) {
            // If this tile should be a save path.
            if (currentColumnIndex == m_SafePathTileColumnIndex) {
                return TileTypes.Floor;
            }
            // Generate a random number.
            int RNGresult = Random.Range(0, 100);
            if (m_ChanceOfSkySpikes > RNGresult && RNGresult > m_MaximunChanceOfFloorSpikes)
                return TileTypes.SkySpikes;
            if (m_ChanceOfFloorSpikes > RNGresult && RNGresult > m_MaximunChanceOfHole)
                return TileTypes.FloorSpikes;
            // If the generate chance is match and this tile is not on the safe path.
            if (m_ChanceOfHole > RNGresult)
                return TileTypes.Hole;
            return TileTypes.Floor;
        }

        private Color32 SetTileColor(bool isOddRow, TileTypes type)
        {
            switch (type) {
                case TileTypes.Wall:
                    return m_MapWallTileColor;
                case TileTypes.Floor:
                    return (isOddRow ? MapThemeColor : m_SecondlyThemeColor);
                case TileTypes.FloorSpikes:
                    return TrapTileColor;
                case TileTypes.SkySpikes:
                    return TrapTileColor;
                default:
                    return MapThemeColor;
            }
        }

        /// <summary>
        /// Get a new tile object form the object pool, and reset its data based on the setting.
        /// </summary>
        /// <param name="type">The diecded type of the tile.</param>
        /// <param name="worldPosition">The world position of the tile.</param>
        /// <returns>Return NULL when this tile is a hole or something went worry.</returns>
        private TileBase ReuseTile(TileTypes type, Vector3 worldPosition, Quaternion rotation)
        {
            ObjectInstance reuseTileObject =
                ObjectPoolManager.Instance.ReuseObject(type.ToString(), worldPosition, rotation);
            return reuseTileObject.Instance.GetComponent<TileBase>();
        }

        private void AddTileIntoList(List<TileBase> theRow, TileBase tile, int index) {
            // If the requested index is out of the list's range, 
            // add a new slot for this tile.
            if ((index + 1) > theRow.Count) {
                theRow.Add(tile);
            }
            // Else overwrite the exist data.
            else {
                if (tile.GetTileType() != TileTypes.Wall) {
                    theRow[index].EnableObject(false);
                } 
                theRow[index] = tile;
            }
        }

        /// <summary>
        /// After reuse the row of tiles, calulate the next save path for the track so we can
        /// ensure this game can run untill forever as long as the player is good enough. 
        /// </summary>
        /// <param name="currentRowTileCount">The count of tiles on this row.</param>
        private void EnsureNextSafePathTile(bool currentRowIsEven, int currentRowTileCount)
        {
            // Odd rows means no wall.
            if (!currentRowIsEven && m_SafePathTileColumnIndex == 0)
            {
                m_SafePathTileColumnIndex++;
            } else if (!currentRowIsEven && m_SafePathTileColumnIndex == (MaxNumberOfTilesOnColumn - 2)) {
                // No need to do anything.
            }
            else
            {
                // Decide go right or go left
                bool left = Random.Range(0, 2) == 0 ? false : true;
                // If currently the safe path is on the even row and decide go left. 
                if (currentRowIsEven && left) m_SafePathTileColumnIndex--;
                // Else if currently the safe path is on the even row and decide go right. 
                else if (!currentRowIsEven && !left) m_SafePathTileColumnIndex++;
            }
        }

        /// <summary>
        /// Create tiles on one line each of a time.
        /// </summary>
        /// <param name="root">The root transform of these tiles.</param>
        /// <param name="rowIndex">The current index of the row.</param>
        /// <param name="numberOfTilesOnColumn">The number of tiles on this column.</param>
        /// <returns></returns>
        private void ResetRowOfTiles(List<TileBase> theRow, int rowIndex, bool isEvenRow) {
            // A flag for identity which tile are normal and which are walls.
            bool isWall = false;
            // Depend on the index of the current row index the number of tiles on this column will change.
            int numbserOfTiles = (isEvenRow ? MaxNumberOfTilesOnColumn : MaxNumberOfTilesOnColumn - 1);
            // Loop through the column. 
            for (int columnIndex = 0; columnIndex < numbserOfTiles; columnIndex++)
            {
                // Only the rows on odd index has walls and only if it is on the first or last position of the column.  
                isWall = (isEvenRow && (columnIndex == 0 || columnIndex == (numbserOfTiles - 1)) ? true : false);
                TileBase tile = ReuseTile(
                    // As long as this tile is not a wall, we can random choice a tile type for this new tile.
                    isWall ? TileTypes.Wall : RandomTileType(columnIndex),
                    isEvenRow ? new Vector3(columnIndex * TileBase.TileOffset - (TileBase.TileOffset / 2), 0, rowIndex * (TileBase.TileOffset / 2)) :
                               new Vector3(columnIndex * TileBase.TileOffset,  0, rowIndex * (TileBase.TileOffset / 2)),
                    TileBase.TileFixedQuaternion);

                // Initialize the new tile.
                tile.Init(rowIndex, columnIndex, SetTileColor(!isEvenRow, tile.GetTileType()));
#if UNITY_EDITOR
                // For debuging.
                tile.gameObject.name = tile.GetTileType().ToString() + " " + (isEvenRow ? "Even" : "Odd") + " [ " + rowIndex + ", " + columnIndex + " ]";
#endif
                AddTileIntoList(theRow, tile, columnIndex);
            }
            EnsureNextSafePathTile(isEvenRow, numbserOfTiles);
        }

        #region Tile collapse
        private void StartTileCollapse(int trackIndex) { 
            // StartCoroutine(TileCollapse(trackIndex));
        }

        private void OnTileCollapse(TileBase tile) {
            tile.Collapse();
            if (PlayerBoxController.PlayerInstance.PlayerLocation.Y == tile.CurrentLocation.Y) {
                // Hide the tile right under the player box so user can clear see how it falls.
                tile.EnableObject(false);
                // Stop further collapsing.
                m_KeepCollapsing = false;
                EventCenter.GameOverEvent(CauseOfGameOver.OnCollapsedTile);
            }
        }

        private void FixedUpdate() {
            if (m_KeepCollapsing) {
                m_Timer++;
                // If its time to collapse.
                if (m_Timer > m_TimeBetweenCollapse)
                {
                    // Check if player run too fast ahead.
                    if (m_CurrentCollapsingTrackCount < m_PlayerPassedTrackCount) {
                        // It should stop collapsing the old track since it will be 
                        // reuse for creating new track ahead, set the collapsing track count
                        // to current track which the player is on.
                        m_CurrentCollapsingTrackCount = m_PlayerPassedTrackCount;

                        // Also reset the collapse row index to ZERO
                        m_CollapseRowIndex = 0;
                    }
                    // Collapse a row.
                    foreach (var tile in m_TilePool[m_CurrentCollapsingTrackCount % 2, m_CollapseRowIndex++])
                    {
                        OnTileCollapse(tile);
                    }
                    // If a whole track is already collapsed.
                    if (m_CollapseRowIndex == LengthOfPreTrack) {
                        // Add one to the collapsing count.
                        m_CurrentCollapsingTrackCount++;
                        // Also reset the collapse row index to ZERO
                        m_CollapseRowIndex = 0;
                    }
                    // Reset the timer.
                    m_Timer = 0;
                }

            }
        }
        #endregion

        #endregion

        /// <summary>
        /// Get the specific tile in the track.
        /// </summary>
        /// <param name="rowIndex">The actul row index of the track.</param>
        /// <param name="columnIndex">The index on column.</param>
        /// <returns></returns>
        public TileBase GetTile(int rowIndex, int columnIndex)
        {
            // Check if the out if range
            if (rowIndex < 0 && (columnIndex <= 0 && columnIndex == MaxNumberOfTilesOnColumn)) return null;
            // Since there only 2 tracks exist, we have to find out witch one the player box is on.
            int trackIndex = (rowIndex % (LengthOfPreTrack * NumberOfPreGenerateTracks)) < LengthOfPreTrack ? 0 : 1;
            return m_TilePool[trackIndex, rowIndex % LengthOfPreTrack][columnIndex];
        }
    }
}


// #region Delegate and Events
//private void OnEnable()
//{
//    GameManager.PlayerMovedOnMapEvent += PlayerPositionUpdated;
//    GameManager.GameStartEvent += OnMapStartCollpase;
//    GameManager.GameOverEvent += OnGameOver;
//}

//private void OnDisable() {
//    GameManager.PlayerMovedOnMapEvent -= PlayerPositionUpdated;
//    GameManager.GameStartEvent -= OnMapStartCollpase;
//    GameManager.GameOverEvent -= OnGameOver;
//}
// #endregion

//#region Public varibales
// ---------- Public varibales -----------
//[Tooltip("The main there color of the whole map.")]
//public Color32 MapThemeColor = Color.white;
//public Color32 PlayerTraceColor = Color.white;

//public const int MaxNumberOfTilesOnRow = 7;
//public const int LengthOfMapChunk = 20;
//public const int NumberOfMapChunk = 2;
//#endregion

//#region Private variables
// ---------- Private variables ----------
//private GameObject m_MapTilePrefab = null;
//private GameObject m_WallTilePrefab = null;
//// Calualate the diagonal line across the tile qube, as long as the
//// tile is a square, the diagonal line's lenght should be (2^2 * (length of side)). 
//private static readonly float m_MapChunkOffset = LengthOfMapChunk * GameManager.TileOffset;
//private List<MapChunk> m_MapChunkList = new List<MapChunk>();

//private float m_CollapseSpeed = 0.1f;
//private int m_CollapsedRow = 0;
//private int m_CollapsingMapChunk = 0;
//private int m_TotalCollapedRowCount = 0;
//private bool m_KeepCollapsing = false;

//private int m_ChanceOfHole = 5;
//// private int m_ChanceOfSpike = 0;
//#endregion

//        #region Private methods
//        /// <summary>
//        /// Use this for initialization.
//        /// </summary>
//        public void InitMap() {
//            if (!m_MapTilePrefab) ResourcesLoader.Load("Tile_white", out m_MapTilePrefab, "Map");
//            if (!m_WallTilePrefab) ResourcesLoader.Load("Wall_white", out m_WallTilePrefab, "Map");

//            // Temp
//            PlayerTraceColor = Color.yellow;

//            CreateMapChunk();
//        }

//        /// <summary>
//        /// Create a chunk of game map.
//        /// The actul game map is combined by two chunk of map, each map chunck contents fix amount of tiles.
//        /// When player move near to the eage of one of map chunk, other chunk will reset its position so it can be
//        /// connect with current chunk of map.
//        /// Repeat this process everytime when the player get near the eage.
//        /// </summary>
//        private void CreateMapChunk() {
//            // Caluate the color of the wall.
//            Color32 wallColor = MapThemeColor.ChangeColorBrightness(-0.3f);

//            for (int chunkIndex = 0; chunkIndex < NumberOfMapChunk; chunkIndex++)
//            {
//                // Create a new gameObject as a root transform of the tile group.
//                GameObject chunkRoot = new GameObject();
//#if UNITY_EDITOR
//                // Change its name for easies bugging in editor.
//                chunkRoot.name = "MapChunkRoot " + chunkIndex;
//#endif
//                // Create a new chunk for store all the map tile data.
//                MapChunk chunk = new MapChunk();
//                // Notice that each loop creates two rows of tiles.
//                for (int rowIndex = 0; rowIndex < LengthOfMapChunk; rowIndex++)
//                {
//                    // Add tiles on the odd row.
//                    chunk.AddRowOfTiles(rowIndex, CreateTilesOnOddRow(chunkRoot.transform, MapThemeColor, wallColor, rowIndex, MaxNumberOfTilesOnRow));
//                    // Add tiles on the even row.
//                    chunk.AddRowOfTiles(rowIndex, CreateTilesOnEvenRow(chunkRoot.transform, MapThemeColor, rowIndex, MaxNumberOfTilesOnRow - 1));
//                }

//                // Reset its parent transform to the map manager for better management.
//                chunkRoot.transform.SetParent(this.transform);
//                // Set its offset position based on its index in the list.
//                chunkRoot.transform.position = new Vector3(0, 0, chunkIndex * m_MapChunkOffset);
//                // Apply the new create transform to this map chunk.
//                chunk.Root = chunkRoot.transform;
//                // Add all the datachunkRoot into the chunk.
//                m_MapChunkList.Add(chunk);
//            }

//            // Reset the last created map chunk so there migth be obstacles on it.
//            m_MapChunkList[m_MapChunkList.Count - 1].ResetTiles();
//        }

//        /// <summary>
//        /// Create tiles on the even rows.
//        /// </summary>
//        /// <param name="numberOfTilesOnColumn">The number of tiles on column.</param>
//        private List<MapTile> CreateTilesOnOddRow(Transform root, Color32 tileColor, Color32 wallColor, int rowIndex, int numberOfTilesOnColumn) {
//            List<MapTile> column = new List<MapTile>();
//            bool isWall = false;
//            for (int columnIndex = 0; columnIndex < numberOfTilesOnColumn; columnIndex++)
//            {
//                isWall = (columnIndex == 0 || columnIndex == (numberOfTilesOnColumn - 1)) ? true : false;
//                // Instantiate a new tile with position and rotatoin settings.
//                GameObject tile = Instantiate(
//                    // If current index is at the most left side of the map or most right side of the map, then its a wall.
//                    isWall ? m_WallTilePrefab : m_MapTilePrefab,
//                    // Set the tile position with offset.
//                    new Vector3(columnIndex * GameManager.TileOffset, 0, rowIndex * GameManager.TileOffset),
//                    // Change its rotation so it can facing the correct direction.
//                    Quaternion.Euler(GameManager.TileRotation)) as GameObject;

//                // Get the map tile script.
//                MapTile mapTile = tile.GetComponent<MapTile>();

//                mapTile.Init(isWall ? TileTypes.Wall : TileTypes.Floor);

//                // Remenber the local position, use it when reseting the tiles.
//                mapTile.OriginalLocalPosition = tile.transform.localPosition;

//                // Note: if using MeshRenderer but not Renderer, the color setting will affect
//                // on all the GameObjects which shares this same material.
//                foreach (var render in tile.GetComponentsInChildren<Renderer>())
//                {
//                    // Change the apperence depend on its type.
//                    render.material.color =
//                        (columnIndex == 0 || columnIndex == (numberOfTilesOnColumn - 1)) ? wallColor : tileColor;
//                    // Remenber its original color, it will comes in handy when the map reset itself.
//                    mapTile.OriginalUpperMeshColor = render.material.color;
//                }
//                // Reset it parent transform so when we want to move this chunk of tiles we only
//                // need to move this parent object.
//                tile.transform.SetParent(root);
//#if UNITY_EDITOR
//                tile.name = "Odd [ " + rowIndex * 2 + ", " + columnIndex + " ]";
//#endif
//                // Finaly, add these tiles into the list.
//                column.Add(mapTile);
//            }
//            return column;
//        }

//        /// <summary>
//        /// Create tiles on the odd rows.
//        /// </summary>
//        /// <param name="numberOfTilesOnColumn">The number of tiles on column.</param>
//        private List<MapTile> CreateTilesOnEvenRow(Transform root, Color32 tileColor, int rowIndex, int numberOfTilesOnColumn)
//        {
//            List<MapTile> column = new List<MapTile>();
//            // Get a lighter color for the tiles.
//            Color32 secondlyColor = tileColor.ChangeColorBrightness(-.2f);
//            for (int columnIndex = 0; columnIndex < numberOfTilesOnColumn; columnIndex++)
//            {
//                // Instantiate a new tile with position and rotatoin settings.
//                GameObject tile = Instantiate(
//                    // Since the even rows wont have a wall, just pass in the normal map tile.
//                    m_MapTilePrefab,
//                    new Vector3(
//                        columnIndex * GameManager.TileOffset + (GameManager.TileOffset / 2), 
//                        0, 
//                        rowIndex * GameManager.TileOffset + (GameManager.TileOffset / 2)),
//                    Quaternion.Euler(GameManager.TileRotation)) as GameObject;

//                MapTile mapTile = tile.GetComponent<MapTile>();

//                mapTile.Init(TileTypes.Floor);

//                // Note: if using MeshRenderer but not Renderer, the color setting will affect
//                // on all the GameObjects which shares this same material.
//                foreach (var render in tile.GetComponentsInChildren<Renderer>())
//                {
//                    // Like befer, Since the even rows wont have a wall, set the normal color.
//                    render.material.color = secondlyColor;
//                    mapTile.OriginalUpperMeshColor = render.material.color;
//                }
//                tile.transform.SetParent(root);
//                // Remenber the local position, use it when reseting the tiles.
//                mapTile.OriginalLocalPosition = tile.transform.localPosition;
//#if UNITY_EDITOR
//                tile.name = "Even [ " + (rowIndex * 2 + 1) + ", " + columnIndex + " ]";
//#endif
//                column.Add(mapTile);
//            }
//            return column;
//        }

//        /// <summary>
//        /// Called when player position updated.
//        /// </summary>
//        /// <param name="playerAtRow">The current player position on Row.</param>
//        /// <param name="playerAtColumn">The current player position on column.</param>
//        /// <param name="traceColor">The trace color that player will left behind.</param>
//        private void PlayerPositionUpdated(int playerAtRow, int playerAtColumn) {
//            // Leave a trace.
//            ChangeTileUpperMeshColor(playerAtRow, playerAtColumn, PlayerTraceColor);

//            // Check if the map needs update.
//            MapUpdateCheck(playerAtRow);
//        }

//        /// <summary>
//        /// Check if the player passed a specific point so that the map needs update itself
//        /// to create a endless map.
//        /// </summary>
//        /// <param name="playerPositionOnY">The current player location.</param>
//        private void MapUpdateCheck(int playerPositionOnY)
//        {
//            // First in sure the player get pass the first map chunck. 
//            if (playerPositionOnY > LengthOfMapChunk * 2)
//            {
//                // Then give it a offset of half of the map chunk (Once again, LengthOfMapChunk is actul the half
//                // of the map), everytime player pass a full chunk (LengthOfMapChunk * 2) call the 
//                // mapManager to generate new map ahead.
//                if ((playerPositionOnY + LengthOfMapChunk) % (LengthOfMapChunk * 2) == 0)
//                {
//                    // This means that player is now standing on the half of the map chunk
//                    ResetMapChunk(playerPositionOnY);
//                }
//            }
//        }

//        private void ChangeTileUpperMeshColor(int playerAtRow, int playerAtColumn, Color32 color)
//        {
//            GetTile(playerAtRow, playerAtColumn).UpperMesh.material.color =
//                playerAtRow % 2 != 0 ? color.ChangeColorBrightness(-0.3f) : color;
//        }

//        private void ResetMapChunk(int playerAtRow)
//        {
//            // First get the logical index (AKA. the N.th map chunk player has passed) of the map chunk.
//            int currentAtIndex = playerAtRow / (LengthOfMapChunk * 2);
//            // Then clamp ot into betweem 0 and total number of map chunls.
//            int currentChunkListIndex = currentAtIndex % NumberOfMapChunk;

//            int previousMapChunkIndex = currentChunkListIndex == 0 ? NumberOfMapChunk - 1 : currentChunkListIndex - 1;

//            // Reset the previous map change to further location
//            m_MapChunkList[previousMapChunkIndex].Root.position =
//                new Vector3(0, 0, (currentAtIndex + 1) * m_MapChunkOffset);

//            // Reset all the contents left in previous map chunk
//            ResetMapChunkContent(previousMapChunkIndex);
//        }

//        /// <summary>
//        /// Leave no trace behind.
//        /// </summary>
//        private void ResetMapChunkContent(int mapChunkIndexInList)
//        {
//            m_MapChunkList[mapChunkIndexInList].ResetTiles();

//            // Everytime the map chunk been reseted, obstacles 
//            // will have a higher chance to be generate. 
//            AddObstaclesGenerateChance();
//        }

//        private void OnMapStartCollpase() {
//            m_KeepCollapsing = true;
//            StartCoroutine(TileCollpase());
//        }

//        private IEnumerator TileCollpase() {
//            while (m_KeepCollapsing) {
//                m_TotalCollapedRowCount = m_CollapsedRow + (LengthOfMapChunk * 2 * m_CollapsingMapChunk);

//                if (GameManager.Player.PlayerOnY - m_TotalCollapedRowCount <= 5)
//                    m_CollapseSpeed = 0.18f;
//                else
//                    m_CollapseSpeed = 0.12f;


//                yield return new WaitForSeconds(m_CollapseSpeed);
//                // If current map chunk has not been reseted by player passing the reset point, 
//                // trigger the tiles to collapse.
//                // The reasome for doing this check is becuase some time the player go too fast,
//                // and the map chunk is not yet fully collapse befer reset to new location, after
//                // reset to the new location some of the tiles will keep been triggerd by this loop.
//                foreach (var tile in m_MapChunkList[m_CollapsingMapChunk].MapList[m_CollapsedRow])
//                {
//                    tile.StartCollapse();
//                    // Check if player is standing on the collapsed tile.
//                    if (GameManager.Player.PlayerOnY <= m_TotalCollapedRowCount) {
//                        // Hide the tile right under the player box so user can clear see how it falls.
//                        m_MapChunkList[m_CollapsingMapChunk].MapList[m_CollapsedRow][GameManager.Player.PlayerOnX].DisplayTile(false);

//                        GameManager.OnGameOver(CauseOfGameOver.OnCollapsedTile);
//                        yield return null;
//                    }
//                }

//                m_CollapsedRow++;

//                // If a map chunk is fully collpased.
//                if (m_CollapsedRow == LengthOfMapChunk * 2) {
//                    // Reset its state.
//                    m_CollapsingMapChunk++;
//                    m_CollapsingMapChunk = m_CollapsingMapChunk % NumberOfMapChunk;
//                    m_CollapsedRow = 0;
//                }
//            }
//        }

//        private void AddObstaclesGenerateChance() {
//            m_ChanceOfHole += 2;
//            if (m_ChanceOfHole >= 20) m_ChanceOfHole = 20;
//        }

//        private void OnGameOver(CauseOfGameOver cause)
//        {
//            // Stop tile from collapsing.
//            m_KeepCollapsing = false;
//            StopCoroutine(TileCollpase());
//        }
//        #endregion
//        #region Public methods
//        public MapTile GetTile(int playerAtRow, int playerAtColumn) {
//            // For example, if the length of the map chunk is 10 and the player is at (4, 6),
//            // so we check sreach for the tile where the player in standing on in first chunk of map, 
//            // if the player is at (12, 6) than we look into the second chunk of map.
//            // Since the map chunck will reposition itself once is fully out of screen, when player
//            // is at (22, 5) than the actul tile should be in the first list of chunk (2, 5).
//            // ------------------------------------------------------------------------------
//            // The index of the rows will always clamp betweem 0 and the length of per-chunk,
//            // and the the player at column will always between 0 and maximun number of tiles in a row.
//            return m_MapChunkList[(playerAtRow / (LengthOfMapChunk * 2)) % NumberOfMapChunk].GetTile(playerAtRow % (LengthOfMapChunk * 2), playerAtColumn);
//        }

//        public TileTypes GenerateObstacles()
//        {
//            int randomResult = Random.Range(0, 100);
//            if (m_ChanceOfHole > randomResult) {
//                return TileTypes.Hole;
//            }
//            return TileTypes.Floor;
//        }
//        #endregion
//    }
// }

