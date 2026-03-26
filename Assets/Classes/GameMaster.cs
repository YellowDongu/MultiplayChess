using System;
using System.Net;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;
using UnityEngine.Windows;
using static PieceManager;

public class GameMaster : MonoBehaviour
{
    // ==============================================================================
    // Enum/Structs
    // ==============================================================================
    public enum NetworkState
    {
        NULL = 0,
        Client,
        Host,
        END
    }

    // ==============================================================================
    // constructor/Initializer
    // ==============================================================================

    private void Awake()
    {
        if (singleInstance != null || boardManager == null)
        {
            Destroy(this.gameObject);
            return;
        }

        singleInstance = this;
    }

    void Start()
    {
        player.gameObject.SetActive(false);
    }

    // ==============================================================================
    // Methods
    // ==============================================================================

    public void StartClient()
    {
        host.Release();
        client.StartClient();
    }

    public void StartServer()
    {
        client.Release();
        if (!host.Initialize())
            return;
        client.SetIP("127.0.0.1");
        networkState = NetworkState.Host;

    }

    public void OnEndEdit()
    {
        if (!Keyboard.current.spaceKey.wasPressedThisFrame)
            return;

        if (networkState != NetworkState.NULL)
            return;

        client.StartClient();
    }

    public GameObject GetNewPiece(PieceType type, Tile spawnLocation, bool black) { return pieceManager.Get(type, spawnLocation, black); }

    public void RequestMove(Tile to, Tile from)
    {
        switch (networkState)
        {
            case NetworkState.NULL:
                DebugMove(from, to);
                ReceivedMove(from.GetPosition(), to.GetPosition(), turn);
                break;
            case NetworkState.Client:
                client.Send(from.GetPosition(), to.GetPosition(), turn);
                break;
            case NetworkState.Host:
                host.Send(from.GetPosition(), to.GetPosition(), turn);
                break;
            default:
                break;
        }
    }

    public void ReceivedMove(Vector2Int from, Vector2Int to, bool currentTurn)
    {
        Win(to);
        boardManager.MovePiece(from, to);

        if (player.gameObject.activeInHierarchy)
            player.Place();

        turn = !currentTurn;
        boardManager.ReCalculate();

        if (networkState == NetworkState.NULL)
            return;

        Piece piece = boardManager.GetTile(from).GetPiece();
        Log($" {GetPieceName(piece.type, piece.isBlack)} : {GetPosition(from)} -> {GetPosition(to)}, {(turn ? "흑턴" : "백턴")}");
        if (win != 0)
        {
            sidePanelUI.PanelUnFold();
            player.gameObject.SetActive(false);
            EndGame();
            return;
        }
            
        player.gameObject.SetActive(isBlack == turn);
    }

    public void StartGame(NetworkState state, bool isLocal, bool _isBlack = true)
    {
        sidePanelUI.LocalMode(isLocal);
        networkState = state;
        pieceManager.Restart();
        player.Initialize(_isBlack, isLocal);
        turn = true;
        win = 0;
        isBlack = _isBlack;
        sidePanelUI.PanelFold();
        DebugStart();
        Log($"매치 시작.");
    }

    public void Win(Vector2Int to)
    {
        Piece piece = boardManager.GetTile(to).GetPiece();

        if (piece == null)
            return;

        if (piece.type == PieceManager.PieceType.King)
            win = (piece.isBlack != isBlack) ? 1 : -1;
    }

    public void EndGame()
    {
        switch (networkState)
        {
            case NetworkState.Client:
                sidePanelUI.AfterGamePanelActive(win == 1);
                break;
            case NetworkState.Host:
                sidePanelUI.AfterGamePanelActive(win == 1);
                break;
            default:
                break;
        }
    }

    public void Rematch()
    {
        switch (networkState)
        {
            case NetworkState.Client:
                client.RematchSend(true);
                break;
            case NetworkState.Host:
                host.GetRematch(true, true);
                break;
            default:
                break;
        }
    }

    public void Exitmatch()
    {
        switch (networkState)
        {
            case NetworkState.Client:
                client.Release();
                break;
            case NetworkState.Host:
                host.Release();
                break;
            default:
                break;
        }

        networkState = NetworkState.NULL;
        player.gameObject.SetActive(false);
        sidePanelUI.MatchmakingPanelActive();
        PieceClear();
    }

    public void LocalStart()
    {
        Log("자유 경기 시작.");
        StartGame(NetworkState.NULL, true, true);
    }

    public void ConnectionLost()
    {
        networkState = NetworkState.NULL;
        sidePanelUI.MatchmakingPanelActive();
        Log("상대와 연결이 끊겼습니다.");
        PieceClear();
    }

    public void GameOff()
    {
        host.Release();
        client.Release();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    private void DebugStart()
    {
        //gameObject.GetComponent<MovementChecker>().Initialize(true); // manual
    }
    private void DebugMove(Tile from, Tile to)
    {
        Log($"{GetPieceName(from.GetPiece().type, from.GetPiece().isBlack)} : {GetPosition(from.GetPosition())} -> {GetPosition(to.GetPosition())}");
        //gameObject.GetComponent<MovementChecker>().Move(from.GetPosition().x, from.GetPosition().y, to.GetPosition().x, to.GetPosition().y); // manual
    }

    public string GetPieceName(PieceType type, bool isBlack)
    {
        string piece = isBlack ? "흑" : "백";

        switch (type)
        {
            case PieceType.Pawn:
                piece += "폰";
                break;
            case PieceType.Rook:
                piece += "룩";
                break;
            case PieceType.Knight:
                piece += "나이트";
                break;
            case PieceType.Bishop:
                piece += "비숍";
                break;
            case PieceType.Queen:
                piece += "퀸";
                break;
            case PieceType.King:
                piece += "킹";
                break;
            default:
                piece += "??";
                break;
        }
        return piece;
    }
    public void PieceClear() { pieceManager.Clear(); }
    public void SetNetworkState(NetworkState state) { networkState = state; }
    public void Log(string text) { logUI.Log(text); }
    public string GetPosition(Vector2Int position) { return $"{(char)((int)('A') + position.x)}{position.y}"; }

    // ==============================================================================
    // variable & GetSet Methods
    // ==============================================================================

    public BoardManager GetBoard() { return boardManager; }
    public static GameMaster GetInstance() { return singleInstance; }


    public NetworkState networkState { get; private set; }
    public bool turn { get; private set; }
    public bool isBlack { get; private set; }
    public int win { get; private set; }
    private static GameMaster singleInstance = null;

    [SerializeField] private BoardManager boardManager;
    [SerializeField] private PieceManager pieceManager;
    [SerializeField] private ClientHandler client;
    [SerializeField] private HostHandler host;
    [SerializeField] private NetworkUI sidePanelUI;
    [SerializeField] private LogUI logUI;
    [SerializeField] private Player player;
}
