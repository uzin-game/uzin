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
    public int flyPrefabIndex = 3;

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

    private void Start()
    {
    }

    private void OnServerStarted()
    {
        Debug.Log("[FlySpawner] Server started, beginning spawn.");
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitUntil(() =>
        {
            return FindFirstObjectByType<PlayerNetwork>() != null;
        });

        player = FindFirstObjectByType<PlayerNetwork>().transform;

        Debug.Log("[FlySpawner] Player found, starting continuous spawn.");

        while (true)
        {
            SpawnFly();
            yield return new WaitForSeconds(Random.Range(spawnIntervalMin, spawnIntervalMax));
        }
    }

    private void SpawnFly()
    {
        if (player == null || netSpawner == null)
        {
            return;
        }

        Vector2 dir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = player.position + (Vector3)(dir * 10f);

        netSpawner.RequestSpawnObject(spawnPos, flyPrefabIndex);

        Debug.Log("[FlySpawner] Fly spawned at " + Time.time);
    }
}
