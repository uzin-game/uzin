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
    private bool interfaceActive = true; // Nouveau flag pour savoir si l'interface est active
    private Coroutine burnCoroutine; // Référence à la coroutine pour pouvoir l'arrêter
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
        // Vérifier si l'interface est toujours active
        if (!interfaceActive) return;

        var inputManager = InputCard?.GetComponent<CardManager>();
        var coalManager = CoalCard?.GetComponent<CardManager>();

        // Vérifications de sécurité
        if (inputManager == null || coalManager == null) return;

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

    // Méthode à appeler quand l'interface se ferme
    public void OnInterfaceClose()
    {
        interfaceActive = false;
        StopBurning();
    }

    // Méthode à appeler quand l'interface s'ouvre
    public void OnInterfaceOpen()
    {
        interfaceActive = true;
    }

    // Méthode pour arrêter le processus de cuisson
    public void StopBurning()
    {
        if (burnCoroutine != null)
        {
            StopCoroutine(burnCoroutine);
            burnCoroutine = null;
        }
        burning = false;
    }

    void SpawnOutput(Vector3 position, int prefabindex)
    {
        Debug.Log("Spawning Output");
        networkSpawner.RequestSpawnOutput(position, prefabindex);
    }

    public bool ConveyorUsing(InventoryItemData inventoryItem)
    {
        var inputManager = InputCard?.GetComponent<CardManager>();
        if (inputManager == null) return false;

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
        if (burning) return; // Éviter de démarrer plusieurs fois
        
        burning = true;
        burnCoroutine = StartCoroutine(BurnProcess());
    }

    private IEnumerator BurnProcess()
    {
        float burnDuration = 10f;
        float productionInterval = 2f;
        float elapsed = 0f;

        var input = InputCard?.GetComponent<CardManager>();
        var coal = CoalCard?.GetComponent<CardManager>();
        var output = OutputCard?.GetComponent<CardManager>();

        // Vérifications de sécurité initiales
        if (input == null || coal == null || output == null)
        {
            burning = false;
            yield break;
        }

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
        questManager = FindFirstObjectByType<QuestManager>();

        while (elapsed < burnDuration && interfaceActive)
        {
            // Vérifications de sécurité à chaque itération
            if (input == null || coal == null || output == null)
            {
                burning = false;
                yield break;
            }

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
            Debug.Log(input.itemData.itemNb);
            int newOreQty = input.itemData.itemNb - 1;
            InventoryItemData c = input.itemData.CreateCopyWithQuantity(newOreQty);
            input.UnSetItem();
            if (newOreQty > 0)
                input.SetItem(c);

            // Ajouter 1 lingot dans la sortie
            if (output.itemData == null)
            {
                output.SetItem(correspondingIngot.CreateCopyWithQuantity(1));
                if (questManager != null && questManager.currentQuestIndex == 4 && currentOre == IronOre.itemName)
                {
                    questManager.Quests[questManager.currentQuestIndex].Progress(1f);
                }
            }
            else if (output.itemData.itemName == correspondingIngot.itemName)
            {
                int outQty = output.itemData.itemNb + 1;
                output.UnSetItem();
                output.SetItem(correspondingIngot.CreateCopyWithQuantity(outQty));
                if (questManager != null && questManager.currentQuestIndex == 4 && currentOre == IronOre.itemName)
                {
                    questManager.Quests[questManager.currentQuestIndex].Progress(1f);
                }
            }
            else
            {
                Debug.LogWarning("Impossible d'empiler : type de lingot différent.");
                burning = false;
                yield break;
            }

            yield return new WaitForSeconds(productionInterval);
            elapsed += productionInterval;
        }

        burning = false;
        burnCoroutine = null;
    }

    // Méthode appelée quand l'objet est détruit
    void OnDestroy()
    {
        StopBurning();
    }

    // Méthode appelée quand l'objet est désactivé
    void OnDisable()
    {
        StopBurning();
    }
}