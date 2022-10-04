using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using Google.Protobuf.Protocol;

[Serializable]
public class KeyOption {
    //TODO: 전부 다 유니티 InputManager의 이름 값으로 수정 필요.
    public readonly KeyCode MoveForward      = KeyCode.W;
    public readonly KeyCode MoveBackward     = KeyCode.S;
    public readonly KeyCode MoveLeft         = KeyCode.A;
    public readonly KeyCode MoveRight        = KeyCode.D;
    public readonly KeyCode Jump             = KeyCode.Space;
    public readonly KeyCode Interact         = KeyCode.E;
    public readonly KeyCode Run              = KeyCode.LeftShift;
    public readonly KeyCode Crouch           = KeyCode.LeftControl;
    public readonly KeyCode Reload           = KeyCode.R;
           
    public readonly KeyCode Inventory        = KeyCode.Tab;
}

public class PlayerController : MonoBehaviour {


    #region Components
    [Header("Interaction Components")]
    private InteractableUI _interactUI = null;
    private InteractController _interactController = null;

    #endregion

    #region Reference-Type Variables
    private Managers       _manager   = Managers.Instance;
    private KeyOption      _keyOption = new KeyOption();

    #endregion

    #region Camera Rotation Variables
    [Header("Camera Rotation Attributes")]
    [SerializeField] private float rotCamXAxisSpeed = 5;
    [SerializeField] private float rotCamYAxisSpeed = 3;

    private float limitMinX = -90;
    private float limitMaxX = 80;
    private float eulerAngleX;
    private float eulerAngleY;

    #endregion

    #region Boolean Type Input Variables
    [Header("Boolean_type Inputs")]
    [SerializeField] private MovementSystem_Local _movement      = null;
    [HideInInspector]public  bool                 toggle_Crouch  = false;
                     private bool                 isCrouch       = false;
                     private bool                 isRun          = false;
                     private bool                 isJump         = false;
                     private bool                 moveForward    = false;
                     private bool                 moveBackward   = false;
                     private bool                 moveLeft       = false;
                     private bool                 moveRight      = false;

    #endregion

    private void Awake() {
        _movement = GetComponent<MovementSystem_Local>();
        _interactController = GetComponent<InteractController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() {
        MouseAction();
        KeyAction();
    }

    private void MouseAction() {
        if(Managers.CanInput == false)
            return;

        eulerAngleX -= Input.GetAxis("Mouse Y") * rotCamXAxisSpeed;
        eulerAngleY += Input.GetAxis("Mouse X") * rotCamYAxisSpeed;

        eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);
        _movement?.RotateTo(new Vector3(eulerAngleX, eulerAngleY, 0));

        Vector2 mouseScroll = Input.mouseScrollDelta;
        if(mouseScroll != Vector2.zero) {
            int direction = mouseScroll.y > 0 ? 1 : -1;

            if(_interactUI.IsOpen) {
                _interactUI.ScrollInteractType(direction);
            }
            else {
                //TODO: Interactable을 보고 있지 않을 때 휠을 굴리면 작동할 작업
            }
        }
    }

    private void KeyAction() {
        if(Input.GetKey(KeyCode.Escape)) {
            //TODO: UI창 띄우기
        }

        #region Ignore Input conditions
        if(_keyOption == null)
            return;

        if(_movement == null)
            return;

        if(_movement.IsGrounded == false)
            return;

        #endregion

        #region Action Input Handler
        if(Input.GetKeyDown(_keyOption.Inventory)) {
            //TODO: UI창 띄우기 등 각종 필요한 액션들 작성 필요
        }

        if(Input.GetKeyDown(_keyOption.Interact)) {
            _interactController.Interact();
        }

        #endregion

        #region Movement Input Handler

        if(toggle_Crouch)
            isCrouch = (Input.GetKeyDown(_keyOption.Crouch) ? !isCrouch : isCrouch );
        else
            isCrouch = Input.GetKey(_keyOption.Crouch);

        isRun        = Input.GetKey(_keyOption.Run);
        isJump       = Input.GetKey(_keyOption.Jump);

        moveForward  = Input.GetKey(_keyOption.MoveForward);
        moveBackward = Input.GetKey(_keyOption.MoveBackward);
        moveLeft     = Input.GetKey(_keyOption.MoveLeft);
        moveRight    = Input.GetKey(_keyOption.MoveRight);

        _movement.SetInput(new bool[7] { moveForward, moveBackward, moveLeft, moveRight, isRun, isCrouch, isJump });

        #endregion
    }

    private float ClampAngle(float angle, float min, float max) {
        if(angle < -360) angle += 360;
        if(angle > 360) angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }
}