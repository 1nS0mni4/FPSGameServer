using Google.Protobuf.Protocol;
using Server.Session;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class Player : Character {
    [SerializeField]
    private GameObject _arm = null;

    private Vector3 _position = Vector3.zero;
    private bool _posInterpolated = false;

    public ClientSession Session { get; set; }

    public override Vector3 Position {
        get => base.Position; 
        set {
            _position = value;
            _posInterpolated = true;
            //transform.position = value;
        }
    }

    protected void Awake() {
        _movement = GetComponent<PlayerMovement>();
    }

    private void Update() {
        transform.rotation = Quaternion.Slerp(transform.rotation, RotateDir, 5.0f * Time.deltaTime);
        //_arm.transform.rotation = Quaternion.Slerp(_arm.transform.rotation, RotateDir, 5.0f * Time.deltaTime);
    }

    public void FixedUpdate() {
        _movement.MoveTo(MoveDir);

        if(_posInterpolated) {
            transform.position = Vector3.Lerp(transform.position, _position, 10f);
            _posInterpolated = false;
        }
    }

    public void Jump() {
        _movement.Jump();
    }
}