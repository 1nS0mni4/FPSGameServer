using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore {
    public class Listener {
        private Socket _listenSocket = null;
        private Func<Session> _sessionFactory;
        public void Listen(IPAddress ipAddr, int port, Func<Session> sessionFactory, int backlog = 10, int count = 1) {
            IPEndPoint endPoint = new IPEndPoint(ipAddr, port);
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory = sessionFactory;

            _listenSocket.Bind(endPoint);
            _listenSocket.Listen(backlog);

            for(int i = 0; i < count; i++) {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);

                Process process = new Process();

                RegisterAccept(args);
            }
        }

        private void RegisterAccept(SocketAsyncEventArgs args) {
            args.AcceptSocket = null;

            bool pending = _listenSocket.AcceptAsync(args);

            if(pending == false)
                OnAcceptCompleted(null, args);
        }

        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs args) {
            if(args.SocketError == SocketError.Success) {
                Session session = _sessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnect(args.AcceptSocket.RemoteEndPoint);
            }

            RegisterAccept(args);
        }
    }
}
