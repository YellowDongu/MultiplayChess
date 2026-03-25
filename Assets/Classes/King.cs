using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static PieceManager;

public class King : Piece
{
    // ==============================================================================
    // constructor/Initializer
    // ==============================================================================
    override public bool Initialize(Tile tile, bool black)
    {
        if (!base.Initialize(tile, black))
        {
            moveableTiles = new List<Tile>(8);
            type = PieceManager.PieceType.King;
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
            x = position.x + _x;
            y = position.y + _y;
            tile = GetTile(x, y);
            if (tile == null)
                continue;

            if (tile.GetPiece() != null && tile.GetPiece().isBlack == isBlack)
                continue;

            moveableTiles.Add(tile);
        }
    }

    static public bool Validate((PieceType type, bool isBlack)[,] board, int x, int y, int X, int Y)
    {
        return (Mathf.Abs(x - X) <= 1) && (Mathf.Abs(y - Y) <= 1);
    }

    // ==============================================================================
    // variables
    // ==============================================================================

    private static (int x, int y)[] move = new (int, int)[] { (1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (1, -1), (-1, 1), (-1, -1) };
}
