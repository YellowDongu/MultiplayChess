using System;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;
using UnityEngine.Windows;

public class GameMaster : MonoBehaviour
{
    // ==============================================================================
    // Enum/Structs
    // ==============================================================================
    public enum NetworkState
    {
        NULL = 0,
        Client,
        Server,
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

    public void StartClient() { networkState = NetworkState.Client; client.StartClient(); }

    public void StartServer()
    {
        client.SetIP("127.0.0.1");
        if (!server.Initialize())
            return;
        networkState = NetworkState.Server;

    }

    public void OnEndEdit()
    {
        if (!Keyboard.current.spaceKey.wasPressedThisFrame)
            return;

        if (networkState != NetworkState.NULL)
            return;
        networkState = NetworkState.Client;
        client.StartClient();
    }

    public void RequestMove(Tile to, Tile from)
    {
        switch (networkState)
        {
            case NetworkState.NULL:
                DebugMove(from, to);
                ReceivedMove(from.GetPosition(), to.GetPosition());
                break;
            case NetworkState.Client:
                client.Send(from.GetPosition(), to.GetPosition());
                break;
            case NetworkState.Server:
                server.SendData(from.GetPosition(), to.GetPosition());
                break;
            default:
                break;
        }
    }

    public void ReceivedMove(Vector2Int from, Vector2Int to)
    {
        Win(to);
        boardManager.MovePiece(from, to);

        if (networkState == NetworkState.NULL)
        {
            player.Place();
            return;
        }

        if(win != 0)
        {
            if (networkState != NetworkState.NULL)
                player.gameObject.SetActive(false);
            return;
        }

        if (isBlack == turn)
            player.Place();

        EndTurn();
        player.gameObject.SetActive(isBlack == turn);
        boardManager.ReCalculate();
    }

    public void StartGame(NetworkState state, bool isLocal, bool _isBlack = true)
    {
        networkState = state;
        pieceManager.Restart();
        player.Initialize(_isBlack, isLocal);
        turn = true;
        win = 0;
        isBlack = _isBlack;
        sidePanelUI.PanelFold();
        DebugStart();
    }

    public void Win(Vector2Int to)
    {
        Piece piece = boardManager.GetTile(to).GetPiece();

        if (piece == null)
            return;

        if (piece.type == PieceManager.PieceType.King)
            win = (piece.isBlack) ? 1 : -1;
    }

    private void DebugStart()
    {
        return;
        gameObject.GetComponent<MovementChecker>().Initialize(true);
    }
    private void DebugMove(Tile from, Tile to)
    {
        return;
        gameObject.GetComponent<MovementChecker>().Move(from.GetPosition().x, from.GetPosition().y, to.GetPosition().x, to.GetPosition().y);
    }

    public void LocalStart() { StartGame(NetworkState.NULL, true, true); }
    public void PieceClear() { pieceManager.Clear(); }
    private void EndTurn() { turn = !turn; }
    public void ConnectionLost() { networkState = NetworkState.NULL; PieceClear(); }

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
    [SerializeField] private NetworkHandler client;
    [SerializeField] private ServerHandler server;
    [SerializeField] private NetworkUI sidePanelUI;
    [SerializeField] private Player player;
}
