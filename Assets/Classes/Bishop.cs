using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static PieceManager;

public class Bishop : Piece
{
    // ==============================================================================
    // constructor/Initializer
    // ==============================================================================
    override public bool Initialize(Tile tile, bool black)
    {
        if (!base.Initialize(tile, black))
        {
            moveableTiles = new List<Tile>(16);
            type = PieceManager.PieceType.Bishop;
        }

        TileSearch();
        return true;
    }

    // ==============================================================================
    // Methods
    // ==============================================================================

    override public void TileSearch()
    {
        Tile tile;
        int x, y;

        moveableTiles.Clear();
        foreach ((int _x, int _y) in move)
        {
            x = position.x;
            y = position.y;

            for (int i = 0; i < 8; i++)
            {
                x += _x;
                y += _y;

                tile = GetTile(x, y);
                if (tile == null)
                    break;
                if (tile.GetPiece() != null)
                {
                    if (tile.GetPiece().isBlack != isBlack)
                        moveableTiles.Add(tile);

                    break;
                }

                moveableTiles.Add(tile);
            }
        }

    }

    static public bool Validate((PieceType type, bool isBlack)[,] board, int x, int y, int X, int Y)
    {
        int _x = x, _y = y;
        int differenceX = X - x, differenceY = Y - y;
        int absX = Mathf.Abs(differenceX);

        if (absX != Mathf.Abs(differenceY))
            return false;
        differenceX = Math.Sign(differenceX);
        differenceY = Math.Sign(differenceY);

        for (int i = 1; i < absX; i++)
        {
            _x += differenceX;
            _y += differenceY;

            if (board[_x, _y].type != PieceType.END)
                return false;
        }

        return true;
    }

    // ==============================================================================
    // variables
    // ==============================================================================

    private static (int x, int y)[] move = new (int, int)[] { (1, 1), (1, -1), (-1, 1), (-1, -1) };
}
