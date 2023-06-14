using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    Camera cam;

    void Update()
    {
        // FindObjectOfType tìm đối tượng duy nhất với kiểu dữ liệu truyền vào (FindObjectsOfType tìm một mảng)
        if(cam == null)
            cam = FindObjectOfType<Camera>();

        if (cam == null)
            return;

        // Xoay username của những người chơi khác theo camera người chơi
        transform.LookAt(cam.transform);
        transform.Rotate(Vector3.up * 180);
    }
}
