using UnityEngine;
using UnityEngine.UI;

namespace BoxDash.UI {
    public class GameUI : SceneUIBase
    {
        #region Public variables
        public override UIManager.UIs GetUIType()
        {
            return UIManager.UIs.Game;
        }
        #endregion

        #region Private varibales
        [SerializeField]
        private Button LeftButton;
        [SerializeField]
        private Button RightButton;
        #endregion

        public override void Init()
        {
            base.Init();
#if UNITY_EDITOR
            if (!LeftButton) Debug.Log("The left button not set.");
            if (!RightButton) Debug.Log("The right button not set.");
#endif
            LeftButton.onClick.AddListener(delegate { PlayerPressLeft(); });
            RightButton.onClick.AddListener(delegate { PlayerPressRight(); });
        }

        public void PlayerPressLeft()
        {
            Debug.Log("Here");
            PlayerInputHandler.OnPlayerInput(PlayerInputHandler.Direction.UpperLeft);
        }

        public void PlayerPressRight() {
            Debug.Log("Here");
            PlayerInputHandler.OnPlayerInput(PlayerInputHandler.Direction.UpperRight);
        }

        public override void HideUI() {
        }

        public override void ShowUI() {
        }
    }

}
