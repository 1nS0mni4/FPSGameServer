using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    bool activeInventory = false;
    private PlayerController playerController;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        inventoryPanel.SetActive(activeInventory);
    }

    public void Inventory()
    {
        activeInventory = !activeInventory;
        inventoryPanel.SetActive(activeInventory);

        playerController.CursorState(activeInventory);
    }
}
