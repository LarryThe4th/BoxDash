using UnityEngine;
using System.Collections;
using BoxDash.Utility;
using BoxDash.Tile;

namespace BoxDash
{
    public class EventCenter
    {
        public delegate void PlayerMovedInWorldSpace(Vector3 position);
        public static PlayerMovedInWorldSpace PlayerMovedInWorldSpaceEvent;

        public delegate void PlayerMovedOnMap(TileBase tile);
        public static PlayerMovedOnMap PlayerMovedOnMapEvent;

        public static void OnPlayerMoved(TileBase tile, Vector3 position)
        {
            if (PlayerMovedOnMapEvent != null) PlayerMovedOnMapEvent(tile);
            if (PlayerMovedInWorldSpaceEvent != null) PlayerMovedInWorldSpaceEvent(position);
        }

        public delegate void GameOver(CauseOfGameOver cause);
        public static GameOver GameOverEvent;
        public static void OnGameOver(CauseOfGameOver cause)
        {
            if (GameOverEvent != null) GameOverEvent(cause);
        }

        public delegate void OnEvent();
        public static OnEvent GameStartEvent;
        public static void OnGameStart()
        {
            if (GameStartEvent != null) GameStartEvent();
        }
    }
}
