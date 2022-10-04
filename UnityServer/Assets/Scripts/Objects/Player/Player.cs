using Client.Session;
using Extensions;
using Google.Protobuf.Protocol;
using Server.Session;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : Character {
    #region Components & GameObjects
    private PlayerMovement _movement = null;


    /// <summary>
    /// 나중에 총 오브젝트 만들게 되면 여기다가 넣기.
    /// </summary>
    private GameObject gunObject = null;

    #endregion

    #region Variables
    private Vector3 moveDirection = Vector3.zero;
    #endregion

    #region Properties
    public uint AuthCode { get; private set; }
    public bool[] Inputs { get; set; } = new bool[7];

    #endregion

    #region Unity Event Functions
    protected override void OnAwakeEvent() {
        _movement = GetComponent<PlayerMovement>();
    }

    private void Update() {
        float vertical = (Inputs[(int)pInputMovementType.InputForward] ? 1 : 0) + (Inputs[(int)pInputMovementType.InputBackward] ? -1 : 0);
        float horizontal = (Inputs[(int)pInputMovementType.InputLeft] ? -1 : 0) + (Inputs[(int)pInputMovementType.InputRight] ? 1 : 0);

        moveDirection = new Vector3(horizontal, 0, vertical).normalized;
    }

    private void FixedUpdate() {
        _movement.MoveTo(moveDirection, Inputs);

        if(ServerManager.ServerTick % 2 == 0)
            SendSync();
    }

    private void OnTriggerEnter(Collider other) {
        //TODO: 총알 트리거로 만들거임
    }

    #endregion

    #region Override Functions
    public void RotateTo(Quaternion camFront) {
        transform.rotation = camFront;
    }

    public override void SetCharacterHealth(float damage) {

    }

    public override void OnDeath() {

    }

    private void SendSync() {
        S_Broadcast_Player_Move move = new S_Broadcast_Player_Move();
        move.AuthCode = AuthCode;
        move.ServerTick = ServerManager.ServerTick;
        move.NewPosition = transform.position.TopVector3();
        move.CamFront = transform.rotation.TopQuaternion();

        InGameSceneManager.Instance.Broadcast(move);
    }

    public void SetAuth(uint authCode) { AuthCode = authCode; }

    #endregion
}