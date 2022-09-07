using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public class ObjectType {
        public static uint GetRandomAuthCode(pObjectType objectType) {
            return (uint)Random.Range((int)objectType, (int)objectType + 65535);
        }
    }
}
