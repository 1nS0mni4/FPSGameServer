using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore {
    public abstract class PacketSession : Session {
        private const ushort Header_Size = 2;
        public sealed override int OnRecv(ArraySegment<byte> segment) {
            int processLen = 0;

            while(true) {
                if(segment.Count < Header_Size)
                    break;

                ushort dataSize = BitConverter.ToUInt16(segment.Array, segment.Offset);
                if(segment.Count < dataSize)
                    break;

                OnRecvPacket(new ArraySegment<byte>(segment.Array, segment.Offset, dataSize));
                processLen += dataSize;

                segment = new ArraySegment<byte>(segment.Array, segment.Offset + dataSize, segment.Count - dataSize);
            }

            return processLen;
        }
        public abstract void OnRecvPacket(ArraySegment<byte> segment);
    }
    public abstract class Session {
        private Socket _socket;
        protected int _disconnected = 1;
        public bool Connected { get { return _disconnected == 0; } }
        public int SessionID { get; set; }

        private SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
        private RecvBuffer _recvBuffer = new RecvBuffer();

        private SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        private Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
        object l_sendQueue = new object();
        private List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        public void Start(Socket socket) {
            _socket = socket;

            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterRecv();
        }

        public void Disconnect() {
            if(Interlocked.Exchange(ref _disconnected, 1) == 1)
                return;
            OnDisconnect(_socket.RemoteEndPoint);
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
            Clear();
        }
        private void Clear() {
            lock(l_sendQueue) {
                _sendQueue.Clear();
                _pendingList.Clear();
            }
        }

        public void Send(ArraySegment<byte> segment) {
            lock(l_sendQueue) {
                _sendQueue.Enqueue(segment);
            }

            if(_pendingList.Count == 0) {
                RegisterSend();
            }
        }

        #region Network Functions
        private void RegisterSend() {
            if(_disconnected == 1)
                return;

            lock(l_sendQueue) {
                while(_sendQueue.Count > 0) { 
                    _pendingList.Add(_sendQueue.Dequeue());
                }

                _sendArgs.BufferList = _pendingList;
                bool pending = _socket.SendAsync(_sendArgs);

                if(pending == false)
                    OnSendCompleted(null, _sendArgs);
            }
        }

        private void OnSendCompleted(object sender, SocketAsyncEventArgs args) {
            if(args.SocketError == SocketError.Success && args.BytesTransferred > 0) {
                try {
                    _pendingList.Clear();

                    if(_sendQueue.Count > 0)
                        RegisterSend();
                    else {

                    }
                }catch(Exception e) {
                    Console.WriteLine($"OnSendCompleted Failed! {e}");
                }
            }
            else {
                Disconnect();
            }
        }

        private void RegisterRecv() {
            if(_disconnected == 1)
                return;

            try {
                _recvBuffer.Clear();
                ArraySegment<byte> segment = _recvBuffer.FreeSegment;
                _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);
                bool pending = _socket.ReceiveAsync(_recvArgs);

                if(pending == false)
                    OnRecvCompleted(null, _recvArgs);
            } catch(Exception e) {
                Console.WriteLine($"RegisterRecv Failed! {e}");
                Disconnect();
            }
        }

        private void OnRecvCompleted(object sender, SocketAsyncEventArgs args) {
            if(args.SocketError == SocketError.Success && args.BytesTransferred > 0) {
                try {
                    if(_recvBuffer.OnWrite(args.BytesTransferred) == false) {
                        Disconnect();
                        return;
                    }

                    int processLen = OnRecv(_recvBuffer.DataSegment);
                    if(processLen < 0 || _recvBuffer.DataSize < processLen) {
                        Disconnect();
                        return;
                    }


                    if(_recvBuffer.OnRead(args.BytesTransferred) == false) {
                        Disconnect();
                        return;
                    }

                    RegisterRecv();
                } catch(Exception e) {
                    Console.WriteLine($"OnRecvCompleted Failed! {e}");
                }
            }
            else {
                Disconnect();
            }
        }
        #endregion

        #region Abstract Functions
        public abstract void OnConnect(EndPoint endPoint);
        public abstract void OnDisconnect(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> segment);
        public abstract void OnSend(int numOfBytes);
        #endregion
    }
}
