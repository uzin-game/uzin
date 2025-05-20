using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using PlayerScripts;

public class MobSpawner : MonoBehaviour
{
    [Header("Spawn Interval (s)")]
    public float spawnIntervalMin = 10f;
    public float spawnIntervalMax = 20f;

    [System.Serializable]
    public struct SpawnableMob
    {
        [Tooltip("Name for identification only")] public string name;
        [Tooltip("Index used by NetworkSpawner to identify the prefab")] public int prefabIndex;
        [Tooltip("Relative weight for random selection")] public float weight;
    }

    [Header("Mob Types to Spawn")]
    [Tooltip("List of mob types, their prefab index, and spawn weight")]
    public List<SpawnableMob> spawnableMobs;

    private NetworkSpawner netSpawner;
    private Transform player;

    private void Awake()
    {
        netSpawner = FindFirstObjectByType<NetworkSpawner>();
        if (netSpawner == null)
            Debug.LogError("[MobSpawner] No NetworkSpawner found!");
    }

    private void OnEnable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        }
        else
        {
            Debug.LogError("[MobSpawner] NetworkManager not found on OnEnable!");
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
        Debug.Log("[MobSpawner] Server started, waiting for player...");
        StartCoroutine(WaitForPlayerThenSchedule());
    }

    private IEnumerator WaitForPlayerThenSchedule()
    {
        yield return new WaitUntil(() => FindFirstObjectByType<PlayerNetwork>() != null);
        player = FindFirstObjectByType<PlayerNetwork>().transform;

        Debug.Log("[MobSpawner] Player found, scheduling first spawn.");
        ScheduleNextSpawn();
    }

    private void ScheduleNextSpawn()
    {
        float delay = Random.Range(spawnIntervalMin, spawnIntervalMax);
        Invoke(nameof(SpawnMob), delay);
    }

    private void SpawnMob()
    {
        if (player == null || netSpawner == null || spawnableMobs.Count == 0)
            return;

        int selected = SelectRandomMobIndex();
        int prefabIndex = spawnableMobs[selected].prefabIndex;

        Vector2 dir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = player.position + (Vector3)(dir * 10f);
        spawnPos.z = 0f;

        netSpawner.RequestSpawnFly(spawnPos, prefabIndex);
        Debug.Log($"[MobSpawner] Spawned '{spawnableMobs[selected].name}' at {spawnPos}");

        ScheduleNextSpawn();
    }

    private int SelectRandomMobIndex()
    {
        float totalWeight = 0f;
        foreach (var mob in spawnableMobs) totalWeight += mob.weight;

        float r = Random.value * totalWeight;

        for (int i = 0; i < spawnableMobs.Count; i++)
        {
            if (r <= spawnableMobs[i].weight)
            {
                return i;
            }

            r -= spawnableMobs[i].weight;
        }
        return 0;
    }
}
