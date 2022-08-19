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
        public Quaternion rotation = Quaternion.Identity;

        public pPlayerStance stance = pPlayerStance.Idle;
        public float speed = 5.0f;

        public Player(int authCode, Vector3 position) {
            AuthCode = authCode;
            this.isSpawned = false;
            this.position = position;
            this.moveDir = Vector3.Zero;
            this.rotation = Quaternion.Identity;
            this.stance = pPlayerStance.Idle;
            this.speed = 0.0f;
        }

        public void Update(double timeDelay) {
            Move(timeDelay);
        }

        public void Move(double timeDelay) {
            switch(stance) {
                case pPlayerStance.Crouch:   { speed = 3.0f; } break;
                case pPlayerStance.Walk:     { speed = 5.0f; } break;
                case pPlayerStance.Run:      { speed = 8.0f; } break;
                case pPlayerStance.Idle:     { speed = 0.0f; } break;
            }

            //TODO: rotation * moveDir이 방향을 나타내도록 수정 필요
            position += rotation.MultiplyVector3(moveDir)
                                .Multiply(speed * ( timeDelay + 0.25f ));            
        }
    }
}
