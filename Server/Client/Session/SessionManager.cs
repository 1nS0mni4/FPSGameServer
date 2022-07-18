using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Session {
    public class SessionManager {
        #region Singleton
        private static SessionManager _instance;
        public static SessionManager Instance { get { return _instance; } }
        static SessionManager() { _instance = new SessionManager(); }
        #endregion

        private Dictionary<int, PacketSession> _sessions = new Dictionary<int, PacketSession>();
        object l_sessions = new object();
        private int _sessionID = 0;

        public T Generate<T>() where T : PacketSession, new() {
            lock(l_sessions) {
                int sessionID = _sessionID++;
                T session = new T(){SessionID = sessionID};
                _sessions.Add(sessionID, session);
                return session;
            }
        }

        public void Remove(PacketSession session) {
            if(session != null) {
                lock(l_sessions) {
                    session.Disconnect();
                    _sessions.Remove(session.SessionID);
                }
            }
        }
    }
}
