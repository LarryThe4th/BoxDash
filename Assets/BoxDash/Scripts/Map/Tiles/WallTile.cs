using UnityEngine;
using BoxDash.Tile;
using System;

public class WallTile : TileBase
{
    public override TileTypes GetTileType()
    {
        return TileTypes.Wall;
    }

    public override void Init(Color32 tileColor)
    {
        foreach (var render in GetComponentsInChildren<Renderer>())
        {
            // Change the apperence depend on its type.
            render.material.color = tileColor;
        }
    }

    public override void OnObjectReuse(params object[] options)
    {
        EnableObject(true);
    }
}
