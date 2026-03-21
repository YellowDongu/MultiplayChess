using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;

public class BoardManager : MonoBehaviour
{
    // ==============================================================================
    // constructor/Initializer
    // ==============================================================================
    public BoardManager() { boardStatus = new Tile[8, 8]; }

    public bool Initialize()
    {
        bool output = true;
        Vector3 position;
        GameObject newInstance;
        position.y = 0.1f;
        for (int i = 0; i < 8; i++)
        {
            position.x = GetPosition(i);
            for (int j = 0; j < 8; j++)
            {
                newInstance = Instantiate(tilePrefab);

                if (newInstance == null)
                {
                    Debug.LogError("[Error] Instantiate failed :: Tiles " + i.ToString() + ":" + j.ToString());
                    output = false;
                    continue;
                }
                boardStatus[i, j] = newInstance.GetComponent<Tile>();
                boardStatus[i, j].Initialize(i, j);
                position.z = GetPosition(j);
                newInstance.transform.position = position;
            }
        }
        return output;
    }

    // ==============================================================================
    // Unity FrameCycle Methods
    // ==============================================================================
    private void Awake()
    {
        if (singleInstance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        singleInstance = this;
        Initialize();
    }

    private void Start()
    {
    }

    // ==============================================================================
    // Methods
    // ==============================================================================

    public void StartGame()
    {
        GetComponent<PieceManager>().Initialize();
    }

    public void Hover(Tile tile)
    {
        if (hover == tile)
        {
            if (hover.GetState() != Tile.Status.Highlight)
                hover.SetState(Tile.Status.Highlight);
            return;

        }
        if (hover != null)
            hover.SetState(Tile.Status.None);
        if (tile == null)
        {
            hover = null;
            return;
        }
        hover = tile;
        hover.SetState(Tile.Status.Highlight);
    }

    public bool MovePiece(Vector2Int from, Vector2Int to) { return MovePiece(boardStatus[from.x, from.y], boardStatus[to.x, to.y]); }
    public bool MovePiece(Tile from, Tile to)
    {
        Piece piece = from.GetPiece();
        Piece occupied = to.GetPiece();

        if (piece == null)
            return false;

        if (occupied != null)
        {
            if (occupied.isBlack == piece.isBlack)
                return false;

            occupied.Release();
        }

        piece.gameObject.transform.position = to.gameObject.transform.position;
        piece.SetPosition(to);
        to.SetPiece(piece);
        from.SetPiece(null);

        return true;
    }
    public void SetPiece(Tile to, Piece piece)
    {
        if (piece == null)
            return;

        if (to.GetPiece() != null)
            to.GetPiece().Release();

        piece.gameObject.transform.position = to.gameObject.transform.position;
        piece.SetPosition(to);
        to.SetPiece(piece);
    }
    private float GetPosition(int index) { return (float)(index - 4) * tilesize + (tilesize * 0.5f); }

    public bool Validate(Vector2Int from, Vector2Int to) { return Validate(GetTile(from), GetTile(to)); }
    public bool Validate(Tile from, Tile to)
    {
        if ((from == null || to == null) || from.GetPiece() == null)
            return false;

        return from.GetPiece().CanMove(to);
    }

    // ==============================================================================
    // variable GetSet Methods
    // ==============================================================================
    public Tile[,] LinkBoard() { return boardStatus; }
    public float tTileSize() { return tilesize; }
    public Tile GetTile(int x, int y) { if (x < 0 || y < 0 || y >= 8 || x >= 8) return null; return boardStatus[x, y]; }
    public Tile GetTile(Vector2Int position) { if (position.x < 0 || position.y < 0 || position.y >= 8 || position.x >= 8) return null; return boardStatus[position.x, position.y]; }

    // ==============================================================================
    // variables
    // ==============================================================================

    private Tile[,] boardStatus;
    private static BoardManager singleInstance = null;

    [SerializeField] private Tile hover = null;
    [SerializeField] private float tilesize = 1.5f;
    [SerializeField] private GameObject tilePrefab = null;
}
