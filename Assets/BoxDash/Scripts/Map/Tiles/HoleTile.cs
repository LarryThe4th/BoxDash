using UnityEngine;
using BoxDash.Tile;
using System;

public class HoleTile : TileBase
{
    public override TileTypes GetTileType()
    {
        return TileTypes.Hole;
    }

    public override void Init(Color32 tileColor)
    {
        // Empty
    }

    public override void OnObjectReuse(params object[] options)
    {
        // Empty
    }
}
