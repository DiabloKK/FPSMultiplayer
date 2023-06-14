using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] Image healthbarImage;
    [SerializeField] GameObject ui;

    [SerializeField] GameObject cameraHolder;

    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;

    [SerializeField] Item[] items;

    int itemIndex;
    int previousItemIndex = -1;

    float verticalLookRotation;
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;

    Rigidbody rb;

    // PhotonView chức năng chính là đồng bộ hóa và quản lý trạng thái của các đối tượng trong một môi trường đa người chơi
    PhotonView PV;

    const float maxHealth = 100f;
    float currentHealth = maxHealth;

    PlayerManager playerManager;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();

        // PV.InstantiationData là một mảng dữ liệu đặc biệt được truyền vào trong quá trình khởi tạo PhontonView
        // PhotonView.Find dùng để tìm kiếm một đối tượng PhotonView trong mạng dựa trên ViewID
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        /*
        Kiểm tra đối tượng có quyền sở hữu hay không 
            Nếu có thì đổi trang bị 
            Nếu không thì phá hủy những các thành phần không cần thiết
        */
        if (PV.IsMine)
        {
            EquipItem(0);
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
            Destroy(ui);
        }
    }

    void Update()
    {
        if (!PV.IsMine)
            return;
        Look();
        Move();
        Jump();

        // Thay đổi trang bị bằng phím
        for(int i=0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }

        // Thay đổi trang bị bằng lăn chuột 
        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if(itemIndex >= items.Length - 1)
            {
                EquipItem(0);
            }
            else
            {
                EquipItem(itemIndex + 1);
            }
        } else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if(itemIndex <= 0)
            {
                EquipItem(items.Length - 1);
            }
            else
            {
                EquipItem(itemIndex - 1);
            }
        }
        //

        // Nhấp chuột phải thì sử dụng vũ khí 
        if(Input.GetMouseButton(0))
        {
            items[itemIndex].Use();
        }

        // Chết nếu rơi khỏi bản đồ 
        if(transform.position.y < -10f)
        {
            Die();
        }

    }

    // Điều khiển chuyển động player
    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }
    // Điều khiển chuyển động player


    // Thay đổi vũ khí
    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
            return;

        itemIndex = _index;

        items[itemIndex].itemGameObject.SetActive(true);

        if(previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }

        previousItemIndex = itemIndex;

        if(PV.IsMine)
        {
            // Hashtable là một cấu trúc dữ liệu trong Unity Photon Networking, giúp lữu trữ và truy cập dữ liệu theo cặp khóa - giá trị
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            //SetCustomProperties là một phương thức của lớp Player trong PhotonNetwork, cho phép thiết lập các thuộc tính tùy chỉnh cho Player 
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }


    // OnPlayerPropertiesUpdate là phương thức được gọi tự động khi thuộc tính tùy chỉnh của một Player thay đổi
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        // PV.Owner là một thuộc tính của lớp PhotonView và đại diện cho Player sở hữu PhotonView đó
        if(changedProps.ContainsKey("itemIndex") && !PV.IsMine && targetPlayer == PV.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

    void FixedUpdate()
    {
        if (!PV.IsMine)
            return;

        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    // Xử lý khi đối tượng nhận xác thương 
    public void TakeDamage(float damage)
    {
        PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
    }

    // SingleShotGun line 45
    [PunRPC]
    /*
    PhotonMessageInfo được cung cấp bởi Photon API để chứa thông tin về một tin nhắn mạng hoặc nhận thông qua mạng
        Senser: đại diện cho người gửi tin nhắn (Player) 
        timestamp: thời gian gửi tin nhắn (số giây tính từ 1/1/1970)
        ...
    */
    void RPC_TakeDamage(float damage, PhotonMessageInfo info)
    {
        currentHealth -= damage;

        healthbarImage.fillAmount = currentHealth / maxHealth;

        if(currentHealth <= 0)
        {
            Die();
            PlayerManager.Find(info.Sender).GetKill();
        }
    }

    void Die()
    {
        playerManager.Die();
    }
}
