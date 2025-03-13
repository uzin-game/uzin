using Unity.Netcode;
using UnityEngine;

public class NetworkSpawner : NetworkBehaviour
{
    public GameObject objectPrefab;

    [ServerRpc(RequireOwnership = false)]
    public void RequestSpawnObjectServerRpc(Vector3 position, ServerRpcParams rpcParams = default)
    {
        if (!IsServer) return;

        GameObject obj = Instantiate(objectPrefab, position, Quaternion.identity);
        obj.GetComponent<NetworkObject>().Spawn(); // Synchronise avec les clients
    }

    public void RequestSpawnObject(Vector3 position)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            RequestSpawnObjectServerRpc(position);
        }
        else
        {
            RequestSpawnObjectServerRpc(position);
        }
    }
}