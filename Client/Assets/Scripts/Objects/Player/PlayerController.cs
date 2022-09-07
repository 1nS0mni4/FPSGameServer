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

[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour {
    [Header("Interaction Ray Implements")]
    public  float               _interactDistance       = 5.0f;
    private Ray                 _interactRay;
    public  LayerMask           _interactLayerMask;
    public  InteractableUI      _interactUI             = null;
    private InteractableObject  _curInteracting         = null;

    private PlayerMovement      _movement               = null;
    private KeyOption           _keyOption              = new KeyOption();

    private Action<int, bool>   MouseClickAction;
    private ushort              _mouseClickListener     = 0;
    private Action<int>         MouseWheelAction;
    private ushort              _mouseWheelListener     = 0;

    public void AddMouseClickListener(Action<int, bool> action) {
        MouseClickAction -= action;
        MouseClickAction += action;
        _mouseClickListener++;
    }
    public void RemoveMouseClickListener(Action<int, bool> action) {
        MouseClickAction -= action;
        _mouseClickListener--;
    }
    public void AddMouseWheelListener(Action<int> action) {
        MouseWheelAction -= action;
        MouseWheelAction += action;
        _mouseWheelListener++;
    }
    public void RemoveMouseWheelListener(Action<int> action) {
        MouseWheelAction -= action;
        _mouseWheelListener--;
    }

    [SerializeField] private float rotCamXAxisSpeed = 5;
    [SerializeField] private float rotCamYAxisSpeed = 3;

                     private float limitMinX = -80;
                     private float limitMaxX = 50;
                     private float eulerAngleX;
                     private float eulerAngleY;
                     
                     private bool isCrouch = false;
                                          
                     private Vector3 moveDir = Vector3.zero;

    private void Awake() {
        Managers.Input.AddMouseInputHandler(MouseInputHandler);
        Managers.Input.AddKeyInputHandler(KeyboardInputHandler);

        _movement = GetComponent<PlayerMovement>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void MouseInputHandler() {
        eulerAngleX -= Input.GetAxis("Mouse Y") * rotCamXAxisSpeed;
        eulerAngleY += Input.GetAxis("Mouse X") * rotCamYAxisSpeed;

        eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);

        _movement.RotateTo(new Vector3(eulerAngleX, eulerAngleY, 0));

        if(_mouseClickListener > 0) {
            if(Input.GetMouseButtonDown(0)) {
                MouseClickAction.Invoke(0, true);
            }
            else if(Input.GetMouseButtonUp(0)) {
                MouseClickAction.Invoke(0, false);
            }
            if(Input.GetMouseButtonDown(1)) {
                MouseClickAction.Invoke(1, true);
            }
            else if(Input.GetMouseButtonUp(1)) {
                MouseClickAction.Invoke(1, false);
            }
        }

        Vector2 mouseScroll = Input.mouseScrollDelta;
        if(mouseScroll != Vector2.zero) {
            int direction = mouseScroll.y > 0 ? 1 : -1;

            if(_interactUI.IsOpen) {
                _interactUI.ScrollInteractType(direction);
            }
            else {

            }
        }

        RaycastHit hit;
        _interactRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        Debug.DrawRay(_interactRay.origin, _interactRay.direction, Color.red, _interactDistance);
        if(Physics.Raycast(_interactRay, out hit, _interactDistance, _interactLayerMask)) {
            if(_curInteracting != null && _curInteracting.CompareTag(hit.collider.tag))
                return;

            _curInteracting = hit.collider.GetComponent<InteractableObject>();
            _curInteracting.ShowInteractType();
        }
        else {
            if(_interactUI.IsOpen) {
                _interactUI.CloseInteractType();
                _curInteracting = null;
            }
        }
        
    }
    public void KeyboardInputHandler() {
        #region Action Input Handler
        if(Input.GetKeyDown(_keyOption.Inventory)) {
            //TODO: UI창 띄우기 등 각종 필요한 액션들 작성 필요
        }

        if(Input.GetKeyDown(_keyOption.Interact)) {
            //TODO: 상호작용 시 필요한 액션 작성 필요
            if(_interactUI == null)
                return;

            if(_curInteracting == null)
                return;

            _interactUI.Interact();
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

        Vector3 direction = new Vector3(moveX, moveDir.y, moveZ).normalized;


        if(moveDir.Equals(direction) == false) {
            moveDir = direction;
            C_Game_Move movePacket = new C_Game_Move();
            movePacket.Velocity = _movement.Velocity.TopVector3();

            Managers.Network.Send(movePacket);
        }

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