using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class MyPlayer : Character, NetworkObject { 
    private C_Transform_Sync _sync = new C_Transform_Sync();
    private WaitForSeconds _syncSleep = new WaitForSeconds(0.25f);
    private void Awake() {
        _sync.Transform = new pTransform();
        _sync.Transform.Position = new pVector3();
        _sync.Transform.Rotation = new pVector3();
    }

    private void OnEnable() {
        StartCoroutine(CoSendTransformSync());
    }

    private IEnumerator CoSendTransformSync() {
        while(true) {
            C_Transform_Sync sync = new C_Transform_Sync();
            sync.Transform = new pTransform();
            sync.Transform.Position = new pVector3();
            sync.Transform.Rotation = new pVector3();

            sync.Transform.Position.X = transform.position.x;
            sync.Transform.Position.Y = transform.position.y;
            sync.Transform.Position.Z = transform.position.z;

            sync.Transform.Rotation.X = transform.rotation.eulerAngles.x;
            sync.Transform.Rotation.Y = transform.rotation.eulerAngles.y;
            sync.Transform.Rotation.Z = transform.rotation.eulerAngles.z;

            Managers.Network.Send(sync);

            yield return _syncSleep;
        }
    }

    private void OnDestroy() {
        StopCoroutine(CoSendTransformSync());
    }
}