using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    private NetworkSpawner networkSpawner;

    private void Start()
    {
        networkSpawner = FindFirstObjectByType<NetworkSpawner>();
    }

    public void PlaceObject(Vector3 position, int prefabIndex)
    {
        if (networkSpawner != null)
        {
            networkSpawner.RequestSpawnObject(position, prefabIndex);
        }
        else
        {
            Debug.LogError("NetworkSpawner non trouvé !");
        }
    }
}