using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    Spawnpoint[] spawnpoints;

    void Awake()
    {
        Instance = this;
        // Lấy danh sách các vị trí mà người chơi sẽ spawnpoint
        spawnpoints = GetComponentsInChildren<Spawnpoint>();
    }

    // Trả về vị trí ngẫu nhiên mà người chơi spawnpoint 
    public Transform GetSpawnpoint()
    {
        return spawnpoints[Random.Range(0, spawnpoints.Length)].transform;
    }

}
