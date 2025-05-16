using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject Panel; // Référence au Panel de l'inventaire
    public GameObject background;
    public Animator inventoryAnimator; // Référence à l'Animator (optionnel)

    private bool isInventoryOpen = false; // État de l'inventaire (ouvert/fermé)

    private void Start()
    {
        // Désactiver l'inventaire au démarrage
        if (Panel != null)
        {
            Panel.SetActive(false);
            background.SetActive(false);
        }
    }

    private void Update()
    {
        // Ouvrir/fermer l'inventaire avec une touche (par exemple, "I")
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    // Méthode pour ouvrir/fermer l'inventaire
    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen; // Inverser l'état

        // Activer/désactiver le Panel ou jouer une animation
        if (inventoryAnimator)
        {
            inventoryAnimator.SetBool("IsOpen", isInventoryOpen);
        }
        else
        {
            Panel.SetActive(!Panel.activeSelf);
            background.SetActive(!background.activeSelf);
        }
    }
}