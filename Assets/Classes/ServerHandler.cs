using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using static PieceManager;

public class ServerHandler : MonoBehaviour
{
    // ==============================================================================
    // constructor/Initializer
    // ==============================================================================
    public bool Initialize()
    {
        if (start)
            return false;

        NetworkSettings settings = new NetworkSettings();
        settings.WithNetworkConfigParameters(
            disconnectTimeoutMS: 5000,
            connectTimeoutMS: 3000,
            heartbeatTimeoutMS: 2000
        );

        driver = NetworkDriver.Create(settings);

        endpoint = NetworkEndpoint.AnyIpv4.WithPort(port);
        if (driver.Bind(endpoint) != 0)
        {
            Debug.LogError("포트 바인딩 실패");
            return false;
        }

        int result = driver.Listen();
        Debug.Log($"Server Listening : {result}");
        start = true;
        client = default;
        connected = false;
        return true;
    }

    public void Release()
    {
        if (!start)
            return;

        if (driver.IsCreated)
            driver.Dispose();

        client = default;
        start = false;
        connected = false;
    }
    void OnDestroy()
    {
        Release();
    }


    void Update()
    {
        if (!start)
            return;

        driver.ScheduleUpdate().Complete();


        if (connected)
        {
            if (!ReadData(out int[] data))
                return;

            Vector2Int from = new Vector2Int(data[0], data[1]), to = new Vector2Int(data[2], data[3]);
            if (!varifier.Move(from.x, from.y, to.x, to.y))
                return;

            SendData(data);
            GameMaster.GetInstance().ReceivedMove(from, to);
        }
        else
        {
            Accept();
            //NetworkEvent.Type cmd;
            //while ((cmd = driver.PopEvent(out NetworkConnection con, out var reader)) != NetworkEvent.Type.Empty)
            //{
            //    Debug.Log($"[SERVER] 원시 이벤트 감지: {cmd} from {con.GetHashCode()}");
            //    if (cmd == NetworkEvent.Type.Connect)
            //        Debug.Log($"[SERVER] Connect");
            //}

        }
    }

    // ==============================================================================
    // Methods
    // ==============================================================================

    private void Accept()
    {
        NetworkConnection connect = driver.Accept();

        if(connect != default)
        {
            client = connect;
            Debug.Log("Client Connected");
            connected = true;
            GameMaster.GetInstance().StartGame(GameMaster.NetworkState.Server, false, true);

            varifier.Initialize(true);
        }
    }

    public bool ReadData(out int[] data)
    {
        data = null;
        NetworkEvent.Type cmd;
        if ((cmd = driver.PopEventForConnection(client, out DataStreamReader stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Data)
            {
                data = new int[4];
                int i;
                for (i = 0; i < 4; i++)
                {
                    if (stream.Length - stream.GetBytesRead() >= 4)
                    {
                        data[i] = stream.ReadInt();
                    }
                }

                if (i != 4)
                    return false;

                Vector2Int from = new Vector2Int(data[0], data[1]);
                Vector2Int to = new Vector2Int(data[2], data[3]);
                Debug.Log($"{from.x}:{from.y}->{to.x}:{to.y}");


                return true;

            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                client = default;
                Debug.Log("Client Disconnected");
                GameMaster.GetInstance().ConnectionLost();
                connected = false;
                return false;
            }
        }

        return false;
    }

    public void SendData(Vector2Int from, Vector2Int to)
    {
        if (!connected || !start)
            return;

        if (!varifier.Move(from.x, from.y, to.x, to.y))
            return;

        if(SendData(new int[4] { from.x, from.y, to.x, to.y }))
            GameMaster.GetInstance().ReceivedMove(from, to);
    }

    private bool SendData(int[] data)
    {
        DataStreamWriter writer;
        if (driver.BeginSend(client, out writer) != 0)
            return false;

        foreach (int item in data)
        {
            if (!writer.WriteInt(item))
            {
                driver.EndSend(writer);
                return false;
            }
        }

        driver.EndSend(writer);
        return true;
    }

    // ==============================================================================
    // variables
    // ==============================================================================

    [SerializeField] private MovementChecker varifier;

    private bool start = false, connected = false;
    private ushort port = 7777;
    private NetworkEndpoint endpoint;
    private NetworkDriver driver;
    private NetworkConnection client;

}