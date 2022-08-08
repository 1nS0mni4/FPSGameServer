using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public enum SlotType
{
    Inventory, Storage
}

// 기획상나오는 스롯들의 기능 함수를 적어 click, drag, drop
public class Slot : MonoBehaviour, IPointerClickHandler
{
    private ItemContainer itemContainer;
    private Image image;
    public SlotItem item;
    public SlotType slotType;
    private Container container;

    [SerializeField]
    private GameObject childImg;
    [SerializeField]
    private GameObject childNameText;
    [SerializeField]
    private GameObject childTypeText;

    public GameObject Childimg
    {
        get
        {
            return childImg;
        }
        set
        {
            childImg = value;
            childImg = this.gameObject.transform.GetChild(0).gameObject;
        }
    }

    public GameObject ChildNameText
    {
        get
        {
            return childNameText;
        }
        set
        {
            childNameText = value;
            childNameText = this.gameObject.transform.GetChild(2).gameObject;
        }
    }

    public GameObject ChildTypeText
    {
        get
        {
            return childTypeText;
        }
        set
        {
            childTypeText = value;
            childTypeText = this.gameObject.transform.GetChild(1).gameObject;
        }
    }

    private void OnValidate()
    {
        Childimg = childImg;
        ChildNameText = childNameText;
        ChildTypeText = childTypeText;
    }

    private void Awake()
    {
        container = FindObjectOfType<Container>();
        image = GetComponent<Image>();
        itemContainer = FindObjectOfType<ItemContainer>();
        UpdateImage(gameObject);
    }

    private void UpdateImage(GameObject targetObject)
    {
        Slot target = targetObject.GetComponent<Slot>();

        target.Childimg.GetComponent<Image>().sprite = target.item.itemImage;
        target.ChildNameText.GetComponent<Text>().text = target.item.itemName;

        if (target.item.itemType != ItemType.none)
        {
            target.ChildTypeText.GetComponent<Text>().text = target.item.itemType.ToString();
        }

        if (target.item.itemType == ItemType.none)
        {
            target.ChildTypeText.GetComponent<Text>().text = null;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (slotType == SlotType.Inventory)
        {
            if (container.isStorage == false)
            {
                return;
            }

            for (int i = 0; i < container.inventorySlotParent.childCount; i++)
            {
                if (container.storageSlots[i].item.itemType == ItemType.none)
                {
                    container.storageSlots[i].item = item;
                    UpdateImage(container.storageSlots[i].gameObject);
                    this.item.itemType = ItemType.none;
                    this.item.itemImage = null;
                    this.item.itemName = null;
                    UpdateImage(gameObject);
                    break;
                }
            }
        }

        if (slotType == SlotType.Storage)
        {
            for (int i = 0; i < container.inventorySlotParent.childCount; i++)
            {
                if (container.inventorySlots[i].item.itemType == ItemType.none)
                {
                    container.inventorySlots[i].item = item;
                    UpdateImage(container.inventorySlots[i].gameObject);
                    this.item.itemType = ItemType.none;
                    this.item.itemImage = null;
                    this.item.itemName = null;
                    UpdateImage(gameObject);
                    break;
                }
            }
        }
    }

}
