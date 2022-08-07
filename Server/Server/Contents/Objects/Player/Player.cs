using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Server.Utils;

namespace Server.Contents.Objects.Player {
    public class Player{
        public int AuthCode { get; set; } = -1;
        public bool isInterpolated = false;
        public pTransform transform = new pTransform().Default();
        public pVector3 direction = pVector3Ex.Default();
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


            //transform.Position += moveForce;
        }
    }
}
