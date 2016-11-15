using UnityEngine;
using System.Collections.Generic;
using BoxDash.Utility;
using System.Linq;

namespace BoxDash.UI
{
    public class UIManager : Singleton<UIManager>
    {
        public enum SceneUITransition {
            MainMenuToGame,
            GameToMainMenu,
        }

        public enum SceneUIs {
            Common = 0,
            MainMenu,
            Game
        }

        #region Private variables
        private List<SceneUIBase> m_SceneUIs = new List<SceneUIBase>();
        #endregion

        // Use this for initialization
        public void Init() {
            m_SceneUIs = GetComponentsInChildren<SceneUIBase>().ToList();
            m_SceneUIs.ForEach(ui => ui.Init());
        }

        public void SwitchSceneUI(SceneUITransition transition) {
            switch (transition) {
                case SceneUITransition.MainMenuToGame:
                    foreach (var ui in GetComponentsInChildren<SceneUIBase>())
                    {
                        if (ui.GetUIType() == SceneUIs.MainMenu) ui.HideUI();
                        if (ui.GetUIType() == SceneUIs.Game) ui.ShowUI();
                    }
                    break;
                case SceneUITransition.GameToMainMenu:
                    foreach (var ui in GetComponentsInChildren<SceneUIBase>())
                    {
                        if (ui.GetUIType() == SceneUIs.MainMenu) ui.ShowUI();
                        if (ui.GetUIType() == SceneUIs.Game) ui.HideUI();
                    }
                    break;
            }
        }
    }
}
