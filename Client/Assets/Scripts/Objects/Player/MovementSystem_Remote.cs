using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class MovementSystem_Remote : MovementSystem {
    private CapsuleCollider _collider = null;

    [SerializeField] private float timeElapsed = 0f;
    [SerializeField] private float timeToReachTarget = 0.05f;
    [SerializeField] private float movementThreshold = 0.05f;

    private readonly List<TransformUpdate> futureTransformUpdates = new List<TransformUpdate>();
    private float squareMovementThreshold;
    private TransformUpdate to;
    private TransformUpdate from;
    private TransformUpdate previous;

    private void Start() {
        squareMovementThreshold = movementThreshold * movementThreshold;
        to = new TransformUpdate(Managers.Network.ServerTick, transform.position);
        from = new TransformUpdate(Managers.Network.InterpolationTick, transform.position);
        previous = new TransformUpdate(Managers.Network.InterpolationTick, transform.position);
    }

    private void Update() {
        for(int i = 0; i < futureTransformUpdates.Count; i++) {
            if(Managers.Network.ServerTick >= futureTransformUpdates[i].Tick) {
                previous = to;
                to = futureTransformUpdates[i];
                from = new TransformUpdate(Managers.Network.InterpolationTick, transform.position);

                futureTransformUpdates.RemoveAt(i);
                i--;

                //얘를 초기화한 이유?: 여기에 등록 되었다는건 최근 위치라는건데, 이동속도를 조절하기 위해선 0으로 초기화 해야한다.
                timeElapsed = 0f;
                //Tick은 0.025초마다 1씩 증가되는데, to.Tick - from.Tick은 두 위치를 이동하기 위한 틱값이 나온다. 1틱 당 0.025초씩 걸린다고 예상하기 때문에 뺀 값에 0.025f를 곱한다.
                timeToReachTarget = ( to.Tick - from.Tick ) * Time.fixedDeltaTime;
            }
        }

        //얘는 for안에 못들어갔을 때 이전 위치를 계속 가지고 있기 때문에 빠르게 이동시켜줘야 다음 위치를 맞춰줄 수 있기 때문에 시간을 더해준다.
        timeElapsed += Time.deltaTime;

        //
        InterpolatePosition(timeElapsed / timeToReachTarget);
    }

    private void InterpolatePosition(float lerpAmount) {
        if(( to.Position - from.Position ).sqrMagnitude < squareMovementThreshold) {
            if(to.Position != from.Position)
                transform.position = Vector3.Lerp(from.Position, to.Position, lerpAmount);

            return;
        }

        transform.position = Vector3.LerpUnclamped(from.Position, to.Position, lerpAmount);
    }

    public override void SyncTransform(uint tick, Vector3 position) {
        if(tick <= Managers.Network.InterpolationTick)
            return;

        for(int i = 0; i < futureTransformUpdates.Count; i++) {
            if(tick < futureTransformUpdates[i].Tick) {
                futureTransformUpdates.Insert(i, new TransformUpdate(tick, position));
                return;
            }
        }

        futureTransformUpdates.Add(new TransformUpdate(tick, position));
    }

    public override void InitializeStat(Define.PlayerStat statData) {
        throw new System.NotImplementedException();
    }
}
