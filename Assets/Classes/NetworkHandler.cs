using System;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class NetworkHandler : MonoBehaviour
{
    public bool StartServer()
    {
        ip = "0.0.0.0";
        bool result = NetworkManager.Singleton.StartServer();
        local = false;

        if (result)
            Debug.Log("Server Started!");

        return result;
    }

    public bool StartClient()
    {
        local = true;
        Debug.Log("Client Attempting Connection...");
        bool result = NetworkManager.Singleton.StartClient();

        if (!result)
            return false;

        Debug.Log("Connected!");
        local = false;

        return result;
    }

    public void SetupConnection()
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, port);
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnect;
    }

    private void OnDisconnect(ulong clientId)
    {
        //// 1. 서버 입장에서 감지할 때
        //if (IsServer)
        //{
        //    Debug.Log($"클라이언트 {clientId}번이 응답이 없어 연결이 해제되었습니다.");
        //}
        //
        //// 2. 클라이언트 입장에서 감지할 때 (서버가 죽었을 때)
        //if (IsClient && clientId == NetworkManager.Singleton.LocalClientId)
        //{
        //    Debug.Log("서버와의 연결이 끊어졌습니다. 메인 화면으로 이동합니다.");
        //}
    }


    // 전송 로직
    [ServerRpc]
    private void MoveRequestServerRpc(Vector2Int from, Vector2Int to) // RPC, remote procedure call
    {
        BoardManager manager = GameMaster.GetInstance().GetBoard();
        //manager.va
        manager.MovePiece(from, to);
    }
    public void Send(Tile to, Tile from)
    {
        if (local)
        {
            if (GameMaster.GetInstance().Validate(from, to))
                GameMaster.GetInstance().Place(to, from);
            return;
        }

        MoveRequestServerRpc(from.GetPosition(), to.GetPosition());
    } // Capsule

    // 송신 로직
    [ClientRpc]
    private void UpdateBoardClientRpc(Vector2Int from, Vector2Int to)
    {
        BoardManager manager = GameMaster.GetInstance().GetBoard();
        manager.MovePiece(from, to);
    }
    public void reception(Tile to, Tile from) { UpdateBoardClientRpc(from.GetPosition(), to.GetPosition()); } // Capsule

    public void GetIP()
    {
        if (!Keyboard.current.enterKey.isPressed)
            return;

        ip = ipInput.text;
    }

    public void SetLocal(bool value) { local = value; }

    private ushort port = 7777;
    private string ip = "127.0.0.1";
    private bool local = true;
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] public UnityEvent PlaceEvent;
}
