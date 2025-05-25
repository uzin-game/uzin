using System.Collections;
using QuestsScrpit;
using RedstoneinventeGameStudio;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DrillUsing : MonoBehaviour
{
    public GameObject CoalCard;
    public GameObject OutputCard;
    public TileBase Tile;
    public TileBase IronTile;
    public TileBase CoalTile;
    public TileBase CopperTile;
    public TileBase OrTile;

    public InventoryItemData CoalItem;
    public InventoryItemData IronOre;
    public InventoryItemData CopperOre;
    public InventoryItemData OrOre;
    
    public FurnaceInteraction furnaceInteraction;

    private InventoryItemData product;
    private bool CanMine = false;
    private bool burning = false;
    private bool advanceQuest;
    
    public QuestManager questManager;
    
    public NetworkSpawner spawner;
    void Start()
    {
        
        spawner = FindFirstObjectByType<NetworkSpawner>();
        // Debug pour vérifier les tiles
        Debug.Log("Current Tile: " + (Tile != null ? Tile.name : "null"));
        Debug.Log("CoalTile: " + (CoalTile != null ? CoalTile.name : "null"));
        Debug.Log("IronTile: " + (IronTile != null ? IronTile.name : "null"));
        Debug.Log("CopperTile: " + (CopperTile != null ? CopperTile.name : "null"));
        Debug.Log("OrTile: " + (OrTile != null ? OrTile.name : "null"));
        
        // Définir le produit et les paramètres une seule fois au début
        if (Tile != null && CoalTile != null && Tile == CoalTile)
        {
            product = CoalItem;
            CanMine = true;
            advanceQuest = false;
            Debug.Log("Configured for Coal mining");
        }
        else if (Tile != null && IronTile != null && Tile == IronTile)
        {
            product = IronOre;
            CanMine = true;
            advanceQuest = true;
            Debug.Log("Configured for Iron mining");
        }
        else if (Tile != null && CopperTile != null && Tile == CopperTile)
        {
            product = CopperOre;
            CanMine = true;
            advanceQuest = false;
            Debug.Log("Configured for Copper mining");
        }
        else if (Tile != null && OrTile != null && Tile == OrTile)
        {
            product = OrOre;
            CanMine = true;
            advanceQuest = false;
            Debug.Log("Configured for Or mining");
        }
        else
        {
            CanMine = false;
            advanceQuest = false;
            Debug.Log("No valid tile configuration found - mining disabled");
        }
        
        Debug.Log("Drill initialized - Product: " + (product != null ? product.itemName : "null") + ", CanMine: " + CanMine);
    }

    void Update()
    {
        bool notBurning = !burning;
        var coalCardManager = CoalCard.GetComponent<CardManager>();
        bool hasCoal = coalCardManager.itemData != null && 
                       coalCardManager.itemData.itemName == CoalItem.itemName && 
                       coalCardManager.itemData.itemNb > 0;
        
        if (notBurning && hasCoal && CanMine)
        {
            Debug.Log("Drill - Mining: " + (product != null ? product.itemName : "null"));
            Debug.Log("Coal available: " + coalCardManager.itemData.itemNb);
            Burn();
        }
    }
    
    void SpawnOutput(Vector3 position, int prefabindex)
    {
        Debug.Log("Spawning Output");
        spawner.RequestSpawnOutput(position, prefabindex);
    }

    public void Burn()
    {
        burning = true;
        StartCoroutine(BurnProcess());
    }

    private IEnumerator BurnProcess()
    {
        var coal = CoalCard.GetComponent<CardManager>();
        var output = OutputCard.GetComponent<CardManager>();

        float burnDuration = 10f;
        float productionInterval = 2f;
        float elapsed = 0f;

        // Vérifie que le charbon est disponible avant de commencer
        if (coal.itemData == null || coal.itemData.itemNb <= 0)
        {
            Debug.Log("Pas assez de charbon pour démarrer");
            burning = false;
            yield break;
        }

        // Consommer 1 charbon immédiatement
        int currentCoalQty = coal.itemData.itemNb;
        int newCoalQty = currentCoalQty - 1;
        
        Debug.Log("Consuming coal: " + currentCoalQty + " -> " + newCoalQty);
        
        coal.UnSetItem();
        if (newCoalQty > 0) 
        {
            coal.SetItem(CoalItem.CreateCopyWithQuantity(newCoalQty));
        }

        var player = GameObject.FindGameObjectWithTag("Player");
        questManager = FindFirstObjectByType<QuestManager>();
        
        // Pendant 10 secondes, produire toutes les 2 secondes
        while (elapsed < burnDuration)
        {
            Debug.Log("Production cycle - elapsed: " + elapsed + "s");
            
            // Ajout du produit dans la carte de sortie
            
            /*
            if (furnaceInteraction.OutputLeTruc)
            {
                int index = 0;
                if (output.itemData != null && output.itemData.itemName == IronOre.itemName) index = 1;
                SpawnOutput(furnaceInteraction.ItemOutpusPosition, index);
            }
            else
            {
                if (output.itemData == null)
                {
                    output.SetItem(product.CreateCopyWithQuantity(1));                                  //TODO
                    if (questManager.currentQuestIndex == 3 && advanceQuest)                                            //TODO
                    {
                        questManager.Quests[questManager.currentQuestIndex].Progress(1f);               //TODO
                    }
                }
                else
                {
                    int outQty = output.itemData.itemNb + 1;                                            //TODO
                    output.UnSetItem();
                    output.SetItem(product.CreateCopyWithQuantity(outQty));                             //TODO
                    if (questManager.currentQuestIndex == 3 && advanceQuest)
                    {
                        questManager.Quests[questManager.currentQuestIndex].Progress(1f);               //TODO
                    }
                }
            }*/
            
            if (output.itemData == null)
            {
                output.SetItem(product.CreateCopyWithQuantity(1));                                  //TODO
                Debug.Log("Created new output: " + product.itemName + " x1");
                if (questManager != null && questManager.currentQuestIndex == 3 && advanceQuest)                                            //TODO
                {
                    questManager.Quests[questManager.currentQuestIndex].Progress(1f);               //TODO
                }
            }
            else
            {
                int outQty = output.itemData.itemNb + 1;                                            //TODO
                output.UnSetItem();
                output.SetItem(product.CreateCopyWithQuantity(outQty));                             //TODO
                Debug.Log("Added to existing output: " + product.itemName + " x" + outQty);
                if (questManager != null && questManager.currentQuestIndex == 3 && advanceQuest)
                {
                    questManager.Quests[questManager.currentQuestIndex].Progress(1f);               //TODO
                }
            }

            yield return new WaitForSeconds(productionInterval);
            elapsed += productionInterval;
        }

        // Terminé après 10 secondes de production
        Debug.Log("Burn process completed");
        burning = false;
    }
}