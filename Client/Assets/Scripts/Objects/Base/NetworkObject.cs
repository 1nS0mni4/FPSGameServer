using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface NetworkObject {
    public uint AuthCode { get; set; }
}
public interface CharacterObject : NetworkObject {
    public abstract Quaternion RotateDir { get; set; }
}

public interface StaticObject : NetworkObject {

}