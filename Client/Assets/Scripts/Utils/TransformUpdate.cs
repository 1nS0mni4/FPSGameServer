using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformUpdate : IComparer<TransformUpdate>, IComparer<uint> {
    public uint Tick { get; private set; }
    public Vector3 Position { get; private set; }

    public TransformUpdate(uint tick, Vector3 position) {
        Tick = tick;
        Position = position;
    }

    public int Compare(TransformUpdate x, TransformUpdate y) {
        if(x == null || y == null)
            return 0;
        return x.Tick.CompareTo(y.Tick);
    }

    public int Compare(uint x, uint y) {
        return x <= y ? -1 : 1;
    }
}
