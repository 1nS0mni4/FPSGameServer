using Extensions;
using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Define;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement_Old : MonoBehaviour {
    #region Components
    private CharacterController _controller = null;
    private Animator            _animator   = null;


    [SerializeField] private PlayerStat          _stat;

    #endregion

    #region Reference-Type Variables
    private NetworkManager _network = null;
    private WaitForSeconds _gravityAffection;
    #endregion

    #region Variables
    private Vector3       moveForce;
    private float _currentSpeed = 0.0f;

    /// <summary>
    /// 하드코딩된 bool Type 배열.
    /// </summary>
    private bool[] _inputs = new bool[7];
    private C_Game_Input _inputPacket = new C_Game_Input();

    #endregion

    #region Properties
    public bool IsGrounded { get => _controller.isGrounded; }

    #endregion

    #region Unity Event Functions
    private void Awake() {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        _gravityAffection = new WaitForSeconds(Time.fixedDeltaTime);
        _network = Managers.Network;
    }

    private void Start() {
        StartCoroutine(CoEffectGravity());
    }

    private void OnDestroy() {
        StopCoroutine(CoEffectGravity());
    }

    private void FixedUpdate() {
        //TODO: 이동 매커니즘은 여기서 구현
        Move();

        if(_network.ServerTick % 200 == 0) {
            SendInput();
        }
    }

    #endregion
    private void Move() {
        _controller.Move(moveForce * Time.fixedDeltaTime);
    }

    public void SetInput(bool[] inputs) {
        if(inputs[(int)pInputMovementType.InputSprint] && inputs[(int)pInputMovementType.InputCrouch])
            inputs[(int)pInputMovementType.InputCrouch] = false;
        if(inputs[(int)pInputMovementType.InputJump]) inputs[(int)pInputMovementType.InputJump] = Jump();

        _inputs = inputs;

        _currentSpeed = _inputs[(int)pInputMovementType.InputSprint] ? _stat.RunSpeed : _inputs[(int)pInputMovementType.InputCrouch] ? _stat.CrouchWalkSpeed : _stat.WalkSpeed;

        float vertical   = (inputs[(int)pInputMovementType.InputForward] ? 1 : 0) + (inputs[(int)pInputMovementType.InputBackward] ? -1 : 0);
        float horizontal = (inputs[(int)pInputMovementType.InputLeft] ? -1 : 0)   + (inputs[(int)pInputMovementType.InputRight] ? 1 : 0);

        Vector3 direction = transform.rotation * new Vector3(horizontal, 0, vertical).normalized;
        moveForce = new Vector3(direction.x * _currentSpeed, moveForce.y, direction.z * _currentSpeed);
    }

    private void SendInput() {
        _inputPacket.CamFront = transform.rotation.TopQuaternion();
        _inputPacket.Inputs.Clear();
        _inputPacket.Inputs.AddRange(_inputs);
        Managers.Network.Send(_inputPacket);
    }

    private bool Jump() {
        if(_controller.isGrounded == false)
            return false;

        moveForce += new Vector3(0, _stat.JumpForce, 0);
        return true;
    }

    public void RotateTo(Vector3 direction) {
        transform.rotation = Quaternion.Euler(direction.x, direction.y, 0);
    }

    public IEnumerator CoEffectGravity() {
        while(true) {
            if(_controller.isGrounded) 
                moveForce.y = 0;
            else 
                moveForce += new Vector3(0, -9.8f * Time.fixedDeltaTime, 0);
            yield return _gravityAffection;
        }
    }
}