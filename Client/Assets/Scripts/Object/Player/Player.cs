using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class Player : Character, CharacterObject {
    [SerializeField]
    private GameObject _arm = null;

    public override Vector3 Position {
        get => base.Position; set {
            _movement.transform.position = value;
        }
    }

    protected void Awake() {
        _movement = GetComponent<PlayerMovement>();
    }
    private void Update() {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, RotateDir.y, 0)), 5.0f * Time.deltaTime);
        _arm.transform.rotation = Quaternion.Slerp(_arm.transform.rotation, Quaternion.Euler(new Vector3(RotateDir.x, 0, 0)), 5.0f * Time.deltaTime);
    }

    public void FixedUpdate() {
        _movement.MoveTo(MoveDir);

    }

    public void Jump() {
        _movement.Jump();
    }
}