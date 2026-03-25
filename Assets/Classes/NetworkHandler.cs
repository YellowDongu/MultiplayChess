using System;
using System.Collections;
using System.Net;
using TMPro;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class NetworkHandler : MonoBehaviour
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

    public void Release()
    {
        if (driver.IsCreated)
            driver.Dispose();
        connection = default;
        Connected = false;
    }

    void OnDestroy()
    {
        Release();
    }

    private void Start()
    {
        Connected = false;
    }



    private void FixedUpdate()
    {
        if (!Connected)
            return;

        driver.ScheduleUpdate().Complete();
        if (ReadData(out int[] data))
            GameMaster.GetInstance().ReceivedMove(new Vector2Int(data[0], data[1]), new Vector2Int(data[2], data[3]));
    }

    // ==============================================================================
    // Methods
    // ==============================================================================

    private IEnumerator Connect()
    {
        Debug.Log($"Connecting to : {ip}");

        NetworkSettings settings = new NetworkSettings();
        settings.WithNetworkConfigParameters(
            disconnectTimeoutMS: 5000,
            connectTimeoutMS: 3000,
            heartbeatTimeoutMS: 2000
        );

        driver = NetworkDriver.Create(settings);
        connection = driver.Connect(NetworkEndpoint.Parse(ip, port));

        NetworkEvent.Type cmd;
        float deltaTime = 0.0f;

        for (; deltaTime <= 5.0f; deltaTime += Time.deltaTime)
        {
            driver.ScheduleUpdate().Complete();

            cmd = driver.PopEventForConnection(connection, out var stream);

            if (cmd == NetworkEvent.Type.Connect)
                break;

            if (Time.frameCount % 30 == 0)
            {
                var currentState = driver.GetConnectionState(connection);
                Debug.Log($"[CLIENT] 연결 시도 중... 현재 상태: {currentState}");

                if (currentState == NetworkConnection.State.Disconnected)
                    Debug.LogError("서버가 연결을 거부(Reject)했습니다.");
            }
            yield return null;
        }

        if (deltaTime > 5.0f)
        {
            Debug.Log("Connection TimeOut!");
            Release();
            yield break;
        }

        Debug.Log("Connected!");
        Connected = true;
        GameMaster.GetInstance().StartGame(GameMaster.NetworkState.Client, false, false);
    }

    public void Send(Vector2Int from, Vector2Int to) { SendData(new int[4] { from.x, from.y, to.x, to.y }); }

    public void SendData(int[] data)
    {
        DataStreamWriter writer;
        if (driver.BeginSend(connection, out writer) != 0)
            return;

        foreach (int item in data)
            writer.WriteInt(item);

        driver.EndSend(writer);
    }

    private bool ReadData(out int[] data)
    {
        data = null;
        NetworkEvent.Type cmd;

        // 1. 큐에서 '패킷 하나'를 꺼냅니다. (C++의 한 프레임 패킷 수신)
        if ((cmd = driver.PopEventForConnection(connection, out DataStreamReader stream)) != NetworkEvent.Type.Empty)
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
                return i == 4;
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("서버와 연결이 끊겼습니다.");
                Release();
                GameMaster.GetInstance().ConnectionLost();
                return false;
            }
        }
        return false;
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

    // ==============================================================================
    // variable & GetSet Methods
    // ==============================================================================

    public void SetConnection(NetworkConnection client) { connection = client; }
    public void SetIP(string _ip) { ipInput.text = _ip; ip = _ip; }

    private bool Connected;
    private ushort port = 7777;
    private string ip = "127.0.0.1";


    private NetworkDriver driver;
    private NetworkConnection connection;

    [SerializeField] private TMP_InputField ipInput;

}
