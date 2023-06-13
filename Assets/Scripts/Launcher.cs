using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
   
    public static Launcher Instance;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    /*
    Transform là một thành phần của một đối tượng trong scene 
    Chứa các thuộc tính:
        position
        rotation
        scale
    */
    [SerializeField] Transform roomListContent;
    /*
     GameObject đại diện cho một thực thể
     Chứa nhiều thành phần ví dự Transform, Rendere, AudioSource,...
     */
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameButton;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Debug.Log("Connecting to Master");
        // Kết nối đến máy chủ của Photon
        PhotonNetwork.ConnectUsingSettings();
    }

    // OnConnectedToMaster được gọi tự động khi kết nối thành công tới máy chủ master của Photon
    public override void OnConnectedToMaster() {
        Debug.Log("Connected to Master");
        // JoinLobby gọi để tham gia vào phòng chờ (lobby) của Photon.
        PhotonNetwork.JoinLobby();
        // Tự động đồng bộ hóa cảnh giữa các người chơi trong phòng 
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby() {
        // Mở menu title 
        MenuManager.Instance.OpenMenu("title");
        Debug.Log("Joined Lobby");
    }

    public void CreateRoom() {
        if(string.IsNullOrEmpty(roomNameInputField.text)) {
            return;
        }
        // Tạo room theo tên của người chơi và kết nối người chơi vào phòng tương ứng
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.Instance.OpenMenu("loading");
    }


    // OnJoinedRoom được gọi khi một người chơi kết nối thành công và tham gia vào một phòng chơi game
    public override void OnJoinedRoom() {
        MenuManager.Instance.OpenMenu("room");
        // Lấy tên phòng ra để hiển thị 
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        // Danh sách người chơi được lấy từ PhotonNetwork.PlayerList  
        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < players.Length; i++)
        {
            // Instantiate dùng để tạo một thể hiện (instances) mới của một đối tượng từ một prelab
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        // IsMasterClient là thuộc tính được dùng để kiểm tra xem người chơi hiện tại có phải là MasterClient hay không
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    // OnMasterClientSwitched hàm này được tự động gọi khi vai trò của MasterClient trong phòng game được chuyển giao cho một người khác 
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    // Được gọi khi việc tạo phòng chơi game bị thất bại 
    public override void OnCreateRoomFailed(short returnCode, string message) {
        errorText.text =  "Room Creation Failed: " + message;
        MenuManager.Instance.OpenMenu("error");
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void LeaveRoom() {
        // Khi gọi hàm này, người chơi hiện tại sẽ rời khỏi phòng chơi game
        foreach (Transform trans in playerListContent)
        {
            Destroy(trans.gameObject);
        }
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        // JoinRoom được sử dụng để tham gia vào một phòng chơi game đã tồn tại
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");
    }

    // OnLeftRoom được gọi tự động sau khi quá trình rời phòng hoàn thành ( rời phòng bằng hàm LeaveRoom)
    public override void OnLeftRoom() {
        MenuManager.Instance.OpenMenu("title");
    }

    // OnRoomListUpadate được gọi khi danh sách các phòng chơi game được cập nhật (ví dụ phòng mới được tạo ra hoặc một phòng bị xóa)
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }

    // OnPlayerEnteredRoom được gọi khi một người chơi khác gia nhập vào phòng chơi game hiện tại 
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
     
}
