using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MachineSelectionDropdown : MonoBehaviour
{
    [Header("Références")] public MachinePlacementManager placementManager;
    [Header("Références")] public NetworkSpawner networkSpawner;

    public TMP_Dropdown dropdown;

    private void Start()
    {
        if (placementManager != null && dropdown != null)
        {
            List<string> options = new List<string>();

            foreach (GameObject machine in networkSpawner.machinePrefabs)
            {
                options.Add(machine.name);
            }

            dropdown.ClearOptions();
            dropdown.AddOptions(options);

            dropdown.value = 0;
        }
    }

    public void OnPlaceMachineButtonClicked()
    {
        if (placementManager != null)
        {
            int index = dropdown.value;
            placementManager.ToggleMenu();
            placementManager.SelectMachine(index);
            placementManager.PlaceMachine(index);
        }
    }

    public void OnCancelButtonClicked()
    {
        placementManager.CancelSelection();
    }
}