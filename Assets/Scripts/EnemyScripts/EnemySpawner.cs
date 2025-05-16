using System.Collections;
using UnityEngine;
using Unity.Netcode;
using PlayerScripts;

public class FlySpawner : MonoBehaviour
{
    [Header("Spawn Interval (s)")]
    public float spawnIntervalMin = 10f;
    public float spawnIntervalMax = 20f;

    [Header("Fly Prefab Index")]
    public int flyPrefabIndex = 0;

    private NetworkSpawner netSpawner;
    private Transform player;

    private void Awake()
    {
        netSpawner = FindFirstObjectByType<NetworkSpawner>();

        if (netSpawner == null)
        {
            Debug.LogError("[FlySpawner] No NetworkSpawner found!");
        }
    }

    private void OnEnable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        }
        else
        {
            Debug.LogError("[FlySpawner] NetworkManager not found on OnEnable!");
        }
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
        }
    }

    private void OnServerStarted()
    {
        Debug.Log("[FlySpawner] Server started, scheduling spawn.");
        StartCoroutine(WaitForPlayerThenSchedule());
    }

    private IEnumerator WaitForPlayerThenSchedule()
    {
        yield return new WaitUntil(() => FindFirstObjectByType<PlayerNetwork>() != null);
        player = FindFirstObjectByType<PlayerNetwork>().transform;

        Debug.Log("[FlySpawner] Player found, scheduling first spawn.");
        ScheduleNextSpawn();
    }

    private void ScheduleNextSpawn()
    {
        float delay = Random.Range(spawnIntervalMin, spawnIntervalMax);
        Invoke(nameof(SpawnFly), delay);
    }

    private void SpawnFly()
    {
        if (player == null || netSpawner == null)
        {
            return;
        }

        Vector2 dir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = player.position + (Vector3)(dir * 10f);

        netSpawner.RequestSpawnFly(spawnPos, flyPrefabIndex);
        Debug.Log("[FlySpawner] Fly spawned at " + Time.time);

        ScheduleNextSpawn();
    }
}