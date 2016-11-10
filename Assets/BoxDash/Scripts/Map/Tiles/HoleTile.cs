using UnityEngine;
using BoxDash.Tile;
using System;

public class HoleTile : TileBase
{
    public override TileTypes GetTileType()
    {
        return TileTypes.Hole;
    }
}
