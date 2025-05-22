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
    [SerializeField] private GameObject DrillPrefab;
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
}