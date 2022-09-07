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
    private bool          onSystemPause = false;
    private ServerSession _loginSession = new ServerSession();
    private ServerSession _gameSession  = new ServerSession();

    public bool InGame { get { return _gameSession.Connected; } }
    public uint AuthCode { get => _loginSession.AuthCode; set => _loginSession.AuthCode = value; }
    public float PingTime { get => _gameSession._pingTime; }

    private void Connect_Login(IPAddress ipAddress = null, int port = 17321) {
        PacketManager.Instance.CustomHandler = PacketQueue.Instance.Push;

        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);

        if(ipAddress == null)
            ipAddress = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddress, port);

        Connector connector = new Connector();
        connector.Connect(endPoint, () => { return _loginSession; });
    }

    public void Connect_Game(long ipAddress, int port) {
        IPEndPoint endPoint = new IPEndPoint(ipAddress, port);

        Connector connector = new Connector();
        connector.Connect(endPoint, () => { return _gameSession; });
    }

    public void Disconnect_Game() {
        if(_gameSession == null)
            return;

        C_Common_Disconnect disconnect = new C_Common_Disconnect();
        _gameSession.Send(disconnect);
        _gameSession.Disconnect();
    }

    public void Send(IMessage packet) {
        if(InGame == false)
            _loginSession.Send(packet);
        else
            _gameSession.Send(packet);
    }
    
    public void OnApplicationPause(bool pause) {
        onSystemPause = pause;
    }

    public void OnApplicationQuit() {
        if(_loginSession == null && _gameSession == null)
            return;

        C_Common_Disconnect disconnect = new C_Common_Disconnect();
        _loginSession.Send(disconnect);
        _gameSession.Send(disconnect);

        _gameSession.Disconnect();
        _loginSession.Disconnect();
    }

    public void Start() {
        Connect_Login();
    }

    private void _gameSession_OnConnectedEvent(object sender, EventArgs e) {
        throw new NotImplementedException();
        
    }

    public void Update() {
        if(PacketQueue.Instance.PacketCount == 0 || onSystemPause == true)
            return;

        List<PacketModel> list = PacketQueue.Instance.PopAll();
        for(int i = 0; i < list.Count; i++) {
            Action<PacketSession, IMessage> action = null;

            action = PacketManager.Instance.GetPacketHandler(list[i].packetID);
            if(action != null)
                action.Invoke(_loginSession, list[i].packet);
        }
    }
}