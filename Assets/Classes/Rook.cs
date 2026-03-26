using System;
using System.Collections.Generic;
using UnityEngine;

using static PieceManager;

public class Rook : Piece
{
    // ==============================================================================
    // constructor/Initializer
    // ==============================================================================
    override public bool Initialize(Tile tile, bool black)
    {
        if (!base.Initialize(tile, black))
        {
            moveableTiles = new List<Tile>(16);
            type = PieceManager.PieceType.Rook;
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
        int x, y, i;

        moveableTiles.Clear();
        foreach ((int _x, int _y) in move)
        {
            x = position.x;
            y = position.y;

            for (i = 0; i < 8; i++)
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
        if (x == X)
        {
            int i = Mathf.Min(y, Y) + 1;
            int max = Mathf.Max(y, Y);

            for (; i < max; i++)
            {
                if (board[x, i].type != PieceType.END)
                    return false;
            }

            return true;
        }
        else if (y == Y)
        {
            int i = Mathf.Min(x, X) + 1;
            int max = Mathf.Max(x, X);

            for (; i < max; i++)
            {
                if (board[i, y].type != PieceType.END)
                    return false;
            }
            return true;
        }
        return false;
    }

    // ==============================================================================
    // variables
    // ==============================================================================

    private static (int x, int y)[] move = new (int, int)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };
}
