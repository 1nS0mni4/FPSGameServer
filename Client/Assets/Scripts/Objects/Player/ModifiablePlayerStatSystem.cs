using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public abstract class ModifiablePlayerStatSystem<T> : MonoBehaviour {
    /// <summary>
    /// 스탯의 기본값을 설정할 때 사용합니다. 최초 생성 시 한 번만 실행해야 합니다.
    /// </summary>
    /// <param name="defaultStat">전달할 기본값</param>
    public abstract void InitializeStat(PlayerStat statData);

    /// <summary>
    /// 스탯을 일정 수준 변화시킬 때 사용합니다. 
    /// </summary>
    /// <param name="deltaValue">스탯의 변화량. Increment: +, Decrement: -</param>
    /// <returns>실제 변화량.</returns>
    public virtual T ApplyValueDelta(T deltaValue) { 
        throw new NotImplementedException("피호출자에서 구현하지 않았거나, 부적절한 호출입니다."); 
    }
}

public abstract class ImmutablePlayerStatSystem : MonoBehaviour {
    /// <summary>
    /// 스탯의 기본값을 설정할 때 사용합니다. 최초 생성 시 한 번만 실행해야 합니다.
    /// </summary>
    /// <param name="defaultStat">전달할 기본값</param>
    public abstract void InitializeStat(PlayerStat statData);
}