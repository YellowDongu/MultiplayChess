using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

}
