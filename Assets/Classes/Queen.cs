using System;
using System.Collections.Generic;
using UnityEngine;
using static PieceManager;

public class Queen : Piece
{
    // ==============================================================================
    // constructor/Initializer
    // ==============================================================================
    override public bool Initialize(Tile tile, bool black)
    {
        if (!base.Initialize(tile, black))
        {
            moveableTiles = new List<Tile>(32);
            type = PieceManager.PieceType.Queen;
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
        if (x == X)
        {
            int i = Mathf.Min(y, Y);
            i += Math.Sign(Y - y);

            for (; i < Y; i++)
            {
                if (board[x, i].type != PieceType.END)
                    return false;
            }

            return true;
        }
        else if (y == Y)
        {
            int i = Mathf.Min(x, X);
            i += Math.Sign(X - x);

            for (; i < X; i++)
            {
                if (board[i, y].type != PieceType.END)
                    return false;
            }
            return true;
        }

        else return Bishop.Validate(board, x, y, X, Y);
    }

    // ==============================================================================
    // variables
    // ==============================================================================

    private static (int x, int y)[] move = new (int, int)[] { (1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (1, -1), (-1, 1), (-1, -1) };
}
