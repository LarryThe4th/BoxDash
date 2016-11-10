using UnityEngine;
using BoxDash.Tile;

public class WallTile : TileBase
{
    public override TileTypes GetTileType()
    {
        return TileTypes.Wall;
    }

    public override void Init(int rowIndex, int columnIndex, Color32 tileColor)
    {
        base.Init(rowIndex, columnIndex, tileColor);
        SetTileColor(tileColor);
    }
}
