using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkSpawner : NetworkBehaviour
{
    [Header("Configuration des Machines")] public List<GameObject> machinePrefabs;
    [Header("Configuration des Ennemis")] public List<GameObject> enemyPrefabs;



    [ServerRpc(RequireOwnership = false)]
    public void RequestSpawnObjectServerRpc(Vector3 position, int prefabIndex, ServerRpcParams rpcParams = default)
    {
        if (!IsServer) return;

        if (machinePrefabs[prefabIndex] == null) Debug.Log("bah ntm fdp");
        else
        {
            GameObject obj = Instantiate(machinePrefabs[prefabIndex], position, Quaternion.identity);
            obj.GetComponent<NetworkObject>().Spawn(); // Synchronise avec les clients
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