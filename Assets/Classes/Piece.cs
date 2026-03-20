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
        if (!output)
        {
            manager = BoardManager.GetInstance();
            if (manager == null)
            {
                Destroy(this.gameObject);
                Debug.Log("[ERROR]  NO BOARD IN THIS GAME!");
            }

            PieceManager pieceManager = manager.gameObject.GetComponent<PieceManager>();
            release = pieceManager.Release;
            initiated = true;
        }

        position = tile.GetPosition();
        tile.SetPiece(this);
        isBlack = black;
        return output;
    }


    // ==============================================================================
    // Unity FrameCycle Methods
    // ==============================================================================
    private void Start()
    {
    }

    private void Update()
    {
        
    }

    // ==============================================================================
    // Methods
    // ==============================================================================
    public bool Move(Tile tile)
    {
        if (tile == null)
            return false;

        foreach (Tile item in moveableTiles)
        {
            if (item != tile)
                continue;

            position = tile.GetPosition();
            moveableTiles.Clear();
            gameObject.transform.position = tile.gameObject.transform.position;

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
    protected BoardManager manager = null;
    protected List<Tile> moveableTiles;
    public PieceManager.PieceType type { get; protected set; }

    delegate void ReleasePieceMethod(Piece component);
    private ReleasePieceMethod release;

}
