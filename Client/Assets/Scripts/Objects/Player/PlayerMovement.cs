using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PlayerStat))]
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour {
    private CharacterController _controller = null;
    private PlayerStat          _stat       = null;
    private Animator            _animator   = null;

    private Vector3       moveForce;
    private pPlayerStance _curStance        = pPlayerStance.Walk;
    

    public Vector3 Velocity { get => _controller.velocity; }
    public bool IsGrounded { get => _controller.isGrounded; }
    public pPlayerStance Stance {
        get => _curStance;
        set {
            if(_curStance == value)
                return;

            _curStance = value;
            switch(value) {
                case pPlayerStance.Idle: {
                    _stat.CurrentSpeed = 0;
                }break;
                case pPlayerStance.Walk: {
                    _stat.CurrentSpeed = _stat.WalkSpeed;
                }break;
                case pPlayerStance.Crouch: {
                    _stat.CurrentSpeed = _stat.CrouchWalkSpeed;
                }break;
                case pPlayerStance.Run: {
                    _stat.CurrentSpeed = _stat.RunSpeed;
                }break;
            }

            _animator.SetFloat("Speed", _stat.CurrentSpeed);
        }
    }

    private void Awake() {
        _controller = GetComponent<CharacterController>();
        _stat = GetComponent<PlayerStat>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start() {
        StartCoroutine(CoEffectGravity());
    }

    public void MoveTo(Vector3 direction) {
        direction = transform.rotation * new Vector3(direction.x, 0, direction.z);
        moveForce = new Vector3(direction.x * _stat.CurrentSpeed, moveForce.y, direction.z * _stat.CurrentSpeed);

        _controller.Move(moveForce * Time.deltaTime);
    }

    public void Jump() {
        if(_controller.isGrounded == false)
            return;
        moveForce += new Vector3(0, _stat.JumpForce, 0);
        _controller.Move(moveForce * Time.deltaTime);
    }

    public void RotateTo(Vector3 direction) {
        transform.rotation = Quaternion.Euler(direction.x, direction.y, 0);
    }

    public IEnumerator CoEffectGravity() {
        while(true) {
            if(_controller.isGrounded) 
                moveForce.y = 0;
            else 
                moveForce += new Vector3(0, -9.8f * Time.deltaTime, 0);

            _controller.Move(moveForce * Time.deltaTime);
            yield return null;
        }
    }

    private void OnDestroy() {
        StopCoroutine(CoEffectGravity());
    }
}