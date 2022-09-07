using Client.Session;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character{
    public uint AuthCode { get; set; }

    [SerializeField]
    private GameObject _arm = null;

    private Rigidbody _rigid;
    private CapsuleCollider _col;

    private float deltaTime = 0.0f;

    public Vector3 Position {
        get => transform.position;
        set => transform.position = value;
    }

    public Vector3 Velocity {
        get => _rigid.velocity;
        set => SimulatePhysics(value);
    }
    public Quaternion RotateDir { get; set; }


    protected void Awake() {
        _rigid = GetComponent<Rigidbody>();
        _col = GetComponent<CapsuleCollider>();
    }

    private void Update() {
        transform.rotation = Quaternion.Slerp(transform.rotation, RotateDir, 5.0f * Time.deltaTime);
    }

    private void SimulatePhysics(Vector3 velocity) {
        _rigid.velocity = velocity;

        deltaTime += Managers.Network.PingTime / 1000;

        while(deltaTime >= Time.fixedDeltaTime) {
            deltaTime -= Time.fixedDeltaTime;
            Physics.Simulate(Time.fixedDeltaTime);
        }
    }

    public void Jump() {
        
    }
}