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

                //�긦 �ʱ�ȭ�� ����?: ���⿡ ��� �Ǿ��ٴ°� �ֱ� ��ġ��°ǵ�, �̵��ӵ��� �����ϱ� ���ؼ� 0���� �ʱ�ȭ �ؾ��Ѵ�.
                timeElapsed = 0f;
                //Tick�� 0.025�ʸ��� 1�� �����Ǵµ�, to.Tick - from.Tick�� �� ��ġ�� �̵��ϱ� ���� ƽ���� ���´�. 1ƽ �� 0.025�ʾ� �ɸ��ٰ� �����ϱ� ������ �� ���� 0.025f�� ���Ѵ�.
                timeToReachTarget = ( to.Tick - from.Tick ) * Time.fixedDeltaTime;
            }
        }

        //��� for�ȿ� ������ �� ���� ��ġ�� ��� ������ �ֱ� ������ ������ �̵�������� ���� ��ġ�� ������ �� �ֱ� ������ �ð��� �����ش�.
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
