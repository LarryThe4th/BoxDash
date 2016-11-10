﻿using UnityEngine;
using BoxDash.Tile;
using BoxDash.Player;
using BoxDash.Utility;
using BoxDash.SceneCamera;

namespace BoxDash {
    public enum CauseOfGameOver {
        OnCollapsedTile = 0,
        FallInHole
    }
    /// <summary>
    /// The root class of the whole game.
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        #region Delegate and Events

        #endregion

        #region Public varibales
        public static PlayerBoxController PlayerBox;
        #endregion

        // Use this for initialization
        private void Start()
        {
            // Initialize the game map.
            MapManager.Instance.Init();

            CameraController.Instance.Init();

            // Initialize the player object.
            if (!PlayerBox) PlayerBox = CreateCharacter();
            PlayerBox.Init(4, 3);

            EventCenter.OnGameStart();
        }

        private PlayerBoxController CreateCharacter()
        {
            // Initzlie the player character.
            GameObject m_PlayerPrefab = null;
            ResourcesLoader.Load("PlayerBox", out m_PlayerPrefab);

            // Instantiate a new player character with position and rotatoin settings.
            GameObject player = Instantiate(
                m_PlayerPrefab,
                Vector3.zero,
                Quaternion.identity) as GameObject;

            return player.GetComponent<PlayerBoxController>();
        }
    }
}
