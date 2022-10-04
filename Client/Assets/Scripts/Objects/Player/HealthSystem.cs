using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class HealthSystem : ModifiablePlayerStatSystem<float> {
    #region Stat Variables
    [HideInInspector] public float _maxHealth = 0.0f;
    [HideInInspector] public float _minHealth = 0.0f;
    private float _curHealth = 0.0f;

    #endregion

    #region Properties
    public float CurrentHealth { get => _curHealth; }

    #endregion

    #region Event Variables
    private Action OnDeadEvent;

    #endregion

    public void AddOnDeadEvent(Action onDeadEvent) {
        OnDeadEvent -= onDeadEvent;
        OnDeadEvent += onDeadEvent;
    }

    public void RemoveOnDeadEvent(Action onDeadEvent) {
        onDeadEvent -= onDeadEvent;
    }

    public override void InitializeStat(PlayerStat statData) {
        _maxHealth = statData.MaxHealth;
        _minHealth = 0.0f;
        _curHealth = _maxHealth;
    }

    public override float ApplyValueDelta(float deltaValue) {
        float prevHealth = _curHealth;
        _curHealth = Mathf.Clamp(_curHealth + deltaValue, _minHealth, _maxHealth);

        if(_curHealth <= _minHealth)
            OnDeadEvent?.Invoke();

        return Mathf.Abs(_curHealth - prevHealth);
    }
}
