using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Server.Utils {
    public class Define {
        public struct Vector3 {
            public float x, y, z;

            public Vector3(float x, float y, float z) {
                this.x = x;
                this.y = y;
                this.z = z;
            }
            public static Vector3 zero { get { return new Vector3(0, 0, 0); } }
            public static Vector3 operator +(Vector3 a, Vector3 b) { return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z); }
            public pVector3 TopVector3() { return new pVector3() { X = x, Y = y, Z = z }; }
        }
    }
}
