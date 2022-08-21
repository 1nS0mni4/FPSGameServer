using Extensions;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class MyPlayer : Character, NetworkObject {
    private C_Transform_Sync _sync;
    private WaitForSeconds _posSyncSleep = new WaitForSeconds(1f);
    private WaitForSeconds _rotSyncSleep = new WaitForSeconds(0.25f);

    private void Awake() {

    }

    private void OnEnable() {
        StartCoroutine(CoSendPositionSync());
        StartCoroutine(CoSendRotationSync());
    }

    private IEnumerator CoSendPositionSync() {
        while(true) {
            C_Transform_Sync sync = new C_Transform_Sync();
            sync.Position = transform.position.TopVector3();

            Managers.Network.Send(sync);

            yield return _posSyncSleep;
        }
    }

    private IEnumerator CoSendRotationSync() {
        while(true) {
            C_Look_Rotation rotSync = new C_Look_Rotation();
            rotSync.Rotation = transform.rotation.TopQuaternion();

            Managers.Network.Send(rotSync);

            yield return _rotSyncSleep;
        }
    }

    private void OnDestroy() {
        StopCoroutine(CoSendPositionSync());
        StopCoroutine(CoSendRotationSync());
    }
}