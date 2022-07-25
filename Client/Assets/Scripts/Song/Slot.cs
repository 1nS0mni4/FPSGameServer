using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public enum SlotType
{
    Inventory, Storage
}

// ��ȹ�󳪿��� ���Ե��� ��� �Լ��� ���� click, drag, drop
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
    private void OnValidate()//����Ƽ ������ ����Ŭ�� ������ ������ ���۵Ǳ����� ����Ǵ� �Լ�
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

    // �ϰ� ���� ������ ��ĭ�̳� , �������ֳ� 
    // �������� , �巡�׾� ��� , Ŭ�� ��������? â���Ҳ���  
    //������ ���Կ��� �巡�� , ��� , Ŭ�� .... ó���� �̺��丮 ui             

    public void OnPointerClick(PointerEventData eventData)
    {//Ŭ�������� �Լ����� Ŭ�������� ���ư��� ����

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
