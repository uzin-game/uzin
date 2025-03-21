using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkSpawner : NetworkBehaviour
{
    [Header("Configuration des Machines")] public List<GameObject> machinePrefabs;

    
    [ServerRpc(RequireOwnership = false)]
    public void RequestSpawnObjectServerRpc(Vector3 position,  int prefabIndex, ServerRpcParams rpcParams = default)
    {
        if (!IsServer) return;
        
        GameObject obj = Instantiate(machinePrefabs[prefabIndex], position, Quaternion.identity);
        obj.GetComponent<NetworkObject>().Spawn(); // Synchronise avec les clients
    }

    public void RequestSpawnObject(Vector3 position,  int machinePrefabindex)
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
}