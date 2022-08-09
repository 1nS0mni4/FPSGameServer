using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Server.Utils;

namespace Server.Contents.Objects.Player {
    public class Player{
        public int AuthCode { get; set; } = -1;
        public bool isSpawned = false;

        public Vector3 position = Vector3.Zero;
        public Vector3 moveDir = Vector3.Zero;
        public Vector3 rotation = Vector3.Zero;

        public pPlayerStance stance = pPlayerStance.Idle;
        public float speed = 5.0f;

        public Player(int authCode, Vector3 position) {
            AuthCode = authCode;
            this.isSpawned = false;
            this.position = position;
            this.moveDir = Vector3.Zero;
            this.rotation = Vector3.Zero;
            this.stance = pPlayerStance.Idle;
            this.speed = 0.0f;
        }

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

            //TODO: 여기서 해당 유저와의 RTT / 2 체크해서 Environment.TickCOunt64 대신 곱셈
            position = rotation * moveDir * speed * Environment.TickCount64;
        }
    }
}
