using Extensions;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(MovementSystem_Local))]
[RequireComponent(typeof(HealthSystem))]
public class MyPlayer : Character{
    #region Components
    [SerializeField] private PlayerController _controller;
    [SerializeField] private MovementSystem_Local _movement;
    [SerializeField] private HealthSystem _healthSystem;
    #endregion

    #region Properties
    public bool CanInput { get; set; } = false;
    #endregion

    #region Unity Event Functions
    private void Awake() {
        _controller = GetComponent<PlayerController>();
        _movement = GetComponent<MovementSystem_Local>();
        _healthSystem = GetComponent<HealthSystem>();

    }

    private void OnEnable() {
        _movement.InitializeStat(_stat);
        _healthSystem.InitializeStat(_stat);

    }

    #endregion

    #region Override Functions
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