using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    private NetworkSpawner networkSpawner;

    private void Start()
    {
        networkSpawner = FindObjectOfType<NetworkSpawner>();
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