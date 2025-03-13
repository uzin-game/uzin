using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MachineSelectionInput : MonoBehaviour
{
    public MachinePlacementManager placementManager;
    public TMP_InputField inputField;

    public void OnValidateButtonClicked()
    {
        int index;

        if (int.TryParse(inputField.text, out index))
        {
            placementManager.ToggleMenu();
            placementManager.SelectMachine(index);
            placementManager.PlaceMachine();
        }
        else
        {
            Debug.LogWarning("Veuillez entrer un nombre valide pour l'index de la machine.");
        }
    }
}