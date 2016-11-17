using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BoxDash.Tile;
using BoxDash.Map;
using BoxDash.Score;

namespace BoxDash.UI {
    public class GameUI : SceneUIBase
    {
        #region Event
        private void OnEnable()
        {
            EventCenter.GameOverEvent += OnGameOver;
            EventCenter.StartGameCountDownEvent += StartCountDown;
            EventCenter.PlayerMovedOnMapEvent += UpdateDistanceScore;
        }

        private void OnDisable()
        {
            EventCenter.GameOverEvent -= OnGameOver;
            EventCenter.StartGameCountDownEvent -= StartCountDown;
            EventCenter.PlayerMovedOnMapEvent -= UpdateDistanceScore;
        }
        #endregion

        #region Public variables
        public override UIManager.SceneUIs GetUIType()
        {
            return UIManager.SceneUIs.Game;
        }
        #endregion

        #region Private varibales
        [SerializeField]
        private Button LeftButton;
        [SerializeField]
        private Button RightButton;
        [SerializeField]
        private Text CountDownText;
        [SerializeField]
        private Text DistanceText;
        private int m_CurrentDistance = 0;
        [SerializeField]
        private Text NewRecordNoitfication;

        [SerializeField]
        private Button PauseButton;

        [SerializeField]
        private Button RestartGameButton;
        [SerializeField]
        private Button BackToMainMenuButton;


        private const int m_MaximunCountDown = 5;
        private int m_CountDown = m_MaximunCountDown;
        #endregion

        public override void Init()
        {
            base.Init();
#if UNITY_EDITOR
            if (!LeftButton) Debug.Log("The left button not set.");
            if (!RightButton) Debug.Log("The right button not set.");
            if (!CountDownText) Debug.Log("The CountDown Text not set.");
            if (!DistanceText) Debug.Log("The Distance Text not set.");
            if (!NewRecordNoitfication) Debug.Log("The NewRecordNoitfication Text not set.");
            if (!RestartGameButton) Debug.Log("The RestartGame Button not set.");
            if (!BackToMainMenuButton) Debug.Log("The RestartGame Button not set.");
#endif
            LeftButton.onClick.AddListener(delegate { PlayerPressLeft(); });
            RightButton.onClick.AddListener(delegate { PlayerPressRight(); });
            CountDownText.text = m_MaximunCountDown.ToString();
            m_CurrentDistance = 0;
            DistanceText.text = m_CurrentDistance.ToString();
            NewRecordNoitfication.enabled = false;

            RestartGameButton.onClick.AddListener(delegate { OnPressRestartGame(); });
            BackToMainMenuButton.onClick.AddListener(delegate { OnPressBackToMainMenu(); });
        }

        public void PlayerPressLeft()
        {
            PlayerInputHandler.OnPlayerInput(PlayerInputHandler.Direction.UpperLeft);
        }

        public void PlayerPressRight()
        {
            PlayerInputHandler.OnPlayerInput(PlayerInputHandler.Direction.UpperRight);
        }

        public override void HideUI()
        {
            base.HideUI();
            m_Animator.SetBool("HitNewRecord", false);
            m_Animator.SetBool("ShowGameUI", false);
        }

        public override void ShowUI()
        {
            base.ShowUI();
            m_Animator.SetBool("ShowGameUI", true);
            StartCountDown();
        }

        private void OnGameOver(CauseOfGameOver cause)
        {
            m_Animator.SetBool("ShowGameOver", true);
        }

        private void OnPressRestartGame() {
            m_Animator.SetBool("ShowGameOver", false);
            // Reset
            m_CurrentDistance = 0;
            m_Animator.SetBool("HitNewRecord", false);
            GameManager.Instance.ResetGame(false);
        }

        private void OnPressBackToMainMenu() {
            m_Animator.SetBool("ShowGameOver", false);
            GameManager.Instance.ResetGame();
            UIManager.Instance.SwitchSceneUI(UIManager.SceneUITransition.GameToMainMenu);
        }

        public void UpdateDistanceScore(TileBase tile) {
            if (!m_IsDisplaying) return;
            m_CurrentDistance = tile.CurrentLocation.Y - MapManager.PlayerRespawnLocation.Y;
            if (m_CurrentDistance == ScoreManager.Instance.GetData(ScoreManager.ScoreTypes.MaxDistance) + 1) {
                ShowNewRecordNoitfication();
            }
            DistanceText.text = (m_CurrentDistance).ToString();
        }

        private void ShowNewRecordNoitfication() {
            m_Animator.SetBool("HitNewRecord", true);
        }

        #region Count Down
        public void StartCountDown() {
            CountDownText.text = m_MaximunCountDown.ToString();
            StartCoroutine(CountDown());
        }

        private IEnumerator CountDown()
        {
           // yield return new WaitForSeconds(1);
            m_Animator.SetBool("ShowCountDown", true);
            yield return new WaitForSeconds(1);
            while (m_CountDown-- > 0) {
                CountDownText.text = m_CountDown <= 0 ? "GO!" : m_CountDown.ToString();
                m_Animator.SetTrigger("CountDownPop");
                yield return new WaitForSeconds(1);
            }
            m_CountDown = m_MaximunCountDown;
            m_Animator.SetBool("ShowCountDown", false);
            EventCenter.OnGameStart();
        }
        #endregion
    }

}
