using System.Collections.Generic;

public class Knight : Piece
{
    // ==============================================================================
    // constructor/Initializer
    // ==============================================================================
    override public bool Initialize(Tile tile, bool black)
    {
        if (!base.Initialize(tile, black))
        {
            moveableTiles = new List<Tile>(8);
            type = PieceManager.PieceType.Knight;
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

    // ==============================================================================
    // variables
    // ==============================================================================

    private static (int x, int y)[] move = new (int, int)[] { (1, 2), (1, -2), (-1, 2), (-1, -2), (2, 1), (2, -1), (-2, 1), (-2, -1) };

}
