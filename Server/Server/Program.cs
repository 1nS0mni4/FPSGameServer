using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Server.Contents.Manager;
using Server.DB;
using Server.Session;
using ServerCore;

namespace Server {
    public class Program {
        public static string HostString = "";

        public static int ClientPort = 17321;
        public static int ServerPort = 35432;

        private static Listener _clientListener = new Listener();
        private static Listener _serverListener = new Listener();

        public static void Main(string[] args) {
            DbCommands.InitializeDB(true);

            HostString = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(HostString);
            IPAddress ipAddr = ipHost.AddressList[0];

            //TODO: 서버 Deploy할 때 config.json파일 따로 만들어서 거기서 포트 작성해주는게 관리하기 좋을 듯
            _clientListener.Listen(ipAddr, ClientPort, SessionManager.Instance.Generate<ClientSession>);
            _serverListener.Listen(ipAddr, ServerPort, SessionManager.Instance.Generate<GameServerSession>);

            while(true) {
                JobTimer.Instance.Flush();
            }
        }
    }
}
