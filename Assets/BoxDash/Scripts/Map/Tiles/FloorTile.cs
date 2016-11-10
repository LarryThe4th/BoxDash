﻿using UnityEngine;
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

    public override void Init(int rowIndex, int columnIndex, Color32 tileColor)
    {
        base.Init(rowIndex, columnIndex, tileColor);
        SetTileColor(tileColor);
    }

    public override void SetTileColor(Color32 tileColor)
    {
        base.SetTileColor(tileColor);
        OriginalColor = tileColor;
    }

    public override void OnObjectReuse(params object[] options)
    {
        base.OnObjectReuse(options);
        // Reset upper mesh color.
        UpperMesh.material.color = OriginalColor;
    }

    public override void UseTile(params object[] options)
    {
        base.UseTile();
        UpperMesh.material.color = (Color32)(options[0]);
    }
}
