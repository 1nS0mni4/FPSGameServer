using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    [SerializeField] 
    public Transform slotsParent;
    [SerializeField] 
    private Transform otherSlotsParent;

    public List<Slot> playerSlots;
    public List<Slot> otherSlots;

    private void Awake()
    {
        playerSlots = new List<Slot>();
        otherSlots = new List<Slot>();
        for (int i = 0; i < slotsParent.childCount; i++)
        {
            playerSlots.Add(slotsParent.GetChild(i).GetComponent<Slot>());
            playerSlots[i].slotType = SlotType.Inventory;

            otherSlots.Add(otherSlotsParent.GetChild(i).GetComponent<Slot>());
            otherSlots[i].slotType = SlotType.Storage;
        }
    }
}
