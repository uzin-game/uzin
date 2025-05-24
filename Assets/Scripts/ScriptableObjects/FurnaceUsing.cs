using RedstoneinventeGameStudio;
using UnityEngine;
using System.Collections;
using QuestsScrpit;

public class FurnaceUsing : MonoBehaviour
{
    public GameObject InputCard;
    public GameObject CoalCard;
    public GameObject OutputCard;

    public InventoryItemData CoalItem;
    public InventoryItemData IronOre;
    public InventoryItemData IronIngot;
    
    public NetworkSpawner networkSpawner;

    private bool burning = false;
    public QuestManager questManager;

    void Start()
    {
        networkSpawner = FindFirstObjectByType<NetworkSpawner>();
    }

    void Update()
    {
        Debug.Log(InputCard.GetComponent<CardManager>().itemData.itemName);
        Debug.Log(IronOre.itemName);
        bool notBurning = !burning;
        bool InputCardNull = InputCard.GetComponent<CardManager>().itemData != null;
        bool CoalCardNull = CoalCard.GetComponent<CardManager>().itemData != null;
        bool InputCardIronOre = InputCard.GetComponent<CardManager>().itemData.itemName == IronOre.itemName;
        bool CoalCardCoal = CoalCard.GetComponent<CardManager>().itemData.itemName == CoalItem.itemName;
        
        
        if (notBurning && InputCardNull && CoalCardNull && InputCardIronOre && CoalCardCoal)
        {
            Burn();
        }
    }

    void SpawnOutput(Vector3 position, int prefabindex)
    {
        Debug.Log("Spawning Output");
        networkSpawner.RequestSpawnOutput(position, prefabindex);
    }
    
    public bool ConveyorUsing(InventoryItemData inventoryItem)
    {
        if (InputCard.GetComponent<CardManager>().itemData == null)
        {
            InputCard.GetComponent<CardManager>().SetItem(inventoryItem);
            return true;
        }
        if (InputCard.GetComponent<CardManager>().itemData.itemName == inventoryItem.itemName)
        {
            InputCard.GetComponent<CardManager>().UnSetItem();
            InputCard.GetComponent<CardManager>()
                .SetItem(inventoryItem.CreateCopyWithQuantity(InputCard.GetComponent<CardManager>().itemData.itemNb +
                                                              inventoryItem.itemNb));
            return true;
        }
        return false;
    }

    public void Burn()
    {
        burning = true;
        StartCoroutine(BurnProcess());
    }

    private IEnumerator BurnProcess()
    {
        var input = InputCard.GetComponent<CardManager>();
        var coal = CoalCard.GetComponent<CardManager>();
        var output = OutputCard.GetComponent<CardManager>();

        float burnDuration = 10f; // Durée de vie du charbon
        float productionInterval = 2f;
        float elapsed = 0f;

        // Vérifie le charbon dispo
        if (coal.itemData == null || coal.itemData.itemNb <= 0)
        {
            burning = false;
            yield break;
        }

        // Consomme immédiatement 1 charbon
        int newCoalQty = coal.itemData.itemNb - 1;
        coal.UnSetItem();
        if (newCoalQty > 0) coal.SetItem(CoalItem.CreateCopyWithQuantity(newCoalQty));

        var player = GameObject.FindGameObjectWithTag("Player");
        questManager = player.GetComponent<QuestManager>();
        // Pendant que le charbon brûle (10s), produire toutes les 2s
        while (elapsed < burnDuration)
        {
            // Vérifie que du minerai est dispo
            if (input.itemData == null || input.itemData.itemNb <= 0)
            {
                burning = false;
                yield break;
            }

            // Consommer 1 minerai
            int newOreQty = input.itemData.itemNb - 1;
            input.UnSetItem();
            if (newOreQty > 0) input.SetItem(IronOre.CreateCopyWithQuantity(newOreQty));

            // Ajouter 1 lingot dans la sortie

            if (GetComponent<FurnaceInteraction>().OutputLeTruc)
            {
                int index = 0;
                if (input.itemData.itemName == IronOre.itemName) index = 1;
                SpawnOutput(GetComponent<FurnaceInteraction>().ItemOutpusPosition, index);
            }
            else
            {
                if (output.itemData == null)
                {
                    int index = 0;
                    if (input.itemData.itemName == IronOre.itemName) index = 1;
                    SpawnOutput(GetComponent<FurnaceInteraction>().ItemOutpusPosition, index);
                    output.SetItem(IronIngot.CreateCopyWithQuantity(1));
                    if (questManager.currentQuestIndex == 4 && input.itemData.itemName == IronOre.itemName)                                            
                    {
                        questManager.Quests[questManager.currentQuestIndex].Progress(1f);               
                    }
                }
                else
                {
                    int outQty = output.itemData.itemNb + 1;
                    output.UnSetItem();
                    output.SetItem(IronIngot.CreateCopyWithQuantity(outQty));
                    if (questManager.currentQuestIndex == 4 && input.itemData.itemName == IronOre.itemName)                                            
                    {
                        questManager.Quests[questManager.currentQuestIndex].Progress(1f);              
                    }
                }
            }
            // Attendre 2 secondes pour la prochaine production
            yield return new WaitForSeconds(productionInterval);
            elapsed += productionInterval;
        }

        // Fin du cycle de combustion du charbon
        burning = false;
    }
}
