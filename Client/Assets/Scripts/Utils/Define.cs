using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    [Serializable]
    public struct PlayerStat {
        [Header("Health Point Variables")]
        public float Health;
        public float MaxHealth;

        [Header("Speed Variables")]
        public float RunSpeed;
        public float WalkSpeed;
        public float CrouchWalkSpeed;
        public float JumpForce;
    }
}
