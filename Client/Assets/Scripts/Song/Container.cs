using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject storagePanel;
    public Transform inventorySlotParent;
    public Transform storageSlotParent;
    private PlayerController playerController;

    public List<Slot> inventorySlots;
    public List<Slot> storageSlots;

    public bool isInventory = false;
    public bool isStorage = false;
    public bool isListCheck = false;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        inventoryPanel.SetActive(isInventory);
        InventorySlotListSetting();
    }

    public void InventoryActive()
    {
        if (isInventory == false)
        {
            isInventory = !isInventory;
            inventoryPanel.SetActive(isInventory);
            playerController.CursorState(isInventory);
        }
        else if (isInventory == true)
        {
            isInventory = !isInventory;
            inventoryPanel.SetActive(isInventory);
            playerController.CursorState(isInventory);
        }
    }

    public void StorageActive()
    {
        if (isStorage == false)
        {
            isStorage = !isStorage;
            storagePanel.SetActive(isStorage);
            playerController.CursorState(isStorage);
        }
        else if (isStorage == true)
        {
            isStorage = !isStorage;
            storagePanel.SetActive(isStorage);
            playerController.CursorState(isStorage);
        }
    }

    public void InventorySlotListSetting()
    {
        inventorySlots = new List<Slot>();

        for (int i = 0; i < inventorySlotParent.childCount; i++)
        {
            inventorySlots.Add(inventorySlotParent.GetChild(i).GetComponent<Slot>());
            inventorySlots[i].slotType = SlotType.Inventory;
        }
    }

    public void StorageSlotListSetting()
    {
        if (isListCheck == false)
        {
            storageSlots = new List<Slot>();

            for (int i = 0; i < storageSlotParent.childCount; i++)
            {
                storageSlots.Add(storageSlotParent.GetChild(i).GetComponent<Slot>());
                storageSlots[i].slotType = SlotType.Storage;
            }
            isListCheck = true;
        }
    }

}
