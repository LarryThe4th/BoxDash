using UnityEngine;
using UnityEngine.UI;

namespace BoxDash.UI
{
    public class MainMenuUI : SceneUIBase
    {
        #region Private variable
        [SerializeField]
        private Button PlayGameButton;
        [SerializeField]
        private Button SoundButton;
        [SerializeField]
        private Button HelpButton;
        #endregion

        public override UIManager.SceneUIs GetUIType()
        {
            return UIManager.SceneUIs.MainMenu;
        }

        public override void Init()
        {
            base.Init();
#if UNITY_EDITOR
            if (!PlayGameButton) Debug.Log("PlayGameButton not set.");
            if (!SoundButton) Debug.Log("SoundButton not set.");
            if (!HelpButton) Debug.Log("HelpButton not set.");
#endif
            PlayGameButton.onClick.AddListener(delegate { OnPressPlayGameButton(); });
            m_IsDisplaying = false;
        }

        private void OnPressPlayGameButton() {
            UIManager.Instance.SwitchSceneUI(UIManager.SceneUITransition.MainMenuToGame);
            GameManager.Instance.ResetGame(false, false);
        }

        private void OpeningSequence() {
            // Empty
        }

        public override void HideUI()
        {
            base.HideUI();
            m_Animator.SetBool("HideMainMenu", true);
        }

        public override void ShowUI()
        {
            base.ShowUI();
            m_Animator.SetBool("HideMainMenu", false);
        }
    }
}
