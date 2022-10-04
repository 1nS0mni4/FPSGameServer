using Extensions;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementSystem_Local : MovementSystem {
    #region Components
    private CharacterController _controller = null;
    private Animator            _animator   = null;

    #endregion

    #region Reference-Type Variables
    private NetworkManager _network = null;
    private WaitForSeconds _gravityAffectionWait;

    #endregion

    #region Movement Variables
    private Vector3     _moveForce;
    private float       _currentSpeed = 0.0f;
    private float       _maxSpeed     = 0.0f;
    private float       _neutralSpeed = 0.0f;
    private float       _minSpeed     = 0.0f;
    private float       _jumpForce    = 0.0f;

    #endregion

    #region Input Variables
    /// <summary>
    /// 하드코딩된 bool Type 배열.
    /// </summary>
    private bool[] _inputs = new bool[7];
    private C_Game_Input _inputPacket = new C_Game_Input();

    #endregion

    #region Properties
    public bool IsGrounded {
        get => _controller.isGrounded;
    }

    #endregion

    #region Unity Event Functions
    private void Awake() {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _gravityAffectionWait = new WaitForSeconds(Time.fixedDeltaTime);
        
    }

    private void Start() {
        _network = Managers.Network;
        StartCoroutine(CoEffectGravity());
    }

    private void FixedUpdate() {
        if(_network.ServerTick % 200 == 0) {
            SendInput();
        }
    }

    private void Update() {
        Move();
    }

    private void OnDestroy() {
        StopCoroutine(CoEffectGravity());
    }
    #endregion

    #region Override Functions
    public override void InitializeStat(Define.PlayerStat statData) {
        _maxSpeed = statData.RunSpeed;
        _neutralSpeed = statData.WalkSpeed;
        _minSpeed = statData.CrouchWalkSpeed;
        _jumpForce = statData.JumpForce;
    }

    public override void SyncTransform(uint serverTick, Vector3 position) {
        //TODO: 차이가 많이 심할 경우 강제 이동.
    }

    #endregion

    #region MovementSystem_Local Functions
    private void Move() {
        _controller.SimpleMove(_moveForce);
    }

    public void SetInput(bool[] inputs) {
        if(inputs[(int)pInputMovementType.InputSprint] && inputs[(int)pInputMovementType.InputCrouch])
            inputs[(int)pInputMovementType.InputCrouch] = false;
        if(inputs[(int)pInputMovementType.InputJump]) inputs[(int)pInputMovementType.InputJump] = Jump();

        _inputs = inputs;

        _currentSpeed = _inputs[(int)pInputMovementType.InputSprint] ? _maxSpeed : _inputs[(int)pInputMovementType.InputCrouch] ? _minSpeed : _neutralSpeed;

        float vertical   = (inputs[(int)pInputMovementType.InputForward] ? 1 : 0) + (inputs[(int)pInputMovementType.InputBackward] ? -1 : 0);
        float horizontal = (inputs[(int)pInputMovementType.InputLeft] ? -1 : 0)   + (inputs[(int)pInputMovementType.InputRight] ? 1 : 0);

        Vector3 direction = transform.rotation * new Vector3(horizontal, 0, vertical).normalized;
        _moveForce = new Vector3(direction.x * _currentSpeed, _moveForce.y, direction.z * _currentSpeed);
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

        _moveForce += new Vector3(0, _jumpForce, 0);
        return true;
    }

    public void RotateTo(Vector3 direction) {
        transform.rotation = Quaternion.Euler(direction.x, direction.y, 0);
    }

    #endregion

    #region Coroutines
    public IEnumerator CoEffectGravity() {
        while(true) {
            if(_controller.isGrounded)
                _moveForce.y = 0;
            else
                _moveForce += new Vector3(0, -9.8f * Time.fixedDeltaTime, 0);
            yield return _gravityAffectionWait;
        }
    }

    #endregion

}
