using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageTarget : MonoBehaviour
{
    public bool isTarget = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            isTarget = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            isTarget = false;
        }
    }
}
