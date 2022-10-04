using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementSystem : ImmutablePlayerStatSystem {
    public abstract void SyncTransform(uint serverTick, Vector3 position);
}