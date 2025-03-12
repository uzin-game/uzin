using CameraScripts;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject mapPrefab;

    public int cameraCount = 0;

    private void Start()
    {
        Debug.Log($"PlayerSpawner started. IsServer: {IsServer}, IsClient: {IsClient}");
        Debug.Log($"Player {NetworkObjectId} spawned with scale: {transform.localScale}");

        if (IsServer)
        {
            //SpawnMap();
            SpawnPlayers();
        }
    }

    private void SpawnMap()
    {
        // Spawn the map and synchronize the seed
        GameObject map = Instantiate(mapPrefab);
        map.GetComponent<NetworkObject>().Spawn();
    }

    private void SpawnPlayers()
    {
        // Spawn players for all connected clients
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            GameObject player = Instantiate(playerPrefab);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

            /*
            // Assign a camera to the player (only for the local player)
            if (player.GetComponent<NetworkObject>().IsLocalPlayer)
            {
                AssignCameraToPlayer(player);
            }*/
        }
    }
    /*
    private void AssignCameraToPlayer(GameObject player)
    {
        
        // Create a camera for the player
        GameObject cameraObject = new GameObject("PlayerCamera");
        cameraObject.AddComponent<Camera>();
        //cameraObject.AddComponent<AudioListener>(); // Optional: Add audio listener

        // Add the PlayerCamera script to the camera
        PlayerCamera cameraController = cameraObject.AddComponent<PlayerCamera>();
        cameraController.playerTransform = player.transform; // Assign the player to follow
    }*/
}