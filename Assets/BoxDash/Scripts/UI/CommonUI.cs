using UnityEngine;
using UnityEngine.UI;
using BoxDash.Score;

namespace BoxDash.UI
{
    public class CommonUI : SceneUIBase
    {
        #region Event
        private void OnEnable()
        {
            EventCenter.PlayerPickUpItemEvent += OnPlayerPickupPoints;
        }

        private void OnDisable()
        {
            EventCenter.PlayerPickUpItemEvent -= OnPlayerPickupPoints;
        }
        #endregion


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
            CurrencyCountText.text = ScoreManager.Instance.GetData(ScoreManager.ScoreTypes.Credit).ToString();
        }

        private void OnPlayerPickupPoints() {
            CurrencyCountText.text = ScoreManager.Instance.GetData(ScoreManager.ScoreTypes.Credit).ToString();
        }
    }
}