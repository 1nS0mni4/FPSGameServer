using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poolable : MonoBehaviour
{
    public Action<GameObject> _destroyEvent = null;

    public void Destroy() {
        _destroyEvent.Invoke(gameObject);
    }
}
