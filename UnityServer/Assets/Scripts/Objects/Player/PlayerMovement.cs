using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour {
    #region Components & GameObjects
    private CharacterController _controller = null;
    private Animator            _animator   = null;
    [SerializeField] private PlayerStat          _stat;

    #endregion

    #region Value_Type Variables
    private Vector3       moveForce;
    private float _currentSpeed = 0.0f;

    #endregion

    #region Properties
    public bool IsGrounded { get => _controller.isGrounded; }

    #endregion

    #region Unity Event Functions

    private void Awake() {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start() {
        StartCoroutine(CoEffectGravity());
    }

    private void OnDestroy() {
        StopCoroutine(CoEffectGravity());
    }

    #endregion

    /// <summary>
    /// CharacterController를 통해 캐릭터를 직접 움직이는 함수.
    /// </summary>
    /// <param name="direction">플레이어의 로컬 이동방향</param>
    /// <param name="inputs">달리기, 앉기, 점프 등을 가지는 변수</param>
    public void MoveTo(Vector3 direction, bool[] inputs) {
        direction = transform.rotation * new Vector3(direction.x, 0, direction.z);
        _currentSpeed = inputs[(int)pInputMovementType.InputSprint] ? _stat.RunSpeed : inputs[(int)pInputMovementType.InputCrouch] ? _stat.CrouchWalkSpeed : _stat.WalkSpeed;

        moveForce = new Vector3(direction.x * _currentSpeed, moveForce.y, direction.z * _currentSpeed);

        if(inputs[(int)pInputMovementType.InputJump]) Jump();

        _controller.SimpleMove(moveForce);
    }

    public void Jump() {
        if(_controller.isGrounded == false)
            return;

        moveForce += new Vector3(0, _stat.JumpForce, 0);
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

}