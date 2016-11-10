using UnityEngine;
using BoxDash.Tile;
using BoxDash.Utility;
using Random = UnityEngine.Random;

namespace BoxDash.Player {
    /// <summary>
    /// This class controls the player box object.
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
    public class PlayerBoxController : MonoBehaviour
    {
        #region Delegate and Events
        private void OnEnable()
        {
            EventCenter.GameStartEvent += OnGameStart;
            EventCenter.GameOverEvent += OnGameOver;
            PlayerInputHandler.PlayerInputEvent += MoveBoxAndCheckIfMapNeedsUpdate;
        }

        private void OnDisable()
        {
            EventCenter.GameStartEvent -= OnGameStart;
            EventCenter.GameOverEvent -= OnGameOver;
            PlayerInputHandler.PlayerInputEvent -= MoveBoxAndCheckIfMapNeedsUpdate;
        }
        #endregion

        #region Public variables
        // ---------- Public variables ------------
        public static PlayerBoxController PlayerInstance;
        public Location2D PlayerLocation = new Location2D();
        #endregion

        #region Private variables
        // We gonna use it when the player fall off the map.
        private Rigidbody m_RigidBody;
        // private BoxCollider m_BoxCollider;
        // A flag of the player control.
        // The player box facing direction.
        private PlayerInputHandler.Direction m_CurrentFacing = PlayerInputHandler.Direction.None;
        private int m_MoveUnit = 1;
        #endregion

        public static void Init()
        {
            // Initzlie the player character.
            GameObject m_PlayerPrefab = null;
            ResourcesLoader.Load("PlayerBox", out m_PlayerPrefab);

            // Instantiate a new player character with position and rotatoin settings.
            GameObject player = Instantiate(
                m_PlayerPrefab,
                Vector3.zero,
                Quaternion.identity) as GameObject;

            PlayerInstance = player.GetComponent<PlayerBoxController>();
        }

        public void SetRespawnPosition(int onRowIndex, int onColumnIndex) {
            PlayerLocation.SetLocation(onColumnIndex, onRowIndex);

            // Shut off the player box's physics
            m_RigidBody = GetComponent<Rigidbody>();
            m_RigidBody.useGravity = false;
            m_RigidBody.velocity = Vector3.zero;
            // m_BoxCollider = GetComponent<BoxCollider>();
            this.transform.rotation = TileBase.TileFixedQuaternion;

            MovePlayer(0, 0);
        }

        private void OnGameStart() {
            // m_PlayerCanControl = true;
        }

        private void MoveBoxAndCheckIfMapNeedsUpdate(PlayerInputHandler.Direction direction) {
            switch (direction) {
                case PlayerInputHandler.Direction.UpperLeft:
                    for (int i = 0; i < m_MoveUnit; i++)
                    {
                        // Reach the left map border.
                        // if (PlayerLocation.Y % 2 != 0 && PlayerLocation.X == 0) break;
                        // Loop the index between 0 and LengthOfMapChunk * NumberOfMapChunk.
                        if (PlayerLocation.Y % 2 != 0)
                        {
                            MovePlayer(1, 0);
                        }
                        else {
                            MovePlayer(1, -1);
                        }
                    }
                    break;
                case PlayerInputHandler.Direction.UpperRight:
                    for (int i = 0; i < m_MoveUnit; i++)
                    {
                        // Reach the right map border.  
                        // if (PlayerLocation.Y % 2 != 0 && PlayerLocation.X == MapManager.Instance.GetMaximunTilesOnColnum - 2) break;
                        // Loop the index between 0 and LengthOfMapChunk * NumberOfMapChunk.
                        if (PlayerLocation.Y % 2 != 0) {
                            MovePlayer(1, 1);
                        }
                        else {
                            MovePlayer(1, 0);
                        }
                    }
                    break;
                default:
                    // No way this will get call...
                    MovePlayer(0, 0);
                    break;
            }
        }

        private void MovePlayer(int moveOnY, int moveOnX, PlayerInputHandler.Direction direction = PlayerInputHandler.Direction.None) {
            TileBase currentTile = MapManager.Instance.GetTile((PlayerLocation.Y + moveOnY), (PlayerLocation.X + moveOnX));
            // If the target tile is a wall or some kinda of trap that is blocking the path
            if (currentTile.CanPass == false || currentTile == null) return;
            // Reset player box object location.
            this.transform.position = new Vector3(
                currentTile.transform.position.x,
                // Lift it up to the ground.
                TileBase.TileSideLength / 2,
                currentTile.transform.position.z);

            if (direction != PlayerInputHandler.Direction.None) {
                // TODO
            }

            // Update player location
            PlayerLocation.Y += moveOnY;
            PlayerLocation.X += moveOnX;

            // Check if the map needs update.
            EventCenter.OnPlayerMoved(currentTile, this.transform.position);
        }

        private void OnGameOver(CauseOfGameOver cause) {
            switch (cause)
            {
                case CauseOfGameOver.OnCollapsedTile:
                    // m_PlayerCanControl = false;
                    m_RigidBody.useGravity = true;
                    m_RigidBody.angularVelocity = new Vector3(
                        Random.Range(0.0f, 1.0f),
                        Random.Range(0.0f, 1.0f),
                        Random.Range(0.0f, 1.0f)) * (Random.Range(1, 10));
                    break;
                case CauseOfGameOver.FallInHole:
                    // m_PlayerCanControl = false;
                    m_RigidBody.useGravity = true;
                    break;
                default:
                    break;
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (other.tag == "Trap") {
                TileBase murder = other.GetComponentInParent<TileBase>();
                if (murder.GetTileType() == TileTypes.FloorSpikes) EventCenter.OnGameOver(CauseOfGameOver.FloorSpikes);
                else if (murder.GetTileType() == TileTypes.SkySpikes) EventCenter.OnGameOver(CauseOfGameOver.SkySpikes);
            }
        }
    }
}
