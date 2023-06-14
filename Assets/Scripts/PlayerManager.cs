using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviour
{

    PhotonView PV;

    GameObject controller;

    int kills;
    int deaths;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if(PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        // Lấy điểm spawnpoint ngẫu nhiên 
        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
        // Tạo một đối tượng PlayerController trong môi trường đa người chơi
        // new object[] { PV.ViewID } bổ sung tham số ViewID cho đối tượng PlayerController
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
    }
    
    public void Die()
    {
        // Xóa đối tượng controller khỏi mạng 
        PhotonNetwork.Destroy(controller);
        // Khởi tạo lại đối tượng controller
        CreateController();

        deaths++;

        // PlayerController line 173
        Hashtable hash = new Hashtable();
        hash.Add("deaths", deaths);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public void GetKill()
    {
        PV.RPC(nameof(RPC_GetKill), PV.Owner);
    }

    // PunRPC SingleShotGun line 45
    [PunRPC]
    void RPC_GetKill()
    {
        kills++;

        // PlayerController line 173
        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public static PlayerManager Find(Player player)
    {
        // Tìm đối tượng "PlayerManager" duy nhất trong danh sách 
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
    }

}
