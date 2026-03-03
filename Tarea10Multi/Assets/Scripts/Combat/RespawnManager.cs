using UnityEngine;

using UnityEngine;
using Unity.Netcode;

public class ReSpawnManager : NetworkBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    public Transform GetRandomSpawnPoint()
    {
        int index = Random.Range(0, spawnPoints.Length);
        return spawnPoints[index];
    }
}

