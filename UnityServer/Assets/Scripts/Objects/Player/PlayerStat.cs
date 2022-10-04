using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PlayerStat {
    [Header("Character's About Health Variables")]
    public float Health;
    public float MaxHealth;

    [Header("Speed Variables")]
    public float RunSpeed;
    public float WalkSpeed;
    public float CrouchWalkSpeed;
    public float JumpForce;
}
