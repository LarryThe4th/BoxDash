using UnityEngine;
using System.Collections;
using BoxDash.Utility;

public class EventCenter {
    public delegate void PlayerMovedInWorldSpace(Vector3 position);
    public static PlayerMovedInWorldSpace PlayerMovedInWorldSpaceEvent;

    public delegate void PlayerMovedOnMap(int playerAtRow, int playerAtColumn);
    public static PlayerMovedOnMap PlayerMovedOnMapEvent;

    public static void OnPlayerMoved(int playerAtRow, int playerAtColumn, Vector3 position)
    {
        if (PlayerMovedOnMapEvent != null) PlayerMovedOnMapEvent(playerAtRow, playerAtColumn);
        if (PlayerMovedInWorldSpaceEvent != null) PlayerMovedInWorldSpaceEvent(position);
    }

}
