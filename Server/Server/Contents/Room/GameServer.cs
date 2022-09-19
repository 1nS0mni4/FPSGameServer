using Google.Protobuf.Protocol;
using Server.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.Contents.Room {
    public class GameServer {
        public uint ServerID { get; set; }
        public pAreaType AreaType { get; set; }
        public GameServerSession Session { get; private set; } = null;
        public pEndPoint EndPoint { get; private set; } = null;
        public bool ServerInitialized { get; private set; } = false;

        public bool CanAccess { get; private set; } = false;

        public void InitializeServer(GameServerSession session, S_Login_Game_Standby serverInfo) {
            Session = session;
            EndPoint = serverInfo.EndPoint;
            AreaType = serverInfo.AreaType;

            ServerInitialized = true;
        }
    }
}
