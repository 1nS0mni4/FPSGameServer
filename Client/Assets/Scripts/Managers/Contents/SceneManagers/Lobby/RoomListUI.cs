using Google.Protobuf;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomListUI : MonoBehaviour {
    [Header("RoomInfo Prefab")]
    [SerializeField]
    private GameObject roomInfoPrefab;
    [Header("RoomInfo Parent Transform")]
    [SerializeField]
    private Transform roomInfoParent;
    private List<RoomInfo> roomInfoList = new List<RoomInfo>();

    //public void CreateRoomInfo(List<GameRoomInfo> dataList) {
    //    if(dataList.Count == 0)
    //        return;

    //    Transform[] child = roomInfoParent.GetComponentsInChildren<Transform>();
    //    for(int i = 0; i < child.Length; i++) {
    //        if(child[i] != roomInfoParent)
    //            Destroy(child[i].gameObject);
    //    }

    //    for(int i = 0; i < dataList.Count; i++) {
    //        RoomInfo info = Instantiate(roomInfoPrefab, roomInfoParent).GetComponent<RoomInfo>();
    //        info.Setup(dataList[i]);
    //    }
    //}
}
