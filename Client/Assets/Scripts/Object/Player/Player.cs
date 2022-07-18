using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerStat))]
[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour
{
    private int userID = -1;
    public int UserID { get { return userID; } }


    protected virtual void Awake() {

    }
}
