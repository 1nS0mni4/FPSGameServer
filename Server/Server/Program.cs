using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Server.DB;
using Server.Session;
using ServerCore;

namespace Server {
    public class Program {
        private static Listener _listener = new Listener();

        public static void Main(string[] args) {
            DbCommands.InitializeDB(false);
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);
            // -> DNS에서는 아직 내 공인IP가 테이블에 저장되어있지 않아서 아마 안되는듯
            //IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 17321);

            _listener.Listen(endPoint, SessionManager.Instance.Generate<ClientSession>);

            while(true) {
                //Console.WriteLine("Server Running...");
                JobTimer.Instance.Flush();
                //Thread.Sleep(1000);
            }
        }
    }
}
