using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using TMPro;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    /*
    RoomInfo là một lớp trong thư viện Photon PUN
    Chứa các thuộc tính:
        Name
        PlayerCount
        MaxPlayers
        IsOpen
        Isvible
        CustomProperties
    */
    public RoomInfo info;

    public void SetUp(RoomInfo _info)
    {
        info = _info;
        text.text = _info.Name;
    }

    public void OnClick()
    {
        Launcher.Instance.JoinRoom(info);
    }
}
