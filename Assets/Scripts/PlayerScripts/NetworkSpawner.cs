using System.Collections.Generic;
using MapScripts;
using QuestsScrpit;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NetworkSpawner : NetworkBehaviour
{
    [Header("Configuration des Machines")] public List<GameObject> machinePrefabs;
    [Header("Configuration des Ennemis")] public List<GameObject> enemyPrefabs;
    [Header("Configuration des outputs (four, mineur etc..)")] public List<GameObject> outputPrefabs;
    [SerializeField] private GameObject DrillPrefab;
    [SerializeField] private GameObject CrafterPrefab;
    [SerializeField] private GameObject TileMap;
    public QuestManager questManager;
    public Tilemap tilemap;
    public GameObject player;

    [ServerRpc(RequireOwnership = false)]
    public void RequestSpawnObjectServerRpc(Vector3 position, int prefabIndex, ServerRpcParams rpcParams = default)
    {
        if (!IsServer) return;

        if (machinePrefabs[prefabIndex] == null) Debug.Log("bah ntm fdp");
        else
        {
            GameObject obj = Instantiate(machinePrefabs[prefabIndex], position, Quaternion.identity);
            obj.GetComponent<NetworkObject>().Spawn(); // Synchronise avec les clients
            if (prefabIndex == 1)
            {
                Transform drillUsingTransform = obj.transform.Find("DrillUsing");
                GameObject DrillUsingGameObject = drillUsingTransform.gameObject;
                DrillUsingGameObject.GetComponent<DrillUsing>().Tile = TileMap.GetComponent<ChunkManager>().GetTileAtCell(tilemap.WorldToCell(position));
                player = GameObject.FindGameObjectWithTag("Player");
                questManager = tilemap.GetComponent<ChunkManager>().questManager;
                if (questManager.currentQuestIndex == 2)
                {
                    Debug.Log("progrès quequette");
                    questManager.Quests[questManager.currentQuestIndex].Progress(1f);
                }
            }

            if (prefabIndex == 6)
            {
                //player = GameObject.FindGameObjectWithTag("Player");
                //questManager = tilemap.GetComponent<ChunkManager>().questManager;
                if (questManager.currentQuestIndex == 6)
                {
                    Debug.Log("progrès quequette");
                    questManager.Quests[questManager.currentQuestIndex].Progress(1f);
                }
            }
        }
    }

    public void RequestSpawnObject(Vector3 position, int machinePrefabindex)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            RequestSpawnObjectServerRpc(position, machinePrefabindex);
        }
        else
        {
            RequestSpawnObjectServerRpc(position, machinePrefabindex);
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void RequestSpawnFlyServerRpc(Vector3 position, int enemyIndex, ServerRpcParams rpcParams = default)
    {
        if (!IsServer) return;

        if (enemyPrefabs[enemyIndex] == null) Debug.Log("bah ntm fdp");
        else
        {
            GameObject obj = Instantiate(enemyPrefabs[enemyIndex], position, Quaternion.identity);
            obj.GetComponent<NetworkObject>().Spawn(); // Synchronise avec les clients
        }
    }

    public void RequestSpawnFly(Vector3 position, int enemyIndex)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            RequestSpawnFlyServerRpc(position, enemyIndex);
        }
        else
        {
            RequestSpawnFlyServerRpc(position, enemyIndex);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnOutputServerRpc(Vector3 position, int prefabIndex, ServerRpcParams rpcParams = default)
    {
        if (!IsServer) return;

        if (outputPrefabs[prefabIndex] == null) Debug.Log("invalid prefab !");
        else
        {
            GameObject obj = Instantiate(outputPrefabs[prefabIndex], position, Quaternion.identity);

            // Désactiver la physique temporairement
            var rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null) rb.simulated = false;

            // Spawn réseau
            obj.GetComponent<NetworkObject>().Spawn();

            // Maintenant que l’objet est spawn et positionné, on active la physique
            if (rb != null) rb.simulated = true;

            Debug.Log("Spawned Item at :" + obj.transform.position);
        }
    }

    public void RequestSpawnOutput(Vector3 position, int prefabIndex)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            SpawnOutputServerRpc(position, prefabIndex);
        }
        else
        {
            SpawnOutputServerRpc(position, prefabIndex);
        }
    }
}