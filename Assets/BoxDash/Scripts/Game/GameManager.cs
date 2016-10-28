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
        #region Public varibales
        public static readonly float TileSideLength = 0.254f;
        public static readonly float TileOffset = Mathf.Sqrt(2) * GameManager.TileSideLength;
        public static readonly Vector3 TileRotation = new Vector3(-90, 45, 0);
        #endregion

        public static void OnPlayerMoved(int playerAtRow, int playerAtColumn, Vector3 position)
        {
            if (MapManager.PlayerMovedOnMapEvent != null) MapManager.PlayerMovedOnMapEvent(playerAtRow, playerAtColumn);
            if (CameraController.PlayerMovedInWorldSpaceEvent != null) CameraController.PlayerMovedInWorldSpaceEvent(position);
        }

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

            player.GetComponent<PlayerBoxController>().Init(2, 4, Quaternion.Euler(TileRotation));
            return player.GetComponent<PlayerBoxController>();
        }
    }

}
