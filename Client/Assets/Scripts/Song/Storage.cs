using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [SerializeField]
    private GameObject Itemcontainer;
    [SerializeField]
    private GameObject storage;
    [SerializeField]
    private GameObject storagePos;

    private Container container;
    private PlayerController playerController;

    public Transform storageSlotParent;
    public List<Slot> storageSlotList;


    public bool isTarget = false;
    public bool isListCheck = false;

    private void Awake()
    {
        container = FindObjectOfType<Container>();
        playerController = FindObjectOfType <PlayerController>();
        StorageSlotList();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            isTarget = true;
            playerController.isInteract = true;
            //storage.transform.parent = container.gameObject.transform;
            PlayerController.storage = this;
            storage.transform.SetParent(Itemcontainer.gameObject.transform);
            storage.transform.position = storagePos.transform.position;
            container.storageSlotParent = storageSlotParent;
            container.storagePanel = storage;

            container.StorageSlotListSetting();
            container.storageSlots = storageSlotList;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isTarget = false;
            playerController.isInteract = false;
            PlayerController.storage = null;
            //storage.transform.parent = this.gameObject.transform;
            storage.transform.SetParent(this.gameObject.transform);
            storage = container.storagePanel;
            container.storagePanel = null;
            container.storageSlotParent = null;
        }
    }

    public void StorageSlotList()
    {
        storageSlotList = new List<Slot>();

        for (int i = 0; i < storageSlotParent.childCount; i++)
        {
            storageSlotList.Add(storageSlotParent.GetChild(i).GetComponent<Slot>());
            storageSlotList[i].slotType = SlotType.Storage;
        }
    }
}
