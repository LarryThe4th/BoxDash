using UnityEngine;
using BoxDash.Map;
using BoxDash.UI;
using BoxDash.SceneCamera;
using BoxDash.Score;
using BoxDash.Player;
using BoxDash.Utility;
using System;

namespace BoxDash {
    public enum CauseOfGameOver {
        OnCollapsedTile = 0,
        FallInHole,
        SkySpikes,
        FloorSpikes,
    }

    /// <summary>
    /// The root class of the whole game.
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        // Use this for initialization
        private void Start()
        {
            Init();
        }

        public void Init()
        {
            // Initialize the game map.
            MapManager.Instance.Init();

            // Initialize the player object.
            PlayerBoxController.Init();

            PlayerBoxController.PlayerInstance.Respawn(MapManager.PlayerRespawnLocation);

            ScoreManager.Instance.Init(PlayerBoxController.GetPlayerName);

            // Initialize the game UIs.
            UIManager.Instance.Init();

            // Reset the camera position.
            CameraController.Instance.Init();
        }
    }
}
