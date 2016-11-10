using UnityEngine;
using System.Collections;
using BoxDash.Player;
using BoxDash.Utility;

namespace BoxDash
{
    public class PlayerInputHandler : Singleton<PlayerInputHandler>
    {
        #region Event
        private void OnEnable()
        {
            EventCenter.GameStartEvent += OnGameStart;
            EventCenter.GameOverEvent += OnGameOver;
        }

        private void OnDisable()
        {
            EventCenter.GameStartEvent -= OnGameStart;
            EventCenter.GameOverEvent -= OnGameOver;
        }

        public delegate void PlayerInput(Direction direction);
        public static PlayerInput PlayerInputEvent;
        public static void OnPlayerInput(Direction direction)
        {
            if (PlayerInputEvent != null && m_PlayerCanControl) PlayerInputEvent(direction);
        }
        #endregion

        public enum Direction
        {
            None = 0,
            UpperLeft,
            UpperRight,
        }
        private static bool m_PlayerCanControl = false;

        private void OnGameStart() {
            m_PlayerCanControl = true;
        }

        private void OnGameOver(CauseOfGameOver cause) {
            m_PlayerCanControl = false;
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        private void Update()
        {
            if (m_PlayerCanControl)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    OnPlayerInput(Direction.UpperLeft);
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    OnPlayerInput(Direction.UpperRight);
                }
            }
        }
#endif
    }
}
