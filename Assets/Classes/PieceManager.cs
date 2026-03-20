using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class PieceManager : MonoBehaviour // 기술적인 것을 위해서 오브젝트 풀링을 구현하도록
{
    public enum PieceType { Pawn, Rook, Knight, Bishop, Queen, King, END }

    // ==============================================================================
    // Unity FrameCycle Methods
    // ==============================================================================

    public void Start()
    {
        boardStatus = gameObject.GetComponent<BoardManager>().LinkBoard();
        BlackPawnPool = new ObjectPool<GameObject>(createFunc: () => Instantiate(BlackPawnPrefab), actionOnGet: obj => obj.SetActive(true), actionOnRelease: obj => obj.SetActive(false), actionOnDestroy: obj => Destroy(obj), maxSize: 10);
        BlackRookPool = new ObjectPool<GameObject>(createFunc: () => Instantiate(BlackRookPrefab), actionOnGet: obj => obj.SetActive(true), actionOnRelease: obj => obj.SetActive(false), actionOnDestroy: obj => Destroy(obj), maxSize: 10);
        BlackQueenPool = new ObjectPool<GameObject>(createFunc: () => Instantiate(BlackQueenPrefab), actionOnGet: obj => obj.SetActive(true), actionOnRelease: obj => obj.SetActive(false), actionOnDestroy: obj => Destroy(obj), maxSize: 10);
        BlackBishopPool = new ObjectPool<GameObject>(createFunc: () => Instantiate(BlackBishopPrefab), actionOnGet: obj => obj.SetActive(true), actionOnRelease: obj => obj.SetActive(false), actionOnDestroy: obj => Destroy(obj), maxSize: 10);
        BlackKnightPool = new ObjectPool<GameObject>(createFunc: () => Instantiate(BlackKnightPrefab), actionOnGet: obj => obj.SetActive(true), actionOnRelease: obj => obj.SetActive(false), actionOnDestroy: obj => Destroy(obj), maxSize: 10);
        whitePawnPool = new ObjectPool<GameObject>(createFunc: () => Instantiate(whitePawnPrefab), actionOnGet: obj => obj.SetActive(true), actionOnRelease: obj => obj.SetActive(false), actionOnDestroy: obj => Destroy(obj), maxSize: 10);
        whiteRookPool = new ObjectPool<GameObject>(createFunc: () => Instantiate(whiteRookPrefab), actionOnGet: obj => obj.SetActive(true), actionOnRelease: obj => obj.SetActive(false), actionOnDestroy: obj => Destroy(obj), maxSize: 10);
        whiteQueenPool = new ObjectPool<GameObject>(createFunc: () => Instantiate(whiteQueenPrefab), actionOnGet: obj => obj.SetActive(true), actionOnRelease: obj => obj.SetActive(false), actionOnDestroy: obj => Destroy(obj), maxSize: 10);
        whiteBishopPool = new ObjectPool<GameObject>(createFunc: () => Instantiate(whiteBishopPrefab), actionOnGet: obj => obj.SetActive(true), actionOnRelease: obj => obj.SetActive(false), actionOnDestroy: obj => Destroy(obj), maxSize: 10);
        whiteKnightPool = new ObjectPool<GameObject>(createFunc: () => Instantiate(whiteKnightPrefab), actionOnGet: obj => obj.SetActive(true), actionOnRelease: obj => obj.SetActive(false), actionOnDestroy: obj => Destroy(obj), maxSize: 10);
        Initialize();
    }

    // ==============================================================================
    // Methods
    // ==============================================================================

    public void Initialize()
    {
        Get(PieceType.King, boardStatus[4, 7], false);
        Get(PieceType.King, boardStatus[3, 0], true);
        //if (BlackKing == null)
        //    BlackKing = Instantiate(BlackKingPrefab);
        //BlackKing.GetComponent<King>().Initialize(boardStatus[4, 7], true);
        //if (whiteKing == null)
        //    whiteKing = Instantiate(whiteKingPrefab);
        //whiteKing.GetComponent<King>().Initialize(boardStatus[3, 0], true);

        for (int i = 0; i < 8; i++)
        {
            Get(PieceType.Pawn, boardStatus[i, 6], false);
            Get(PieceType.Pawn, boardStatus[i, 1], true);
        }

        Get(PieceType.Rook, boardStatus[0, 7], false);
        Get(PieceType.Rook, boardStatus[7, 7], false);
        Get(PieceType.Rook, boardStatus[0, 0], true);
        Get(PieceType.Rook, boardStatus[7, 0], true);

        Get(PieceType.Knight, boardStatus[1, 7], false);
        Get(PieceType.Knight, boardStatus[6, 7], false);
        Get(PieceType.Knight, boardStatus[1, 0], true);
        Get(PieceType.Knight, boardStatus[6, 0], true);

        Get(PieceType.Bishop, boardStatus[2, 7], false);
        Get(PieceType.Bishop, boardStatus[5, 7], false);
        Get(PieceType.Bishop, boardStatus[2, 0], true);
        Get(PieceType.Bishop, boardStatus[5, 0], true);

        Get(PieceType.Queen, boardStatus[3, 7], false);
        Get(PieceType.Queen, boardStatus[4, 0], true);
    }


    public GameObject Get(PieceType type, Tile spawnLocation, bool black)
    {
        GameObject output = null;
        switch (type)
        {
            case PieceType.Pawn:
                if (black)
                    BlackPawnPool.Get(out output);
                else
                    whitePawnPool.Get(out output);
                output.GetComponent<Pawn>().Initialize(spawnLocation, black);
                break;
            case PieceType.Rook:
                if (black)
                    BlackRookPool.Get(out output);
                else
                    whiteRookPool.Get(out output);
                output.GetComponent<Rook>().Initialize(spawnLocation, black);
                break;
            case PieceType.Knight:
                if (black)
                    BlackKnightPool.Get(out output);
                else
                    whiteKnightPool.Get(out output);
                output.GetComponent<Knight>().Initialize(spawnLocation, black);
                break;
            case PieceType.Bishop:
                if (black)
                    BlackBishopPool.Get(out output);
                else
                    whiteBishopPool.Get(out output);
                output.GetComponent<Bishop>().Initialize(spawnLocation, black);
                break;
            case PieceType.Queen:
                if (black)
                    BlackQueenPool.Get(out output);
                else
                    whiteQueenPool.Get(out output);
                output.GetComponent<Queen>().Initialize(spawnLocation, black);
                break;
            case PieceType.King:
                if (black)
                {
                    if (BlackKing == null)
                        BlackKing = Instantiate(BlackKingPrefab);
                    output = BlackKing;
                }
                else
                {
                    if (whiteKing == null)
                        whiteKing = Instantiate(whiteKingPrefab);
                    output = whiteKing;
                }
                output.GetComponent<King>().Initialize(spawnLocation, black);
                break;
            default:
                break;
        }
        return output;
    }

    public void Release(GameObject target)
    {
        if (target == null)
            return;
        Release(target.GetComponent<Piece>());
    }

    public void Release(Piece component)
    {
        if (component == null)
            return;

        switch (component.type)
        {
            case PieceType.Pawn:
                if (component.isBlack)
                    BlackPawnPool.Release(component.gameObject);
                else
                    whitePawnPool.Release(component.gameObject);
                break;
            case PieceType.Rook:
                if (component.isBlack)
                    BlackRookPool.Release(component.gameObject);
                else
                    whiteRookPool.Release(component.gameObject);
                break;
            case PieceType.Knight:
                if (component.isBlack)
                    BlackKnightPool.Release(component.gameObject);
                else
                    whiteKnightPool.Release(component.gameObject);
                break;
            case PieceType.Bishop:
                if (component.isBlack)
                    BlackBishopPool.Release(component.gameObject);
                else
                    whiteBishopPool.Release(component.gameObject);
                break;
            case PieceType.Queen:
                if (component.isBlack)
                    BlackQueenPool.Release(component.gameObject);
                else
                    whiteQueenPool.Release(component.gameObject);
                break;
            default:
                component.gameObject.SetActive(false);
                break;
        }
    }
    public void Restart()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (boardStatus[i, j].GetPiece() != null)
                    Release(boardStatus[i, j].GetPiece());
            }
        }

        foreach (var item in WhiteDead)
            Release(item);
        foreach (var item in BlackDead)
            Release(item);

        Initialize();
    }


    // ==============================================================================
    // variables
    // ==============================================================================
    [SerializeField] private GameObject BlackPawnPrefab = null;
    [SerializeField] private GameObject BlackRookPrefab = null;
    [SerializeField] private GameObject BlackQueenPrefab = null;
    [SerializeField] private GameObject BlackKingPrefab = null;
    [SerializeField] private GameObject BlackBishopPrefab = null;
    [SerializeField] private GameObject BlackKnightPrefab = null;
    [SerializeField] private GameObject whitePawnPrefab = null;
    [SerializeField] private GameObject whiteRookPrefab = null;
    [SerializeField] private GameObject whiteQueenPrefab = null;
    [SerializeField] private GameObject whiteKingPrefab = null;
    [SerializeField] private GameObject whiteBishopPrefab = null;
    [SerializeField] private GameObject whiteKnightPrefab = null;

    private GameObject BlackKing;
    private GameObject whiteKing;
    private ObjectPool<GameObject> BlackPawnPool;
    private ObjectPool<GameObject> BlackRookPool;
    private ObjectPool<GameObject> BlackQueenPool;
    private ObjectPool<GameObject> BlackBishopPool;
    private ObjectPool<GameObject> BlackKnightPool;
    private ObjectPool<GameObject> whitePawnPool;
    private ObjectPool<GameObject> whiteRookPool;
    private ObjectPool<GameObject> whiteQueenPool;
    private ObjectPool<GameObject> whiteBishopPool;
    private ObjectPool<GameObject> whiteKnightPool;

    private List<GameObject> WhiteDead;
    private List<GameObject> BlackDead;

    private Tile[,] boardStatus;
}
