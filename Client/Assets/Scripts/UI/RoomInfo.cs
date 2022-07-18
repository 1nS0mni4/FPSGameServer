using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct RoomInfoData {
    public int roomID;
    public string roomName;
    public int curCount;
    public int maxCount;
}
public class RoomInfo : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI _roomIDText;
    [SerializeField]
    private TextMeshProUGUI _roomNameText;
    [SerializeField]
    private TextMeshProUGUI _capacityText;
    [SerializeField]
    private Button button_Enter;

    private string color = string.Empty;

    private int _roomID = -1;

    //public void Setup(GameRoomInfo data) {
    //    _roomID = data.RoomID;
    //    _roomIDText.text = data.RoomID.ToString();
    //    _roomNameText.text = data.RoomName;
    //    color = ( data.MaxCapacity - data.ConnectedCount ) > 1 ? "green" : "red";
    //    _capacityText.text = $"<size=40><color={color}>{data.ConnectedCount}</color>/</size>{data.MaxCapacity}";
    //    if(data.MaxCapacity <= data.ConnectedCount)
    //        button_Enter.interactable = false;
    //}

    public void OnEnterClicked() {

    }
}
