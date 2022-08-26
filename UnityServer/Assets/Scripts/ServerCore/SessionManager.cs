using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Session {
    public class SessionManager {
        #region Singleton
        private static SessionManager _instance;
        public static SessionManager Instance { 
            get {
                if(_instance == null)
                    _instance = new SessionManager();
                return _instance; 
            } 
        }
        static SessionManager() {
            _instance = new SessionManager();
        }
        #endregion

        private Dictionary<int, PacketSession> _sessions = new Dictionary<int, PacketSession>();
        object l_sessions = new object();
        private int _sessionID = 0;
        public T Generate<T>() where T: PacketSession, new(){
            //int sessionID = Interlocked.Exchange(ref _sessionID, _sessionID + 1);
            //T session = new T(){SessionID = sessionID};

            //lock(l_sessions) {
            //    _sessions.Add(sessionID, session);
            //}

            //return session;
            return null;
        }

        public void Remove(PacketSession session) {
            //if(session != null) {
            //    lock(l_sessions) {
            //        session.Disconnect();
            //        _sessions.Remove(session.SessionID);
            //    }
            //}
        }
    }
}
