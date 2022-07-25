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
    private Container container;
    private Image image;
    public SlotItem item;
    public SlotType slotType;

    [SerializeField]
    private GameObject childImg;
    [SerializeField]
    private GameObject childNameText;

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
            childNameText = this.gameObject.transform.GetChild(1).gameObject;
        }
    }

    //UI Components
    private void OnValidate()//유니티 라이프 사이클과 별개로 게임이 시작되기전에 실행되는 함수
    {
        Childimg = childImg;
        ChildNameText = childNameText;
    }

    private void Awake()
    {
        container = FindObjectOfType<Container>();
        image = GetComponent<Image>();
        UpdateImage(gameObject);
    }

    private void UpdateImage(GameObject targetObject)
    {


        Slot target = targetObject.GetComponent<Slot>();


        target.Childimg.GetComponent<Image>().sprite = target.item.itemImage;
        target.ChildNameText.GetComponent<Text>().text = target.item.itemName;

    }

    // 니가 넣을 슬롯이 빈칸이냐 , 아이템있냐 
    // 갯수관련 , 드래그앤 드롭 , 클릭 먹을꺼냐? 창착할꺼냐  
    //보통은 슬롯에서 드래그 , 드롭 , 클릭 .... 처리를 이벤토리 ui             

    public void OnPointerClick(PointerEventData eventData)
    {//클릭에대한 함수구현 클릭했을때 돌아가는 로직

        if (slotType == SlotType.Inventory)
        {
            for (int i = 0; i < container.slotsParent.childCount; i++)
            {
                if (container.otherSlots[i].item.itemType == ItemType.none)
                {
                    container.otherSlots[i].item = item;
                    UpdateImage(container.otherSlots[i].gameObject);
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
            for (int i = 0; i < container.slotsParent.childCount; i++)
            {
                if (container.playerSlots[i].item.itemType == ItemType.none)
                {
                    container.playerSlots[i].item = item;
                    UpdateImage(container.playerSlots[i].gameObject);
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
