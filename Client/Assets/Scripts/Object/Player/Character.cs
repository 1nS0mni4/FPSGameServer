using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour { 
    public int AuthCode { get; set; }

    [SerializeField] protected PlayerMovement _movement = null;

    public virtual Vector3 MoveDir { get; set; }
    public virtual Vector3 RotateDir { get; set; }
    public virtual Vector3 Position { get => transform.position; set => transform.position = value; }
    public virtual pPlayerStance Stance { 
        get {
            if(_movement == null)
                return pPlayerStance.Nostance;

            return _movement.Stance; 
        } 
        set {
            if(_movement != null)
                _movement.Stance = value;
        } 
    }
}
