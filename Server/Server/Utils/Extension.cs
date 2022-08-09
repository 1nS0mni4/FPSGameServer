using Google.Protobuf.Protocol;
using System;
using System.Numerics;

namespace Server.Utils {
    public static class pVector3Ex {
        public static pVector3 Default() {
            pVector3 res = new pVector3();
            res.X = res.Y = res.Z = 0;

            return res;
        }

        public static float Magnitude(this pVector3 vec) {
            return MathF.Sqrt(vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z);
        }

        public static Vector3 ToVector3(this pVector3 vec) {
            return new Vector3(vec.X, vec.Y, vec.Z);
        }

        public static pVector3 Vector3(Vector3 vec) {
            pVector3 ret = new pVector3();
            ret.X = vec.X; ret.Y = vec.Y; ret.Z = vec.Z;
            return ret;
        }
    }
}
