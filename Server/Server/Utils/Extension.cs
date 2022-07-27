using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Server.Utils {
    public static class TransformEx {
        public static pTransform TopTransform(this Transform tran) {
            pTransform res = new pTransform();
            res.Position.X = tran.position.x; res.Position.Y = tran.position.y; res.Position.Z = tran.position.z;
            res.Rotation.X = tran.rotation.x; res.Rotation.Y = tran.rotation.y; res.Rotation.Z = tran.rotation.z;

            return res;
        }

        public static pTransform Default(this pTransform tran) {
            pTransform res = new pTransform();
            res.Position = new pVector3();
            res.Rotation = new pVector3();

            res.Position.X = res.Position.Y = res.Position.Z = 0;
            res.Rotation.X = res.Rotation.Y = res.Rotation.Z = 0;

            return res;
        }
    }

    public static class pVector3Ex {
        public static pVector3 Default(this pVector3 vec) {
            pVector3 res = new pVector3();
            res.X = res.Y = res.Z = 0;

            return res;
        }
    }
}
