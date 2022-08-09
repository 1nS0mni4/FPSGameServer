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

    public static class pVector3Ex {
        public static pVector3 Default() {
            pVector3 res = new pVector3();
            res.X = res.Y = res.Z = 0;

            return res;
        }

        public static Vector3 toVector3(this pVector3 vector) {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        public static float Magnitude(this pVector3 vec) {
            return Mathf.Sqrt(vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z);
        }

        public static Vector3 ToUnityVector3(this pVector3 vec) {
            return new Vector3(vec.X, vec.Y, vec.Z);
        }

        public static Quaternion ToUnityQuaternion(this pVector3 vec) {
            return Quaternion.Euler(vec.X, vec.Y, vec.Z);
        }

        public static pVector3 UnityVector3(Vector3 vec) {
            pVector3 ret = new pVector3();
            ret.X = vec.x; ret.Y = vec.y; ret.Z = vec.z;
            return ret;
        }

        public static pVector3 UnityQuaternion(Quaternion quat) {
            Vector3 vec = quat.eulerAngles;
            return UnityVector3(vec);
        }
    }
}