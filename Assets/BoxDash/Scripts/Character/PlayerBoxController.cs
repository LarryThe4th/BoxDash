using UnityEngine;
using BoxDash.Map;
using Random = UnityEngine.Random;

namespace BoxDash.Player {
    /// <summary>
    /// This class controls the player box object.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerBoxController : MonoBehaviour
    {
        #region Delegate and Events
        private void OnEnable()
        {
            GameManager.GameOverEvent += OnGameOver;
        }

        private void OnDisable()
        {
            GameManager.GameOverEvent -= OnGameOver;
        }
        #endregion

        #region Public variables
        // ---------- Public variables ------------
        public int PlayerOnY {
            get;private set;
        }
        public int PlayerOnX
        {
            get; private set;
        }
        #endregion

        #region Private variables
        private enum MoveDirection {
            UpperLeft = 0,
            UpperRight,
        }

        private Rigidbody m_RigidBody;
        private bool m_PlayerCanControl = false;
        #endregion

        public void Init(int onRow, int onColumn, Quaternion rotation) {
            PlayerOnY = onColumn;
            PlayerOnX = onRow;

            m_RigidBody = GetComponent<Rigidbody>();
            m_RigidBody.useGravity = false;
            m_RigidBody.velocity = Vector3.zero;

            SetPlayerLocation(PlayerOnY, PlayerOnX);

            this.transform.rotation = rotation;

            GameManager.OnPlayerMoved(PlayerOnY, PlayerOnX, this.transform.position);

            // Temp
            GameManager.OnGameStart();

            m_PlayerCanControl = true;
        }

        private void MoveBoxAndCheckIfMapNeedsUpdate(MoveDirection direction, int unit = 1) {
            switch (direction) {
                case MoveDirection.UpperLeft:
                    for (int i = 0; i < unit; i++)
                    {
                        // Reach the left map border.
                        if (PlayerOnY % 2 != 0 && PlayerOnX == 0) break;
                        // Loop the index between 0 and LengthOfMapChunk * NumberOfMapChunk.
                        if (PlayerOnY % 2 != 0)
                        {
                            SetPlayerLocation(++PlayerOnY, PlayerOnX);
                        }
                        else {
                            SetPlayerLocation(++PlayerOnY, --PlayerOnX);
                        }
                        // Check if the map needs update.
                        GameManager.OnPlayerMoved(PlayerOnY, PlayerOnX, this.transform.position);
                    } 
                    break;
                case MoveDirection.UpperRight:
                    for (int i = 0; i < unit; i++)
                    {
                        // Reach the right map border.  
                        if (PlayerOnY % 2 != 0 && PlayerOnX == MapManager.MaxNumberOfTilesOnRow - 2) break;
                        // Loop the index between 0 and LengthOfMapChunk * NumberOfMapChunk.
                        if (PlayerOnY % 2 != 0) {
                            SetPlayerLocation(++PlayerOnY, ++PlayerOnX);
                        }
                        else {
                            SetPlayerLocation(++PlayerOnY, PlayerOnX);
                        }
                        // Check if the map needs update.
                        GameManager.OnPlayerMoved(PlayerOnY, PlayerOnX, this.transform.position);
                    }
                    break;
                default:
                    // No way this will get call...
                    SetPlayerLocation(PlayerOnY, PlayerOnX);
                    break;
            }
        }

        private void SetPlayerLocation(int locationX, int locationY) {
            MapTile currentTile = MapManager.Instance.GetTile(locationX, locationY);
            // Reset player box object location.
            this.transform.position = currentTile.transform.position;
            // this.transform.position = MapManager.Instance.GetTile(locationX, locationY).transform.position;
            // Lift it up to the ground.
            this.transform.position += new Vector3(0, GameManager.TileSideLength / 2, 0);

            // Whops
            if (currentTile.GetTileType == TileTypes.Hole) {
                GameManager.OnGameOver(CauseOfGameOver.FallInHole);
            }
        }

        private void OnGameOver(CauseOfGameOver cause) {
            switch (cause)
            {
                case CauseOfGameOver.OnCollapsedTile:
                    m_PlayerCanControl = false;
                    m_RigidBody.useGravity = true;
                    m_RigidBody.angularVelocity = new Vector3(
                        Random.Range(0.0f, 1.0f),
                        Random.Range(0.0f, 1.0f),
                        Random.Range(0.0f, 1.0f)) * (Random.Range(1, 10));
                    break;
                case CauseOfGameOver.FallInHole:
                    m_PlayerCanControl = false;
                    m_RigidBody.useGravity = true;
                    break;
                default:
                    m_PlayerCanControl = false;
                    break;
            }


        }

        private void Update()
        {
            if (m_PlayerCanControl) { 
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
}
