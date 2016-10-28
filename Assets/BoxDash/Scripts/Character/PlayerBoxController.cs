using UnityEngine;
using BoxDash.Map;

namespace BoxDash.Player {
    /// <summary>
    /// This class controls the player box object.
    /// </summary>
    public class PlayerBoxController : MonoBehaviour
    {
        #region Private variables
        private enum MoveDirection {
            UpperLeft = 0,
            UpperRight,
        }

        // Keep tracking where the player is standing.
        private int m_PlayerOnY = 0;
        private int m_PlayerOnX = 0;
        #endregion

        public void Init(int onRow, int onColumn) {
            m_PlayerOnY = onColumn;
            m_PlayerOnX = onRow;

            SetPlayerLocation(m_PlayerOnY, m_PlayerOnX);

            this.transform.rotation = MapManager.Instance.GetTile(m_PlayerOnY, m_PlayerOnX).transform.rotation;
            MapManager.Instance.ChangeTileUpperMeshColor(m_PlayerOnY, m_PlayerOnX, Color.yellow);
        }

        private void MoveBoxAndCheckIfMapNeedsUpdate(MoveDirection direction, int unit = 2) {
            switch (direction) {
                case MoveDirection.UpperLeft:
                    for (int i = 0; i < unit; i++)
                    {
                        // Reach the left map border.
                        if (m_PlayerOnY % 2 != 0 && m_PlayerOnX == 0) break;
                        // Loop the index between 0 and LengthOfMapChunk * NumberOfMapChunk.
                        if (m_PlayerOnY % 2 != 0)
                        {
                            SetPlayerLocation(++m_PlayerOnY, m_PlayerOnX);
                        }
                        else {
                            SetPlayerLocation(++m_PlayerOnY, --m_PlayerOnX);
                        }
                        // Check if the map needs update.
                        MapUpdateCheck(m_PlayerOnY);

                        // Leave a trace on the player's path so the player can clearly see it.
                        MapManager.Instance.ChangeTileUpperMeshColor(m_PlayerOnY, m_PlayerOnX, Color.yellow);
                    } 
                    break;
                case MoveDirection.UpperRight:
                    for (int i = 0; i < unit; i++)
                    {
                        // Reach the right map border.  
                        if (m_PlayerOnY % 2 != 0 && m_PlayerOnX == MapManager.MaxNumberOfTilesOnRow - 2) break;
                        // Loop the index between 0 and LengthOfMapChunk * NumberOfMapChunk.
                        if (m_PlayerOnY % 2 != 0) {
                            SetPlayerLocation(++m_PlayerOnY, ++m_PlayerOnX);
                        }
                        else {
                            SetPlayerLocation(++m_PlayerOnY, m_PlayerOnX);
                        }
                        // Check if the map needs update.
                        MapUpdateCheck(m_PlayerOnY);

                        // Leave a trace on the player's path so the player can clearly see it.
                        MapManager.Instance.ChangeTileUpperMeshColor(m_PlayerOnY, m_PlayerOnX, Color.yellow);
                    }
                    break;
                default:
                    // No way this will get call...
                    SetPlayerLocation(m_PlayerOnY, m_PlayerOnX);
                    break;
            }
        }

        /// <summary>
        /// Check if the player passed a specific point so that the map needs update itself
        /// to create a endless map.
        /// </summary>
        /// <param name="playerPositionOnY">The current player location.</param>
        private void MapUpdateCheck(int playerPositionOnY) {
            // First in sure the player get pass the first map chunck. 
            if (playerPositionOnY > MapManager.LengthOfMapChunk * 2)
            {
                // Then give it a offset of half of the map chunk (Once again, LengthOfMapChunk is actul the half
                // of the map), everytime player pass a full chunk (LengthOfMapChunk * 2) call the 
                // mapManager to generate new map ahead.
                if ((playerPositionOnY + MapManager.LengthOfMapChunk) % (MapManager.LengthOfMapChunk * 2) == 0)
                {
                    // This means that player is now standing on the half of the map chunk
                    MapManager.OnCorssHalfOfTheMapChunck(playerPositionOnY);
                }
            }
        }

        private void SetPlayerLocation(int locationX, int locationY) {
            // Reset player box object location.
            this.transform.position = MapManager.Instance.GetTile(locationX, locationY).transform.position;
            // this.transform.position = MapManager.Instance.GetTile(locationX, locationY).transform.position;
            // Lift it up to the ground.
            this.transform.position += new Vector3(0, GameManager.TileSideLength / 2, 0);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                MoveBoxAndCheckIfMapNeedsUpdate(MoveDirection.UpperLeft);

            }
            if(Input.GetKeyDown(KeyCode.D))
            {
                MoveBoxAndCheckIfMapNeedsUpdate(MoveDirection.UpperRight);
            }
        }
    }
}
