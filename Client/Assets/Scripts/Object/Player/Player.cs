using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour, DynamicObject {
    private PlayerMovement _movement = null;
    public pPlayerStance Stance { get => _movement.Stance; set {
            _movement.Stance = value;
        }
    }

    public Vector3 MoveDir { get; set; }
    public int AuthCode { get; set; }

    protected virtual void Awake() {
        _movement = GetComponent<PlayerMovement>();
    }

    public void Jump() {
        _movement.Jump();
    }

    //2022-08-02 해야할 것들 : Stance 코드 위치, MoveDir과 MoveForce 코드 위치, PlayerController와 PlayerMovement코드 정리 필요
}