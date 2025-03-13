using System.Collections.Generic;
using UnityEngine;

public class MachinePlacementManager : MonoBehaviour
{
    [Header("Configuration des Machines")] public List<GameObject> machinePrefabs;

    public GameObject machineMenuUI;
    private GameObject selectedMachinePrefab;
    [SerializeField] private Transform player;

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

    public void SetPlayerTransform(Transform newPlayerTransform)
    {
        player = newPlayerTransform;
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
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mosPos);

        worldPos.z = -1;

        Instantiate(selectedMachinePrefab, worldPos, Quaternion.identity);

        Debug.Log("Machine placée à : " + worldPos);

        selectedMachinePrefab = null;
    }
}