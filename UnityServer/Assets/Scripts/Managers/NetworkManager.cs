using Client.Session;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Session;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Jobs;
using UnityEngine;

public class NetworkManager {

    IPEndPoint m_loginEndPoint = null;
    private Connector _connector = new Connector();
    private ServerSession m_loginSession = new ServerSession();

    private Listener _listener = new Listener();

    public long LocalIPAddress { get; private set; }
    public int LocalPort { get; private set; } = 54321;

    public void ConnectTo(IPAddress ipAddress, int port) {
        m_loginEndPoint = new IPEndPoint(ipAddress, port);

        _connector.Connect(m_loginEndPoint, () => { return m_loginSession; });
    }

    public void Listen() {
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, LocalPort);

        _listener.Listen(endPoint, () => SessionManager.Instance.Generate<ClientSession>(), 10, 10);

        S_Login_Game_Standby standby = new S_Login_Game_Standby();
        standby.AreaType = InGameSceneManager.Instance._areaType;

        pEndPoint pendPoint = new pEndPoint();
        pendPoint.HostString = host;
        pendPoint.Port = endPoint.Port;
        standby.EndPoint = pendPoint;

        standby.AreaType = InGameSceneManager.Instance._areaType;

        m_loginSession.Send(standby);
    }

    public void Broadcast(IMessage packet) {
        
    }
}
