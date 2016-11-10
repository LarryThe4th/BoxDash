using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

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

        public override UIManager.UIs GetUIType()
        {
            return UIManager.UIs.MainMenu;
        }

        public override void Init()
        {
            base.Init();
#if UNITY_EDITOR
            if (!PlayGameButton) Debug.Log("PlayGameButton not set.");
            if (!SoundButton) Debug.Log("SoundButton not set.");
            if (!HelpButton) Debug.Log("HelpButton not set.");
#endif
            // PlayGameButton.
            // throw new NotImplementedException();
        }

        private void OpeningSequence() {
        }

        public override void HideUI()
        {
            m_Animator.SetBool("HideMainMenu", true);
        }

        public override void ShowUI()
        {
            m_Animator.SetBool("HideMainMenu", false);
        }
    }
}
