using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : BaseStat
{
    [Header("Speed Variables")]
    public float CurrentSpeed = 0.0f;
    public float RunSpeed = 0.0f;
    public float WalkSpeed = 0.0f;
    public float CrouchWalkSpeed = 0.0f;
    public float JumpForce = 0.0f;
}
