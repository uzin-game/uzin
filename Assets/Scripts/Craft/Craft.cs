using System.Collections;
using System.Collections.Generic;
using QuestsScrpit;
using RedstoneinventeGameStudio;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Craft : NetworkBehaviour
{
    public GameObject Select;
    public InventoryUsing Inventory;
    public Image error;
    public CraftingRecipes re = null;
    public QuestManager questmanager;
    public CraftingRecipes Toldefer;
    public CraftingRecipes bouts;
    public void execute()
    {
        Debug.Log("Craft");
        StartCoroutine(OnPressed()); // ← CORRECT
    }

    public override void OnNetworkSpawn()
    {
        questmanager = FindFirstObjectByType<QuestManager>();
    }

    public IEnumerator OnPressed()
    {
        Debug.Log("Craft OnPressed");
        if (re == null)
        {
            Debug.Log("Craft OnPressed: re is null");
            error.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            error.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Craft OnPressed: re is not null");
            List<InventoryItemData> used = new List<InventoryItemData>();
            bool CanCraft = true;
            foreach (var item in re.Ressources)
            {
                if (Inventory.RemoveItem(item)) used.Add(item);
                else CanCraft = false;
            }
            if (CanCraft)
            {
                Debug.Log("Craft OnPressed: CanCraft is true");
                Inventory.AddItem(re.product.CreateCopyWithQuantity(re.amount));
                if (questmanager != null && questmanager.currentQuestIndex == 5 && re.product.itemName == Toldefer.product.itemName) 
                {
                    questmanager.Quests[5].Progress(1f);
                }
                if (questmanager != null && questmanager.currentQuestIndex == 6 && re.product.itemName == bouts.product.itemName)
                {
                    questmanager.Quests[6].Progress(1f);
                }
            }
            else
            {
                Debug.Log("Craft OnPressed: CanCraft is false");
                foreach (var item in used) Inventory.AddItem(item);
                error.gameObject.SetActive(true);
                yield return new WaitForSeconds(1f);
                error.gameObject.SetActive(false);
            }
        }
        
    }
}
