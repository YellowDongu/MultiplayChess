using UnityEngine;

public class GameMaster : MonoBehaviour
{

    public enum Status
    {
        MainMenu,
        InGame,
        EndGame,
        End
    }

    public enum PlayerState
    {
        White = 0,
        Black = 1,
        Server = 2,
        Client = 3,
        Local = 4,
        End = 5
    }

    public static GameMaster GetInstance() { return singleInstance; }
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
        //#if UNITY_SERVER
        //        playerState = PlayerState.Server;
        //#else
        //
        //        playerState = PlayerState.Client;
        //#endif
        turn = true;
        player.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RequestMove(Tile HoveringTile, Tile SelectedTile)
    {
        handler.Send(HoveringTile, SelectedTile);
    }

    public bool Place(Tile HoveringTile, Tile SelectedTile)
    {
        if (HoveringTile.GetState() != Tile.Status.Select)
            return false;

        if (boardManager.MovePiece(SelectedTile, HoveringTile))
        {
            player.Place();
            EndTurn();
            return true;
        }
        return false;
    }


    public bool Validate(Vector2Int from, Vector2Int to) { return Validate(boardManager.GetTile(from), boardManager.GetTile(to)); }
    public bool Validate(Tile from, Tile to)
    {
        if (turn != from.GetPiece().isBlack)
            return false;

        return boardManager.Validate(from, to);
    }

    public void StartGame()
    {
        boardManager.GetComponent<PieceManager>().Initialize();
    }

    private void EndTurn() { turn = !turn; }

    public BoardManager GetBoard() { return boardManager; }

    private PlayerState playerState = PlayerState.Local;
    public bool turn { get; private set; }
    private static GameMaster singleInstance = null;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private NetworkHandler handler;
    [SerializeField] private Player player;

}
