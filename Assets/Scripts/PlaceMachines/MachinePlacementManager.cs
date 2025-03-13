using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MachinePlacementManager : NetworkBehaviour
{
    [Header("Configuration des Machines")] public List<GameObject> machinePrefabs;

    public GameObject machineMenuUI;
    private GameObject selectedMachinePrefab;

    private Vector3 mosPos;

    public void SelectMachine(int machineIndex)
    {
        if (machineIndex >= 0 && machineIndex < machinePrefabs.Count)
        {
            selectedMachinePrefab = machinePrefabs[machineIndex];
            Debug.Log("Machine sélectionnée : " + selectedMachinePrefab.name);
        }
        else
        {
            Debug.LogWarning("Index de machine invalide");
        }
    }

    public void Start()
    {
        machineMenuUI.SetActive(false);
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

    public void PlaceMachine()
    {
        Vector3 worldPos = Camera.main!.ScreenToWorldPoint(mosPos);

        worldPos.z = -1;

        var objectplacer = FindObjectOfType<ObjectPlacer>();

        objectplacer.PlaceObject(worldPos);

        Debug.Log("Machine placée à : " + worldPos);

        selectedMachinePrefab = null;
    }
}