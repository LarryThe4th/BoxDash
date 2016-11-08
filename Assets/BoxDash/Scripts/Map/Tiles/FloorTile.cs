using UnityEngine;
using BoxDash.Tile;
using System;

public class FloorTile : TileBase
{
    #region Public varibales
    public Renderer UpperMesh;
    private Color32 OriginalColor;
    #endregion

    public override TileTypes GetTileType()
    {
        return TileTypes.Floor;
    }

    public override void Init(Color32 tileColor)
    {
        OriginalColor = tileColor;
        foreach (var render in GetComponentsInChildren<Renderer>())
        {
            // Change the apperence depend on its type.
            render.material.color = tileColor;
        }
    }

    public override void OnObjectReuse(params object[] options)
    {
        EnableObject(true);
        // Reset upper mesh color.
        UpperMesh.material.color = OriginalColor;
    }
}
