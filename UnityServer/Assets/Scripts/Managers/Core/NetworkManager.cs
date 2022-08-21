using Client.Session;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

[System.Serializable]
public class NetworkManager : Manager, IManagerStart, IManagerUpdate, IManagerOnApplicationPause, IManagerOnApplicationQuit
{
    private bool onSystemPause = false;
    ServerSession _frontSession = new ServerSession();
    ServerSession _gameSession = new ServerSession();
    private bool InGame { get { return _gameSession.Connected; } }

    public int AuthCode { get => _frontSession.AuthCode; set => _frontSession.AuthCode = value; }
    public int RoomCode { get; set; } = -1;

    public Dictionary<Type, Action<object>> MessageWait = new Dictionary<Type, Action<object>>();

    private void ServerConnection() {
        PacketManager.Instance.CustomHandler = PacketQueue.Instance.Push;
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        Connector connector = new Connector();
        connector.Connect(endPoint, () => { return _frontSession; });
    }

    public void Start() {
        ServerConnection();
    }

    public void Update() {
        if(PacketQueue.Instance.PacketCount == 0 || onSystemPause == true)
            return;

        List<PacketModel> list = PacketQueue.Instance.PopAll();
        for(int i = 0; i < list.Count; i++) {
            Action<PacketSession, IMessage> action = null;

            action = PacketManager.Instance.GetPacketHandler(list[i].packetID);
            if(action != null)
                action.Invoke(_frontSession, list[i].packet);
        }
    }

    public void Send(IMessage packet) {
        if(InGame == false)
            _frontSession.Send(packet);
        else
            _gameSession.Send(packet);
    }

    public void ConnectToGameServer() {
        
    }

    public void OnApplicationPause(bool pause) {
        onSystemPause = pause;
    }

    public void OnApplicationQuit() {
        if(_frontSession == null)
            return;

        C_Disconnect disconnect = new C_Disconnect();
        Send(disconnect);
        _frontSession.Disconnect();
    }
}