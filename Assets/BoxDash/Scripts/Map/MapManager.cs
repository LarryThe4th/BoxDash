using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using BoxDash.Tile;
using BoxDash.Player;
using BoxDash.Utility;
using System;

namespace BoxDash.Map
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
            EventCenter.GameOverEvent += OnGameOver;
            EventCenter.PlayerMovedOnMapEvent += OnPlayerMoved;
        }

        private void OnDisable()
        {
            EventCenter.GameStartEvent -= OnGameStarted;
            EventCenter.GameOverEvent -= OnGameOver;
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

        public static readonly Location2D PlayerRespawnLocation = new Location2D(3, 6);
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
        private int m_ChanceOfHole = 0;
        private const int m_ChanceIncreasePreUpdate_Hole = 3;
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

        #region Initiailzation
        /// <summary>
        /// Use this to init the map manager when game start.
        /// </summary>
        public void Init()
        {
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
            // TODO: i have to rework this part...now is look like shit..

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

        private void CreateObjectPool(GameObject prefab, int poolSize)
        {
            ObjectPoolManager.Instance.CreaterPool(
                prefab.GetComponent<TileBase>().GetTileType().ToString(),
                prefab.GetComponent<PoolObject>(),
                this.transform, poolSize);
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
        #endregion

        private void OnGameStarted()
        {
            m_KeepCollapsing = true;
        }

        private void OnGameOver(CauseOfGameOver cause)
        {
            m_KeepCollapsing = false;
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
