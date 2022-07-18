using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Contents.Objects.Player {
    public class Player {
        public bool isInterpolated = false;
        public pVector3 position = Utils.Define.Vector3.zero.TopVector3();
        public pVector3 direction = new pVector3(){ X = 0, Y = 0, Z = 0 };
        public pPlayerStance stance = pPlayerStance.Idle;
        public float speed = 5.0f;

        public void Update() {
            Move();
        }

        public void Move() {
            switch(stance) {
                case pPlayerStance.Crouch:   { speed = 3.0f; } break;
                case pPlayerStance.Walk:     { speed = 5.0f; } break;
                case pPlayerStance.Run:      { speed = 8.0f; } break;
                case pPlayerStance.Idle:     { speed = 0.0f; } break;
            }
            if(speed == 0.0f)
                return;

            float tickCount = System.Environment.TickCount;
            pVector3 moveForce = new pVector3(){X = position.X + (direction.X * speed * tickCount), Y = position.Y + (direction.Y * speed * tickCount), Z = position.Z + (direction.Z * speed * tickCount) };
            position = moveForce;
        }
    }
}
