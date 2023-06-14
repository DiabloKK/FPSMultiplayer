using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    // Thông tin của vũ khí
    public ItemInfo itemInfo;
    // Đối tượng vũ khí trong scene
    public GameObject itemGameObject;

    public abstract void Use();
}
