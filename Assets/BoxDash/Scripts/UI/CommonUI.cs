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

        public override UIManager.UIs GetUIType()
        {
            return UIManager.UIs.Common;
        }

        public override void Init()
        {
            base.Init();
        }

        public override void HideUI()
        {
            // Empty
        }

        public override void ShowUI()
        {
            // Empty
        }
    }
}