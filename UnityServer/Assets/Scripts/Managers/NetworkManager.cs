using Client.Session;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Session;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetworkManager {

    IPEndPoint m_loginEndPoint = null;
    private Connector _connector = new Connector();
    private ServerSession m_loginSession = new ServerSession();
    public ServerSession LoginSession { get => m_loginSession; }

    private Listener _listener = new Listener();

    public long LocalIPAddress { get; private set; }
    public int LocalPort { get; private set; }

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

        pEndPoint pEndPoint = new pEndPoint();
        pEndPoint.IpAddress = endPoint.Address.Address;
        pEndPoint.Port = endPoint.Port;
        standby.EndPoint = pEndPoint;

        m_loginSession.Send(standby);
    }
}
