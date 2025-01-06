using static oreMap;
using UnityEngine;

public class PlaceMachines : MonoBehaviour
{
    public GameObject Miner;
    public GameObject MainObject;
    public GameObject Map;

    public oreMap oreMapInstance;

    public Quaternion spawnRotation = Quaternion.identity;
    public Vector3 spawnPosition;
    public float yCoord;
    public float xCoord;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        oreMapInstance = new oreMap();
        yCoord = MainObject.transform.position.y;
        xCoord = MainObject.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        Renderer oreRenderer = Map.GetComponent<Renderer>();


        yCoord = MainObject.transform.position.y;
        xCoord = MainObject.transform.position.x;

        if (Input.GetButton("Fire1"))
        {
            if (oreMapInstance.oreLocations[(int)xCoord,(int)yCoord])
            {
                Vector3 spawnPosition = MainObject.transform.position;
                SpawnObject(spawnPosition);
            }
        }
        
    }

    public void SpawnObject(Vector3 spawnPosition)
    {
        if (Miner != null) // Check if a prefab is assigned
        {
            // Instantiate the object at the specified position and rotation
            Instantiate(Miner, spawnPosition, spawnRotation);
            Debug.Log("Object spawned at " + spawnPosition);
        }
        else
        {
            Debug.LogError("No object assigned to spawn!");
        }
    }
}
