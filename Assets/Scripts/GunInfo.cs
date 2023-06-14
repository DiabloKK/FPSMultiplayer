using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Cho phép tạo mới một tài nguyên (asset) dựa trên một script hoặc một lớp cụ thể
[CreateAssetMenu(menuName = "FPS/New Gun")]
public class GunInfo : ItemInfo
{
    public float damage;
}
