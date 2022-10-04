using Client.Session;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(MovementSystem_Remote))]
public class Player : Character{
    #region Components & GameObjects
    [Header("Player Components")]
    [SerializeField] private GameObject _arm = null;
                     private CapsuleCollider _col;
    [SerializeField] private HealthSystem _healthSystem;
    [SerializeField] private MovementSystem_Remote _movement;

    #endregion

    #region Variables

    #endregion

    #region Properties
    public HealthSystem Health { get => _healthSystem; }

    #endregion

    #region Unity Event Functions
    protected override void OnAwakeEvent() {
        base.OnAwakeEvent();
        _healthSystem = GetComponent<HealthSystem>();
        _col          = GetComponent<CapsuleCollider>();

    }

    protected override void OnEnableEvent() {
        base.OnStartEvent();
        _healthSystem.InitializeStat(_stat);
        _healthSystem.AddOnDeadEvent(OnDeath);
    }

    protected override void OnStartEvent() {

    }
    #endregion

    #region Override Functions
    public override void Move(uint serverTick, Vector3 newPosition) {
        _movement.SyncTransform(serverTick, newPosition);
    }

    public override void Rotate(Quaternion camFront) {
        transform.rotation = camFront;
    }

    public override void OnDamage(float damage) {
        float refinedDamage = damage;
        //TODO: 여기서 방어구 시스템에게 값을 전달해서 수정된 값을 전달하도록 하자.
        //refinedDamage = _armorSystem.Absorb(refinedDamage);
        _healthSystem.ApplyValueDelta(refinedDamage);
    }

    public override void OnDeath() {
        
    }

    #endregion
}