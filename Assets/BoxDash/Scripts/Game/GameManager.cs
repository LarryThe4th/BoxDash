using UnityEngine;
using BoxDash.Utility;
using BoxDash.SceneCamera;

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
        #region Delegate and Events

        #endregion

        // Use this for initialization
        private void Start()
        {
            // Initialize the game map.
            MapManager.Instance.Init();

            CameraController.Instance.Init();

            // Initialize the player object.
            PlayerBoxController.Init();
            PlayerBoxController.PlayerInstance.SetRespawnPosition(6, 3);

            EventCenter.OnGameStart();
        }
    }
}
