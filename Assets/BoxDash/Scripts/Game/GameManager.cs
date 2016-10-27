using UnityEngine;
using BoxDash.Map;
using BoxDash.Player;
using BoxDash.Utility;

namespace BoxDash {
    /// <summary>
    /// The root class of the whole game.
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        public static readonly float TileSideLength = 0.254f;
        private PlayerBoxController m_Player;

        // Use this for initialization
        private void Start()
        {
            // Initialize the game map.
            MapManager.Instance.InitMap();

            // Initialize the player object.
            m_Player = CreateCharacter();
            m_Player.Init();
        }

        private PlayerBoxController CreateCharacter() {
            // Initzlie the player character.
            GameObject m_PlayerPrefab = null;
            ResourcesLoader.Load("playerBox", out m_PlayerPrefab);

            // Instantiate a new player character with position and rotatoin settings.
            GameObject player = Instantiate(
                m_PlayerPrefab,
                MapManager.Instance.GetPlayerRespawnLocation.position,
                MapManager.Instance.GetPlayerRespawnLocation.rotation) as GameObject;

            return player.GetComponent<PlayerBoxController>();
        }


    }

}
