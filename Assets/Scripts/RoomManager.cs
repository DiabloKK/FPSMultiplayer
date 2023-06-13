using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    void Awake()
    {
        if(Instance)
        {
            Destroy(gameObject);
            return;
        }
        // được gọi để đảm bảo rằng đối tượng RoomManager không bị hủy bỏ khi chuyển scene 
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    // Hàm OnEnable được gọi khi đối tượng kích hoạt 
    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceceLoaded;
    }
    
    // Hàm OnDisable được gọi tự động khi một đối tượng bị vô hiệu hóa
    public override void OnDisable()
    {
        base.OnDisable();
        /*
        Trong SceneManager của Unity, thuộc tính 'sceneLoaded' là một sự kiện được kích hoạt
        khi một scene mới được tải và hoàn tất quá trình tải scene
        */
        SceneManager.sceneLoaded -= OnSceceLoaded;
    }

    void OnSceceLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if(scene.buildIndex == 1)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
    }

}
