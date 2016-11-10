using UnityEngine;
using System.Collections;
using BoxDash.Utility;

namespace BoxDash.UI
{
    public class UIManager : Singleton<UIManager>
    {
        public enum UIs {
            Common = 0,
            MainMenu,
            Game
        }

        // Use this for initialization
        public void Init() {
            foreach (var ui in GetComponentsInChildren<SceneUIBase>())
            {
                ui.Init();
            }
        }
    }
}
