using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class MyPlayer : MonoBehaviour, NetworkObject {
    public int AuthCode { get; set; }


}