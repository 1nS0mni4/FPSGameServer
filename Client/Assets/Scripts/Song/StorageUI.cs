using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageUI : MonoBehaviour
{
    public GameObject storagePanel;    
    private PlayerController playerController;
    bool activeStroage = false;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        storagePanel.SetActive(activeStroage);
    }

    public void Storage()
    {
        activeStroage = !activeStroage;
        storagePanel.SetActive(activeStroage);

        playerController.CursorState(activeStroage);
    }
}
