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

    //2022-08-02 �ؾ��� �͵� : Stance �ڵ� ��ġ, MoveDir�� MoveForce �ڵ� ��ġ, PlayerController�� PlayerMovement�ڵ� ���� �ʿ�
}