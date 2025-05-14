using System.Collections.Generic;
using PlayerScripts;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MachinePlacementManager : NetworkBehaviour
{
    [Header("Configuration des Machines")] public List<GameObject> machinePrefabs;

    public GameObject machineMenuUI;
    private GameObject selectedMachinePrefab;
    private NetworkSpawner networkSpawner;

    private Vector3 mosPos;

    public void SelectMachine(int machineIndex)
    {
        if (machineIndex >= 0 && machineIndex < machinePrefabs.Count)
        {
            selectedMachinePrefab = machinePrefabs[machineIndex];
            Debug.Log("Machine sélectionnée : " + selectedMachinePrefab.name + " a l'indice : " + machineIndex );
        }
        else
        {
            Debug.LogWarning("Index de machine invalide");
        }
    }

    public void Start()
    {
        machineMenuUI.SetActive(false);
        networkSpawner = FindObjectOfType<NetworkSpawner>();

    }

    public void CancelSelection()
    {
        selectedMachinePrefab = null;
    }

    public void ToggleMenu()
    {
        machineMenuUI.SetActive(!machineMenuUI.activeSelf);
    }

    private void Update()
    {
        if (!machineMenuUI.activeSelf && Input.GetKeyDown(KeyCode.P))
        {
            mosPos = Input.mousePosition;
            ToggleMenu();
        }
    }

    public void PlaceMachine(int index)
    {
        Vector3 worldPos = Camera.main!.ScreenToWorldPoint(mosPos);

        worldPos.z = -1;
        
        Vector3 ChunkPos = new Vector3(Mathf.Floor(worldPos.x) + 0.5f, Mathf.Floor(worldPos.y) + 0.5f, worldPos.z);

        //var objectplacer = FindObjectOfType<ObjectPlacer>();

        Debug.Log("placing " + selectedMachinePrefab.name + " with index : " + index);
        
        PlaceTheObject(ChunkPos, index);

        Debug.Log("Machine placée à : " + ChunkPos);

        selectedMachinePrefab = null;
    }
    
    public void PlaceTheObject(Vector3 position, int prefabIndex)
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