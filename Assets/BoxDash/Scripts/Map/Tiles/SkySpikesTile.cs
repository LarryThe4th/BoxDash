﻿using UnityEngine;
using BoxDash.Tile;
using System;

[RequireComponent(typeof(Animator))]
public class SkySpikesTile : TileBase
{
    #region Public varibales
    public Renderer UpperMesh;
    private Color32 OriginalColor;
    #endregion

    public override TileTypes GetTileType()
    {
        return TileTypes.SkySpikes;
    }

    public override void Init(int rowIndex, int columnIndex, Color32 tileColor)
    {
        base.Init(rowIndex, columnIndex, tileColor);
        // Reset the upper mesh's color.
        SetTileColor(tileColor);
        m_Animator.speed = 1;
        m_Animator.SetBool("Active", true);
    }

    public override void SetTileColor(Color32 tileColor)
    {
        base.SetTileColor(tileColor);
        OriginalColor = tileColor;
    }

    public override void OnObjectReuse(params object[] options)
    {
        base.OnObjectReuse(options);
        UpperMesh.material.color = OriginalColor;
    }

    public override void UseTile(params object[] options)
    {
        base.UseTile();
        UpperMesh.material.color = (Color32)(options[0]);
    }

    public override void Collapse()
    {
        base.Collapse();
        m_Animator.speed = 0;
    }
}
