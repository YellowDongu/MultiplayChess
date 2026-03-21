using System;
using System.Collections.Generic;
using UnityEngine;

abstract public class Piece : MonoBehaviour
{
    // ==============================================================================
    // constructor/Initializer
    // ==============================================================================

    virtual public bool Initialize(Tile tile, bool black)
    {
        bool output = initiated;
        BoardManager boardManager = GameMaster.GetInstance().GetBoard();

        if (!output)
        {
            if (boardManager == null)
            {
                Destroy(this.gameObject);
                Debug.Log("[ERROR]  NO BOARD IN THIS GAME!");
            }

            PieceManager pieceManager = boardManager.gameObject.GetComponent<PieceManager>();
            release = pieceManager.Release;
            GetTile = boardManager.GetTile;
            initiated = true;
        }

        boardManager.SetPiece(tile, this);
        isBlack = black;
        return output;
    }

    // ==============================================================================
    // Methods
    // ==============================================================================
    public bool Move(Tile tile)
    {
        if (tile == null)
            return false;

        if (CanMove(tile.GetPosition()))
        {
            position = tile.GetPosition();
            moveableTiles.Clear();
            gameObject.transform.position = tile.gameObject.transform.position;

            return true;
        }
        return false;
    }
    public bool CanMove(Vector2Int position) { return CanMove(GameMaster.GetInstance().GetBoard().GetTile(position)); }
    public bool CanMove(Tile tile)
    {
        foreach (Tile item in moveableTiles)
        {
            if (tile != item)
                continue;

            return true;
        }
        return false;
    }

    public void HighlightTile()
    {
        foreach (var item in moveableTiles)
            item.SetState(Tile.Status.Select);
    }
    public void DeHighlightTile()
    {
        foreach (var item in moveableTiles)
            item.SetState(Tile.Status.None);
    }

    abstract public void TileSearch();

    // ==============================================================================
    // variable GetSet Methods
    // ==============================================================================
    public Vector2Int GetPosition() { return position; }
    public void SetPosition(Tile tile) { position = tile.GetPosition(); }
    public void Release() { release(this); }

    // ==============================================================================
    // variables
    // ==============================================================================

    private bool initiated = false;
    public bool isBlack { get; protected set; }
    protected Vector2Int position;
    protected List<Tile> moveableTiles;
    public PieceManager.PieceType type { get; protected set; }

    protected delegate void ReleasePieceMethod(Piece component);
    protected ReleasePieceMethod release;
    protected delegate Tile GetTileMethod(int x, int y);
    protected GetTileMethod GetTile;

}
