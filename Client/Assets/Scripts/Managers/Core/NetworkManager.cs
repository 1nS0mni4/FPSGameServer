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
public class NetworkManager : Manager, IManagerStart, IManagerUpdate, IManagerFixedUpdate, IManagerOnApplicationPause, IManagerOnApplicationQuit
{
    private bool          onSystemPause = false;
    private ServerSession _loginSession = new ServerSession();
    private ServerSession _gameSession  = new ServerSession();

    public ushort InterpolationTick { get; private set; }
    private ushort _ticksBetweenPositionUpdates = 2;
    public ushort TicksBetweenPositionUpdates {
        get => _ticksBetweenPositionUpdates;
        private set {
            _ticksBetweenPositionUpdates = value;
            InterpolationTick = (ushort)( ServerTick - value );
        }
    }

    [SerializeField] private ushort tickDivergenceTolerance = 1;

    public bool InGame { get { return _gameSession.Connected; } }
    public uint AuthCode { get => _loginSession.AuthCode; set => _loginSession.AuthCode = value; }
    private uint _serverTick;
    public uint ServerTick {
        get => _serverTick;
        private set {
            _serverTick = value;
            InterpolationTick = (ushort)( value - TicksBetweenPositionUpdates );
        }
    }

    #region Unity Event Functions
    public void Start() {
        ServerTick = 2;     
        Connect_Login();
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

    public void FixedUpdate() { ServerTick++; }

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

    #endregion

    private void Connect_Login(IPAddress ipAddress = null, int port = 17321) {
        PacketManager.Instance.CustomHandler = (session, packet, id) => {
            PacketQueue.Instance.Push(id, packet);
        };

        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);

        if(ipAddress == null)
            ipAddress = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddress, port);

        Connector connector = new Connector();
        connector.Connect(endPoint, () => { return _loginSession; });
    }

    public void Connect_Game(pEndPoint pendPoint) {
        IPHostEntry ipHost = Dns.GetHostEntry(pendPoint.HostString);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, pendPoint.Port);

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

    public void SetTick(uint serverTick) {
        if(Mathf.Abs(ServerTick - serverTick) > tickDivergenceTolerance) {
            Debug.Log($"Client tick: {ServerTick} -> {serverTick}");
            ServerTick = serverTick;
        }
    }
}