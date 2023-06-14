using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotGun : Gun
{
    [SerializeField] Camera cam;

    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public override void Use()
    {
        Shoot();
    }

    void Shoot()
    {
        // Khởi tạo tia Ray từ Camera đi qua vị trí giữa màn hình
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        // Kiểm tra tia Ray có va chạm với bất kỳ đối tượng nào trong không gian 3D không 
        /*
         RaycastHit được sử dụng để lưu trữ thông tin va chạm
            collider: đối tượng Collider mà tia Ray đã va chạm tới
            point: Vị trí trong không gian 3D mà tia Ray đã va chạm với đối tượng 
            normal: Vector hướng pháp tuyến của bề mặt mà tia Ray va chạm
            distance: khoảng cách từ điểm xuất phát của tia Ray đến điểm va chạm  
        */
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Gọi phương thức TakeDamage trên một thành phần IDamageable của game object được va chạm
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
            // PV.RPC được sử dụng để gửi một Remote Procedure Call từ một đối tượng PhotonView hiện tại đến tất cả các người chơi trong mạng
            // RpcTarget tham số xác định muc tiêu của RPC (All đại diện cho tất cả)
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        }
    }

    // PunRPC cho phép được gọi từ một máy khách và thực thi trên tất cả máy chủ và máy khách khác trong trong mạng
    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        // Biểu diễn viên đạn
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if(colliders.Length != 0)
        {
            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal*0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
            Destroy(bulletImpactObj, 10f);
            bulletImpactObj.transform.SetParent(colliders[0].transform);
        }
    }
}
