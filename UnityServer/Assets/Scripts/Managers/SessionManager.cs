using Google.Protobuf;
using Server.Session;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SessionManager {
    private static SessionManager _instance = new SessionManager();
    public static SessionManager Instance { get => _instance; }

    private int _sessionId = 0;
    private Dictionary<int, PacketSession> _sessions = new Dictionary<int, PacketSession>();
    private object l_sessions = new object();

    public T Generate<T>() where T: PacketSession, new() {
        T session = new T();
        session.SessionID = Interlocked.Exchange(ref _sessionId, _sessionId + 1);

        lock(l_sessions) {
            _sessions.Add(session.SessionID, session);
        }

        return session;
    }

    public void Remove(PacketSession session) {
        if(_sessions.ContainsKey(session.SessionID) == false)
            return;

        lock(l_sessions) {
            session.Disconnect();
            _sessions.Remove(session.SessionID);
        }
    }
}
