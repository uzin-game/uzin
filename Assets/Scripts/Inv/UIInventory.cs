using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Nécessaire pour TextMeshPro

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory; // Référence à l'inventaire
    public Transform itemPanel;  // Panel parent pour les slots d'items
    public GameObject itemPrefab1;
    public GameObject itemPrefab2;
    public GameObject itemPrefab3;
    public GameObject itemPrefab4;
    public GameObject itemPrefab5;
    public GameObject itemPrefab6;
    public GameObject itemPrefab7;
    public GameObject itemPrefab8;
    public GameObject itemPrefab9;
    public GameObject itemPrefab10;
    public GameObject itemPrefab11;
    public GameObject itemPrefab12;
    public GameObject itemPrefab13;
    public GameObject itemPrefab14;
    public GameObject itemPrefab15;
    public GameObject itemPrefab16;
    public ItemDatabase itemDatabase;

    private void Start()
    {
        if (inventory == null)
        {
            Debug.LogError("Inventory reference is missing in InventoryUI.");
            return;
        }

        inventory.items.OnListChanged += _ => UpdateUI();
        inventory.quantity.OnListChanged += _ => UpdateUI();

        UpdateUI();
    }

    private void UpdateUI()
    {
        foreach (Transform child in itemPanel)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < inventory.items.Count; i++)
        {
            Item item = inventory.items[i];
            GameObject itemUI = Instantiate(GetUIPrefab(i), itemPanel);

            Image itemImage = itemUI.GetComponent<Image>();
            if (itemImage != null)
            {
                itemImage.sprite = itemDatabase.GetSprite(item.iconId);
            }

            TMP_Text itemNameText = itemUI.transform.Find("ItemName")?.GetComponent<TMP_Text>();
            if (itemNameText != null) itemNameText.text = item.itemName.ToString();

            TMP_Text itemNbText = itemUI.transform.Find("ItemNb")?.GetComponent<TMP_Text>();
            if (itemNbText != null) itemNbText.text = inventory.quantity[i].ToString();
        }
    }
    public GameObject GetUIPrefab(int i)
    {
        switch (i)
        {
            case 0: return itemPrefab1;
            case 1: return itemPrefab2;
            case 2: return itemPrefab3;
            case 3: return itemPrefab4;
            case 4: return itemPrefab5;
            case 5: return itemPrefab6;
            case 6: return itemPrefab7;
            case 7: return itemPrefab8;
            case 8: return itemPrefab9;
            case 9: return itemPrefab10;
            case 10: return itemPrefab11;
            case 11: return itemPrefab12;
            case 12: return itemPrefab13;
            case 13: return itemPrefab14;
            case 14: return itemPrefab15;
            case 15: return itemPrefab16;
            default:
            {
                Debug.LogError($"Unknown item type {i}");
                return itemPrefab1;
            }
        }
    }

    // Mettre à jour l'UI lorsque l'inventaire change
    private void UpdateUI(NetworkListEvent<Item> changeEvent)
    {
        // Effacer l'UI actuelle
        foreach (Transform child in itemPanel)
        {
            Destroy(child.gameObject);
        }

        // Recréer l'UI pour chaque item
        for (int i = 0; i < inventory.items.Count; i++)
        {
            Item item = inventory.items[i];
            GameObject itemUI = Instantiate(GetUIPrefab(i), itemPanel); 

            Image itemImage = itemUI.GetComponent<Image>();
            if (itemImage != null)
            {
                itemImage.sprite = itemDatabase.GetSprite(item.iconId);
            }
            else
            {
                Debug.LogWarning("L'élément UI ne contient pas d'Image.");
            }

            // Mettre à jour le texte (nom de l'item)
            TMP_Text itemNameText = itemUI.transform.Find("ItemName")?.GetComponent<TMP_Text>();
            if (itemNameText != null)
            {
                itemNameText.text = item.itemName.ToString();
            }
            else
            {
                Debug.LogWarning("L'élément UI ne contient pas de ItemNB");
            }
            
            TMP_Text itemNbText = itemUI.transform.Find("ItemNb")?.GetComponent<TMP_Text>();
            if (itemNbText != null)
            {
                itemNbText.text = inventory.quantity[i].ToString();
            }
            else
            {
                Debug.LogWarning("L'élément UI ne contient pas de ItemNB");
            }
        }
    }
}