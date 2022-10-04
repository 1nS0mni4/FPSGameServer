using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public abstract class ModifiablePlayerStatSystem<T> : MonoBehaviour {
    /// <summary>
    /// ������ �⺻���� ������ �� ����մϴ�. ���� ���� �� �� ���� �����ؾ� �մϴ�.
    /// </summary>
    /// <param name="defaultStat">������ �⺻��</param>
    public abstract void InitializeStat(PlayerStat statData);

    /// <summary>
    /// ������ ���� ���� ��ȭ��ų �� ����մϴ�. 
    /// </summary>
    /// <param name="deltaValue">������ ��ȭ��. Increment: +, Decrement: -</param>
    /// <returns>���� ��ȭ��.</returns>
    public virtual T ApplyValueDelta(T deltaValue) { 
        throw new NotImplementedException("��ȣ���ڿ��� �������� �ʾҰų�, �������� ȣ���Դϴ�."); 
    }
}

public abstract class ImmutablePlayerStatSystem : MonoBehaviour {
    /// <summary>
    /// ������ �⺻���� ������ �� ����մϴ�. ���� ���� �� �� ���� �����ؾ� �մϴ�.
    /// </summary>
    /// <param name="defaultStat">������ �⺻��</param>
    public abstract void InitializeStat(PlayerStat statData);
}