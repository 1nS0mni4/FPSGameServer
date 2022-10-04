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

        private uint _maxCapacity = 0;
        private uint _curAccessedUserCount = 0;
        public bool CanAccess { get {
                if(ServerInitialized == false)
                    return false;
                return _maxCapacity - _curAccessedUserCount > 0;
            }
        } 

        public void InitializeServer(GameServerSession session, S_Login_Game_Standby serverInfo) {
            Session = session;
            EndPoint = serverInfo.EndPoint;
            AreaType = serverInfo.AreaType;
            _maxCapacity = serverInfo.Capacity;

            ServerInitialized = true;
        }

        public void InitializeServer(GameServerSession session, S_Login_Debug_Game_Standby serverInfo) {
            Session = session;
            EndPoint = serverInfo.EndPoint;
            AreaType = serverInfo.AreaType;
            _maxCapacity = serverInfo.Capacity;

            ServerInitialized = true;
        }

        public void InformUserAccess(uint userCount) {
            Interlocked.Exchange(ref _curAccessedUserCount, _curAccessedUserCount + userCount);
        }
    }
}
