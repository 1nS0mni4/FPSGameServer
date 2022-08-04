using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface NetworkObject {
    public int AuthCode { get; set; }
}

public interface DynamicObject : NetworkObject {
    public Vector3 MoveDir { get; set; }
}

public interface CharacterObject : DynamicObject {
    public Vector3 RotateDir { get; set; }
}

public interface StaticObject : NetworkObject {

}