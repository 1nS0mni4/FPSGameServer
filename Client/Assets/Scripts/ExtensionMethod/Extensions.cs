using Google.Protobuf.Protocol;
using System;
using UnityEngine;

namespace Extensions {
    public static class ActionEx {
        public static void AddListener(this Action<int, bool> action, Action<int, bool> param) {
            RemoveListener(action, param);
            action += param;
        }

        public static void RemoveListener(this Action<int, bool> action, Action<int, bool> param) {
            action -= param;
        }
    }

    public static class Vector3Ex {
        public static pVector3 TopVector3(this Vector3 vec) {
            pVector3 pVec = new pVector3();
            pVec.X = vec.x; pVec.Y = vec.y; pVec.Z = vec.z;
            return pVec;
        }
    }

    public static class QuaternionEx {
        public static pQuaternion TopQuaternion(this Quaternion quat) {
            pQuaternion pQuat = new pQuaternion();
            pQuat.X = quat.x; pQuat.Y = quat.y; pQuat.Z = quat.z; pQuat.W = quat.w;
            return pQuat;
        }
    }

    public static class pVector3Ex {
        public static Vector3 toVector3(this pVector3 vector) {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        public static float Magnitude(this pVector3 vec) {
            return Mathf.Sqrt(vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z);
        }

        public static Vector3 ToUnityVector3(this pVector3 vec) {
            return new Vector3(vec.X, vec.Y, vec.Z);
        }
    }

    public static class pQuaternionEx {
        public static Quaternion ToUnityQuaternion(this pQuaternion pQuat) {
            return new Quaternion(pQuat.X, pQuat.Y, pQuat.Z, pQuat.W);
        }

        public static Quaternion ToUnityQuaternion(this pVector3 vec) {
            return Quaternion.Euler(vec.X, vec.Y, vec.Z);
        }
    }
}