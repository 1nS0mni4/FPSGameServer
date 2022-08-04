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

    public static class TransformEx {
        public static pTransform TopTransform(this Transform tran) {
            pTransform res = new pTransform();
            res.Position = new pVector3();
            res.Rotation = new pVector3();

            res.Position.X = tran.position.x;   
            res.Position.Y = tran.position.y;   
            res.Position.Z = tran.position.z;

            res.Rotation.X = tran.rotation.x;   
            res.Rotation.Y = tran.rotation.y;   
            res.Rotation.Z = tran.rotation.z;

            return res;
        }
    }

    public static class pVector3Ex {
        public static Vector3 toVector3(this pVector3 vector) {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }
    }
}