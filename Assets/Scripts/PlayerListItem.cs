using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text text;
    Player player;

    public void SetUp(Player _player)
    {
        player = _player;
        text.text = _player.NickName;
    }

    // Được gọi khi một người chơi rời khỏi phòng chơi game hiện tại nhận từ Photon
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("HELLO");
        if(player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

}
