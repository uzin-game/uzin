using RedstoneinventeGameStudio;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using QuestsScrpit;

public class FurnaceUsing : MonoBehaviour
{
    public GameObject InputCard;
    public GameObject CoalCard;
    public GameObject OutputCard;

    public InventoryItemData CoalItem;
    public InventoryItemData IronOre;
    public InventoryItemData IronIngot;
    public InventoryItemData Gold;
    public InventoryItemData GoldIngot;
    public InventoryItemData Copper;
    public InventoryItemData CopperIngot;

    public NetworkSpawner networkSpawner;
    private bool burning = false;
    public QuestManager questManager;

    // Mapping minerai → lingot
    private Dictionary<string, InventoryItemData> oreToIngot;

    void Start()
    {
        networkSpawner = FindFirstObjectByType<NetworkSpawner>();

        oreToIngot = new Dictionary<string, InventoryItemData>
        {
            { IronOre.itemName, IronIngot },
            { Gold.itemName, GoldIngot },
            { Copper.itemName, CopperIngot }
        };
    }

    void Update()
    {
        var inputManager = InputCard.GetComponent<CardManager>();
        var coalManager = CoalCard.GetComponent<CardManager>();

        bool notBurning = !burning;
        bool inputNotNull = inputManager.itemData != null;
        bool coalNotNull = coalManager.itemData != null;

        if (!notBurning || !inputNotNull || !coalNotNull) return;

        var inputItem = inputManager.itemData;
        var coalItem = coalManager.itemData;

        bool isOre = oreToIngot.ContainsKey(inputItem.itemName);
        bool isCoal = coalItem.itemName == CoalItem.itemName;

        if (isOre && isCoal)
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
        var inputManager = InputCard.GetComponent<CardManager>();
        var existingItem = inputManager.itemData;

        if (existingItem == null)
        {
            inputManager.SetItem(inventoryItem);
            return true;
        }

        if (existingItem.itemName == inventoryItem.itemName)
        {
            inputManager.UnSetItem();
            inputManager.SetItem(inventoryItem.CreateCopyWithQuantity(existingItem.itemNb + inventoryItem.itemNb));
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

        float burnDuration = 10f;
        float productionInterval = 2f;
        float elapsed = 0f;

        if (coal.itemData == null || coal.itemData.itemNb <= 0)
        {
            burning = false;
            yield break;
        }

        // Consomme 1 charbon
        int newCoalQty = coal.itemData.itemNb - 1;
        coal.UnSetItem();
        if (newCoalQty > 0) coal.SetItem(CoalItem.CreateCopyWithQuantity(newCoalQty));

        var player = GameObject.FindGameObjectWithTag("Player");
        questManager = player.GetComponent<QuestManager>();

        while (elapsed < burnDuration)
        {
            if (input.itemData == null || input.itemData.itemNb <= 0)
            {
                burning = false;
                yield break;
            }

            var currentOre = input.itemData.itemName;
            if (!oreToIngot.ContainsKey(currentOre))
            {
                burning = false;
                Debug.LogWarning("Type de minerai non supporté !");
                yield break;
            }

            InventoryItemData correspondingIngot = oreToIngot[currentOre];

            // Consommer 1 minerai
            int newOreQty = input.itemData.itemNb - 1;
            input.UnSetItem();
            if (newOreQty > 0)
                input.SetItem(input.itemData.CreateCopyWithQuantity(newOreQty));

            // Ajouter 1 lingot dans la sortie
            if (output.itemData == null)
            {
                output.SetItem(correspondingIngot.CreateCopyWithQuantity(1));
            }
            else if (output.itemData.itemName == correspondingIngot.itemName)
            {
                int outQty = output.itemData.itemNb + 1;
                output.UnSetItem();
                output.SetItem(correspondingIngot.CreateCopyWithQuantity(outQty));
            }
            else
            {
                Debug.LogWarning("Impossible d'empiler : type de lingot différent.");
                burning = false;
                yield break;
            }

            // Quête
            if (questManager.currentQuestIndex == 4 && currentOre == IronOre.itemName)
            {
                questManager.Quests[questManager.currentQuestIndex].Progress(1f);
            }

            yield return new WaitForSeconds(productionInterval);
            elapsed += productionInterval;
        }

        burning = false;
    }
}
