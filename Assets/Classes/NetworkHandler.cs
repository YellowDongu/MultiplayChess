using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class NetworkHandler : MonoBehaviour
{
    // ==============================================================================
    // constructor/Initializer
    // ==============================================================================
    public bool InitializeDevice()
    {
        if (start)
            return false;

        NetworkSettings settings = new NetworkSettings();
        settings.WithNetworkConfigParameters(
            disconnectTimeoutMS: 4000,
            connectTimeoutMS: 2000,
            heartbeatTimeoutMS: 1000
        );

        driver = NetworkDriver.Create(settings);
        start = driver.IsCreated;

        if (!start)
            GameMaster.GetInstance().Log("네트워크 디바이스 생성 실패");

        return driver.IsCreated;
    }


    public void Release()
    {
        if (driver.IsCreated)
            driver.Dispose();

        connection = default;
        start = connected = false;
    }

    void OnDestroy()
    {
        Release();
    }


    // ==============================================================================
    // Methods
    // ==============================================================================

    public void Disconnected()
    {
        Release();
        GameMaster.GetInstance().ConnectionLost();
    }

    public virtual void Send(Vector2Int from, Vector2Int to, bool turn) { SendData(new int[6] { 0, from.x, from.y, to.x, to.y, (turn) ? 1 : 0 }); }
    public void SendStart(bool youAreBlack) { SendData(new int[2] { -1, ((youAreBlack) ? 1 : 0) }); }

    protected bool SendData(int[] data)
    {
        if (!connected)
            return false;

        DataStreamWriter writer;
        if (driver.BeginSend(connection, out writer) != 0)
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
    // variable & GetSet Methods
    // ==============================================================================


    protected bool start = false;
    protected bool connected = false;
    protected ushort port = 7777;
    protected string ip = "127.0.0.1";

    protected NetworkDriver driver;
    protected NetworkEndpoint endpoint;
    protected NetworkConnection connection;

}
