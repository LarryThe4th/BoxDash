using UnityEngine;
using UnityEngine.UI;
using System;

namespace BoxDash.UI
{
    public class CommonUI : SceneUIBase
    {
        #region Private varibales
        [SerializeField]
        private Text CurrencyCountText;
        #endregion

        public override UIManager.SceneUIs GetUIType()
        {
            return UIManager.SceneUIs.Common;
        }

        public override void Init()
        {
            base.Init();
        }
    }
}