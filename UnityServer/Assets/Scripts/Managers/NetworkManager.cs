using Client.Session;
using Google.Protobuf;
using Server.Session;
using ServerCore;
using System.Net;

public class NetworkManager {

    IPEndPoint m_loginEndPoint = null;
    private Connector _connector = new Connector();
    private ServerSession m_loginSession = new ServerSession();

    private Listener _listener = new Listener();

    public string LocalHostName { get; private set; }
    public int LocalPort { get; private set; } = 54321;

    public void ConnectTo(IPAddress ipAddress, int port) {
        m_loginEndPoint = new IPEndPoint(ipAddress, port);

        _connector.Connect(m_loginEndPoint, () => { return m_loginSession; });
    }

    public void Listen() {
        LocalHostName = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(LocalHostName);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, LocalPort);

        _listener.Listen(endPoint, () => SessionManager.Instance.Generate<ClientSession>(), 10, 10);
    }

    public void SendToLoginServer(IMessage packet) {
        m_loginSession.Send(packet);
    }
}
