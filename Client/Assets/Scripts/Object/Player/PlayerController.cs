using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using Google.Protobuf.Protocol;

[Serializable]
public class KeyOption {
    public KeyCode MoveForward      = KeyCode.W;
    public KeyCode MoveBackward     = KeyCode.S;
    public KeyCode MoveLeft         = KeyCode.A;
    public KeyCode MoveRight        = KeyCode.D;
    public KeyCode Jump             = KeyCode.Space;
    public KeyCode Interact         = KeyCode.E;
    public KeyCode Run              = KeyCode.LeftShift;
    public KeyCode Crouch           = KeyCode.LeftControl;
    public KeyCode Reload           = KeyCode.R;

    public KeyCode Inventory        = KeyCode.Tab;
}

public class PlayerController : MonoBehaviour {
    private PlayerMovement _movement = null;
    KeyOption _keyOption = new KeyOption();

    private Action<int, bool> MouseAction;
    private ushort _mouseListener = 0;

    public void AddMouseListener(Action<int, bool> action) {
        MouseAction -= action;
        MouseAction += action;
        _mouseListener++;
    }
    public void RemoveMouseListener(Action<int, bool> action) {
        MouseAction -= action;
        _mouseListener--;
    }

    [SerializeField]
    private float rotCamXAxisSpeed = 5;
    [SerializeField]
    private float rotCamYAxisSpeed = 3;

    private float limitMinX = -80;
    private float limitMaxX = 50;
    private float eulerAngleX;
    private float eulerAngleY;

    private bool isCrouch = false;

    private bool isUIControl = false;

    private UnityEngine.Vector3 moveDir = UnityEngine.Vector3.zero;

    private void Awake() {
        Managers.Input.AddMouseInputHandler(MouseInputHandler);
        Managers.Input.AddKeyInputHandler(KeyboardInputHandler);

        _movement = GetComponent<PlayerMovement>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void MouseInputHandler() {
        if(isUIControl)
            return;

        eulerAngleX -= Input.GetAxis("Mouse Y") * rotCamXAxisSpeed;
        eulerAngleY += Input.GetAxis("Mouse X") * rotCamYAxisSpeed;

        eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);

        _movement.RotateTo(new UnityEngine.Vector3(eulerAngleX, eulerAngleY, 0));

        if(_mouseListener > 0) {
            if(Input.GetMouseButtonDown(0)) {
                MouseAction.Invoke(0, true);
            }
            else if(Input.GetMouseButtonUp(0)) {
                MouseAction.Invoke(0, false);
            }
            if(Input.GetMouseButtonDown(1)) {
                MouseAction.Invoke(1, true);
            }
            else if(Input.GetMouseButtonUp(1)) {
                MouseAction.Invoke(1, false);
            }
        }
    }
    public void KeyboardInputHandler() {
        if(isUIControl)
            return;

        #region Action Input Handler
        if(Input.GetKeyDown(_keyOption.Inventory)) {
            isUIControl = !isUIControl;
            //TODO: UI창 띄우기 등 각종 필요한 액션들 작성 필요
        }

        if(Input.GetKeyDown(_keyOption.Interact)) {
            //TODO: 상호작용 시 필요한 액션 작성 필요
        }

        #endregion

        #region Movement Input Handler
        if(_movement == null)
            return;

        if(_movement.IsGrounded == false)
            return;

        if(_keyOption == null)
            return;

        if(Input.GetKeyDown(_keyOption.Jump)) {
            _movement.Jump();
        }

        if(Input.GetKeyDown(_keyOption.Crouch)) {
            _movement.Stance = pPlayerStance.Crouch;
            isCrouch = true;
        }
        if(Input.GetKeyUp(_keyOption.Crouch)) {
            _movement.Stance = pPlayerStance.Walk;
            isCrouch = false;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        if(moveX != 0 || moveZ != 0) {
            bool isRun = false;
            if(moveZ > 0 && isCrouch == false) isRun = Input.GetKey(_keyOption.Run);

            _movement.Stance = isRun ? pPlayerStance.Run : ( isCrouch ? pPlayerStance.Crouch : pPlayerStance.Walk );
        }
        else {
            _movement.Stance = pPlayerStance.Idle;
        }

        Vector3 direction = new Vector3(moveX, 0, moveZ).normalized;

        if(moveDir.Equals(direction) == false) {
            moveDir = direction;
            //C_Move movePacket = new C_Move(){Dir = new Google.Protobuf.Protocol.Vector3()};
            //movePacket.Dir.X = moveDir.x;
            //movePacket.Dir.Z = moveDir.z;
            //movePacket.Stance = _movement.Stance;
            //입력 누르고 있는 시간 += Time.deltaTime;

            //Managers.Network.Send(movePacket);
        }
        //else if(입력 누르고 있는 시간 >= 이동 주기){
        //    Managers.Network.Send(movePacket);
        //    입력 누르고 있는 시간 = 0.0f;
        //}
        _movement.MoveTo(moveDir);
        #endregion
    }
    private float ClampAngle(float angle, float min, float max) {
        if(angle < -360) angle += 360;
        if(angle > 360) angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }

#if UNITY_EDITOR
    private void OnApplicationQuit() {
        Managers.Input.RemoveMouseInputHandler(MouseInputHandler);
        Managers.Input.RemoveKeyInputHandler(KeyboardInputHandler);
    }
#endif

    private void OnDestroy() {
        Managers.Input.RemoveMouseInputHandler(MouseInputHandler);
        Managers.Input.RemoveKeyInputHandler(KeyboardInputHandler);
    }
}