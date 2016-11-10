using UnityEngine;
using BoxDash.Utility;
using BoxDash.Player;
using BoxDash.SceneCamera;
using BoxDash.UI;

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

            // Initialize the game UIs.
            UIManager.Instance.Init();

            // Reset the camera position.
            CameraController.Instance.Init();

            // Initialize the player object.
            PlayerBoxController.Init();
            PlayerBoxController.PlayerInstance.SetRespawnPosition(6, 3);

        }

        //public void PlayGame() {
        //    EventCenter.OnGameStart();
        //}
    }
}
