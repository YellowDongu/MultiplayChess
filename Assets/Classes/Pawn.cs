using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static PieceManager;

public class Pawn : Piece
{
    // ==============================================================================
    // constructor/Initializer
    // ==============================================================================
    override public bool Initialize(Tile tile, bool black)
    {
        if (!base.Initialize(tile, black))
        {
            moveableTiles = new List<Tile>(4);
            type = PieceManager.PieceType.Pawn;
        }

        TileSearch();
        return true;
    }

    // ==============================================================================
    // Methods
    // ==============================================================================

    override public void TileSearch()
    {
        moveableTiles.Clear();
        if (isBlack)
        {
            for (int i = 1; i < 3; i++)
            {
                Tile tile = GetTile(position.x, position.y + i);
                if (tile == null || tile.GetPiece() != null)
                    break;

                moveableTiles.Add(tile);
            }

            TileSearch(position.x + 1, position.y + 1);
            TileSearch(position.x - 1, position.y + 1);
        }
        else
        {
            for (int i = 1; i < 3; i++)
            {
                Tile tile = GetTile(position.x, position.y - i);
                if (tile == null || tile.GetPiece() != null)
                    break;

                moveableTiles.Add(tile);
            }

            TileSearch(position.x + 1, position.y - 1);
            TileSearch(position.x - 1, position.y - 1);
        }
    }

    private void TileSearch(int x, int y)
    {
        Tile tile = GetTile(x, y);
        if (tile == null)
            return;
        if (tile.GetPiece() == null || tile.GetPiece().isBlack == isBlack)
            return;
        moveableTiles.Add(tile);
    }

    static public bool Validate((PieceType type, bool isBlack)[,] board, int x, int y, int X, int Y)
    {
        int differnce = Mathf.Abs(y - Y);
        if (x != X)
        {
            if (differnce > 1 || Mathf.Abs(x - X) > 1)
                return false;

            return board[X, Y].type != PieceType.END;
        }

        if (differnce > 2)
            return false;

        switch (differnce)
        {
            case 1:
                return board[X, Y].type == PieceType.END;
            case 2:
                return (board[X, Y + ((Y - y) / 2)].type == PieceType.END) && (board[X, Y].type == PieceType.END);
            default:
                return false;
        }
    }

}
