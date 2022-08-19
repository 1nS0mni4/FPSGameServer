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
    }

    public static class Vector3Ex {
        public static Vector3 Multiply(this Vector3 vec, double value) {
            return Multiply(vec, (float)value);
        }

        public static Vector3 Multiply(this Vector3 vec, float value) {
            return new Vector3(vec.X * value, vec.Y * value, vec.Z * value);
        }

        public static pVector3 TopVector3(this Vector3 vec) {
            pVector3 pVec = new pVector3();
            pVec.X = vec.X; pVec.Y = vec.Y; pVec.Z = vec.Z;
            return pVec;
        }
    }

    public static class QuaternionEx {
        public static pQuaternion TopQuaternion(this Quaternion quat) {
            pQuaternion pQuat = new pQuaternion();
            pQuat.X = quat.X; pQuat.Y = quat.Y; pQuat.Z = quat.Z; pQuat.W = quat.W;
            return pQuat;
        }

        public static Vector3 MultiplyVector3(this Quaternion rotation, Vector3 point) {
            float num = rotation.X * 2f;
            float num2 = rotation.Y * 2f;
            float num3 = rotation.Z * 2f;
            float num4 = rotation.X * num;
            float num5 = rotation.Y * num2;
            float num6 = rotation.Z * num3;
            float num7 = rotation.X * num2;
            float num8 = rotation.Y * num3;
            float num9 = rotation.Z * num3;
            float num10 = rotation.W * num;
            float num11 = rotation.W * num2;
            float num12 = rotation.W * num3;
            Vector3 result = default(Vector3);
            result.X = ( 1f - ( num5 + num6 ) ) * point.X + ( num7 - num12 ) * point.Y + ( num8 + num11 ) * point.Z;
            result.Y = ( num7 + num12 ) * point.X + ( 1f - ( num4 + num6 ) ) * point.Y + ( num9 - num10 ) * point.Z;
            result.Z = ( num8 - num11 ) * point.X + ( num9 + num10 ) * point.Y + ( 1f - ( num4 + num5 ) ) * point.Z;
            return result;
        }
    }
}