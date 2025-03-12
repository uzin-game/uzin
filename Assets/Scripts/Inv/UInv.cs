using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Nécessaire pour TextMeshPro

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory; // Référence à l'inventaire
    public Transform itemPanel;  // Panel parent pour les slots d'items
    public GameObject itemUIPrefab; // Prefab pour l'UI d'un item

    private void Start()
    {
        if (inventory == null)
        {
            Debug.LogError("Inventory reference is missing in InventoryUI.");
            return;
        }

        // S'abonner aux changements de l'inventaire
        inventory.items.OnListChanged += UpdateUI;

        // Mise à jour initiale de l'UI
        UpdateUI(default);
    }

    // Mettre à jour l'UI lorsque l'inventaire change
    private void UpdateUI(NetworkListEvent<Itemss> changeEvent)
    {
        // Effacer l'UI actuelle
        foreach (Transform child in itemPanel)
        {
            Destroy(child.gameObject);
        }

        // Recréer l'UI pour chaque item
        foreach (var item in inventory.items)
        {
            GameObject itemUI = Instantiate(itemUIPrefab, itemPanel); // Créer un nouvel élément UI

            // Mettre à jour l'image
            Image itemImage = itemUI.GetComponent<Image>();
            if (itemImage != null)
            {
                itemImage.sprite = ItemDatabase.Instance.GetSprite(item.iconId);
            }
            else
            {
                Debug.LogWarning("L'élément UI ne contient pas d'Image.");
            }

            // Mettre à jour le texte (nom de l'item)
            TMP_Text itemText = itemUI.GetComponentInChildren<TMP_Text>();
            if (itemText != null)
            {
                itemText.text = item.itemName.ToString();
            }
            else
            {
                Debug.LogWarning("L'élément UI ne contient pas de TMP_Text.");
            }
        }
    }
}