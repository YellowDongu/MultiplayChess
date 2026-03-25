using System.Collections;
using System.Net;
using TMPro;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class ClientHandler : NetworkHandler
{
    // ==============================================================================
    // constructor/Initializer
    // ==============================================================================
    public void StartClient()
    {
        if (!GetIP())
            return;

        StartCoroutine(Connect());
    }


    // ==============================================================================
    // Unity FrameCycle Methods
    // ==============================================================================

    private void FixedUpdate()
    {
        if (!connected)
            return;

        NetworkEvent.Type cmd;
        driver.ScheduleUpdate().Complete();

        if ((cmd = driver.PopEventForConnection(connection, out DataStreamReader stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Data)
                ReadData(ref stream);
            else if (cmd == NetworkEvent.Type.Disconnect)
                Disconnected();
        }
    }

    protected void ReadData(ref DataStreamReader stream)
    {
        switch (stream.ReadInt())
        {
            case -1:
                {
                    bool isBlack = stream.ReadInt() == 1;
                    GameMaster.GetInstance().StartGame(GameMaster.NetworkState.Client, false, isBlack);
                }
                break;
            case 0:
                {
                    int[] data = new int[4];
                    int i = 0;

                    for (; i < 4; i++)
                        if (stream.Length - stream.GetBytesRead() >= 4)
                            data[i] = stream.ReadInt();

                    bool turn = stream.ReadInt() == 1;
                    if (i != 4)
                        return;

                    Vector2Int from = new Vector2Int(data[0], data[1]);
                    Vector2Int to = new Vector2Int(data[2], data[3]);

                    Debug.Log($"{from.x}:{from.y}->{to.x}:{to.y}, turn : {((turn) ? "Black" : "White")}"); // Debug
                    GameMaster.GetInstance().ReceivedMove(from, to, turn);
                }
                return;
            case 1:
                break;

            default:
                break;
        }


        return;
    }


    // ==============================================================================
    // Methods
    // ==============================================================================

    private IEnumerator Connect()
    {
        GameMaster.GetInstance().Log($"{ip} 연결중....");

        if(!InitializeDevice())
            yield break;

        connection = driver.Connect(NetworkEndpoint.Parse(ip, port));

        NetworkEvent.Type connectionEvent;
        float deltaTime = 0.0f;

        for (; deltaTime <= 5.0f; deltaTime += Time.deltaTime)
        {
            driver.ScheduleUpdate().Complete();

            connectionEvent = driver.PopEventForConnection(connection, out var stream);
            if (connectionEvent == NetworkEvent.Type.Connect)
                break;

            if (Time.frameCount % 60 == 0 && driver.GetConnectionState(connection) == NetworkConnection.State.Disconnected)
            {
                GameMaster.GetInstance().Log($"서버가 연결을 거부했습니다.");
                yield break;
            }

            yield return null;
        }

        if (deltaTime > 5.0f)
        {
            GameMaster.GetInstance().Log($"상대가 응답하지 않습니다.");
            Release();
            yield break;
        }

        GameMaster.GetInstance().Log($"연결 성공.");
        GameMaster.GetInstance().SetNetworkState(GameMaster.NetworkState.Client);
        connected = true;
    }

    public bool GetIP()
    {
        ip = ipInput.text;
        if (IPAddress.TryParse(ip, out IPAddress address))
        {
            if (address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork) // only IPv4
                return false;

            return true;
        }

        ip = "127.0.0.1";
        return false;
    }

    public void RematchSend(bool rematch)
    {
        if (!connected || !start)
            return;

        SendData(new int[2] { 1, (rematch ? 1 : 0) });
    }

    // ==============================================================================
    // variable & GetSet Methods
    // ==============================================================================

    public void SetIP(string _ip) { ipInput.text = _ip; ip = _ip; }


    [SerializeField] private TMP_InputField ipInput;
}
