using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore {
    public class RecvBuffer {
        private byte[] _buffer;
        private int _writePos, _readPos;

        public RecvBuffer(int chunkSize = 65535) {
            _buffer = new byte[chunkSize];
            _writePos = _readPos = 0;
        }

        public int DataSize { get { return _writePos - _readPos; } }
        public int FreeSize { get { return _buffer.Length - _writePos; } }

        public ArraySegment<byte> DataSegment { get { return new ArraySegment<byte>(_buffer, _readPos, _writePos); } }
        public ArraySegment<byte> FreeSegment { get { return new ArraySegment<byte>(_buffer, _writePos, _buffer.Length - _writePos); } }

        public bool OnWrite(int numOfBytes) {
            if(FreeSize < numOfBytes)
                return false;

            _writePos += numOfBytes;
            return true;
        }

        public bool OnRead(int numOfBytes) {
            if(DataSize < numOfBytes)
                return false;

            _readPos += numOfBytes;
            return true;
        }

        public void Clear() {
            int dataSize = DataSize;

            if(_readPos == _writePos) {
                _readPos = _writePos = 0;
                return;
            }
            else {
                Array.Copy(_buffer, _readPos, _buffer, 0, dataSize);
                _readPos = 0;
                _writePos = dataSize;
            }
        }
    }
}
