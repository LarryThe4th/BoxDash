using UnityEngine;
using BoxDash.Map;
using BoxDash.Player;
using BoxDash.Utility;
using BoxDash.SceneCamera;

namespace BoxDash {
    /// <summary>
    /// The root class of the whole game.
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        #region Delegate and Events
        public delegate void PlayerMovedInWorldSpace(Vector3 position);
        public static PlayerMovedInWorldSpace PlayerMovedInWorldSpaceEvent;

        public delegate void PlayerMovedOnMap(int playerAtRow, int playerAtColumn);
        public static PlayerMovedOnMap PlayerMovedOnMapEvent;

        public delegate void OnEvent();
        public static OnEvent GameStartEvent;
        public static void OnGameStart() {
            if (GameStartEvent != null) GameStartEvent();
        }

        public static OnEvent GameOverEvent;
        public static void OnGameOver()
        {
            if (GameOverEvent != null) GameOverEvent();
        }

        public static void OnPlayerMoved(int playerAtRow, int playerAtColumn, Vector3 position)
        {
            if (PlayerMovedOnMapEvent != null) PlayerMovedOnMapEvent(playerAtRow, playerAtColumn);
            if (PlayerMovedInWorldSpaceEvent != null) PlayerMovedInWorldSpaceEvent(position);
        }
        #endregion


        #region Public varibales
        public static readonly float TileSideLength = 0.254f;
        public static readonly float TileOffset = Mathf.Sqrt(2) * GameManager.TileSideLength;
        public static readonly Vector3 TileRotation = new Vector3(-90, 45, 0);
        #endregion

        // Use this for initialization
        private void Start()
        {
            // Initialize the game map.
            MapManager.Instance.InitMap();

            CameraController.Instance.Init();

            // Initialize the player object.
            CreateCharacter();
        }

        private PlayerBoxController CreateCharacter() {
            // Initzlie the player character.
            GameObject m_PlayerPrefab = null;
            ResourcesLoader.Load("PlayerBox", out m_PlayerPrefab);

            // Instantiate a new player character with position and rotatoin settings.
            GameObject player = Instantiate(
                m_PlayerPrefab,
                Vector3.zero,
                Quaternion.identity) as GameObject;

            player.GetComponent<PlayerBoxController>().Init(3, 6, Quaternion.Euler(TileRotation));
            return player.GetComponent<PlayerBoxController>();
        }
    }

}
