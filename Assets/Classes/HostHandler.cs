using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class HostHandler : NetworkHandler
{
    // ==============================================================================
    // constructor/Initializer
    // ==============================================================================
    public void StartServer()
    {
        if (!Initialize())
            return;
    }

    public bool Initialize()
    {
        if (start)
            return false;

        if(!InitializeDevice())
            return false;

        endpoint = NetworkEndpoint.AnyIpv4.WithPort(port);
        if (driver.Bind(endpoint) != 0)
        {
            GameMaster.GetInstance().Log($"포트 바인딩에 실패했습니다. 게임 재시작을 권장합니다.");
            Release();
            return false;
        }

        if (driver.Listen() != 0)
        {
            GameMaster.GetInstance().Log($"호스트 연결 생성 실패. 다시 시도하여주십시오.");
            Release();
            return false;
        }

        start = true;
        connection = default;
        connected = false;
        GameMaster.GetInstance().Log($"연결 준비 완료. 상대가 올때까지 대기합니다.");
        return true;
    }


    // ==============================================================================
    // Unity FrameCycle Methods
    // ==============================================================================

    void Update()
    {
        if (!start)
            return;

        driver.ScheduleUpdate().Complete();


        if (connected)
        {
            NetworkEvent.Type cmd;
            if ((cmd = driver.PopEventForConnection(connection, out DataStreamReader stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                    ReadData(ref stream, varifier);
                else if (cmd == NetworkEvent.Type.Disconnect)
                    Disconnected();
            }
        }
        else
            Accept();
    }

    protected void ReadData(ref DataStreamReader stream, MovementChecker checker)
    {
        switch (stream.ReadInt())
        {
            case -1:
                {
                    bool isBlack = stream.ReadInt() == 1;
                    GameMaster.GetInstance().StartGame(GameMaster.NetworkState.Host, false, isBlack);
                }
                break;
            case 0:
                {
                    int[] data = new int[6];
                    int i = 0;

                    for (; i < 4; i++)
                        if (stream.Length - stream.GetBytesRead() >= 4)
                            data[i + 1] = stream.ReadInt();

                    bool turn = stream.ReadInt() == 1;
                    if (i != 4)
                        return;

                    Vector2Int from = new Vector2Int(data[1], data[2]);
                    Vector2Int to = new Vector2Int(data[3], data[4]);

                    Debug.Log($"{from.x}:{from.y}->{to.x}:{to.y}, turn : {((turn) ? "Black" : "White")}"); // Debug

                    if (!checker.Move(data[1], data[2], data[3], data[4]))
                        return;

                    data[0] = 0;
                    data[5] = (turn) ? 1 : 0;

                    SendData(data);
                    GameMaster.GetInstance().ReceivedMove(from, to, turn);
                }
                return;
            case 1:
                {
                    bool rematch = stream.ReadInt() == 1;
                    GetRematch(rematch, false);
                }
                break;

            default:
                break;
        }


        return;
    }

    // ==============================================================================
    // Methods
    // ==============================================================================

    private void Accept()
    {
        NetworkConnection connect = driver.Accept();

        if (connect != default)
        {
            connection = connect;
            GameMaster.GetInstance().Log("상대방이 들어왔습니다. 게임 시작.");
            connected = true;

            varifier.Initialize(true);

            SendStart(false);
            GameMaster.GetInstance().StartGame(GameMaster.NetworkState.Host, false, true);
        }
    }

    public override void Send(Vector2Int from, Vector2Int to, bool turn)
    {
        if (!connected || !start)
            return;

        if (!varifier.Move(from.x, from.y, to.x, to.y))
            return;

        if (SendData(new int[6] { 0, from.x, from.y, to.x, to.y, (turn) ? 1 : 0 }))
            GameMaster.GetInstance().ReceivedMove(from, to, turn);
    }

    public void GetRematch(bool rematch, bool isHost)
    {
        if(isHost)
            hostRematch = rematch;
        else
            clientRematch = rematch;

        if (hostRematch && clientRematch)
        {
            bool hostIsWin = GameMaster.GetInstance().win == 1;
            SendStart(hostIsWin ? true : false);
            GameMaster.GetInstance().StartGame(GameMaster.NetworkState.Host, false, hostIsWin ? false : true);
        }
    }

    // ==============================================================================
    // variables
    // ==============================================================================

    [SerializeField] private MovementChecker varifier;
    private bool clientRematch = false, hostRematch = false;
}
