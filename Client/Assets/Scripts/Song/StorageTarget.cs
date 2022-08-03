using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageTarget : MonoBehaviour
{
    private StorageUI storageUI;

    public bool isTarget = false;

    private void Awake()
    {
        storageUI = FindObjectOfType<StorageUI>();   
    }

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
            storageUI.activeStroage = false;
            storageUI.storagePanel.SetActive(false);
        }
    }
}
