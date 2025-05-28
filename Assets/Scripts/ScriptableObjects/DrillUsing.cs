using System.Collections;
using System.Collections.Generic;
using QuestsScrpit;
using RedstoneinventeGameStudio;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DrillUsing : NetworkBehaviour
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

    public List<GameObject> OutputPrefabs;
    
    public NetworkVariable<MachineOutputMode> OutputMode = new NetworkVariable<MachineOutputMode>(
        MachineOutputMode.Inventory,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    void Start()
    {
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

    public void SetOutputMode(MachineOutputMode newMode)
    {
        OutputMode.Value = newMode;
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
            if (OutputMode.Value == MachineOutputMode.Inventory)
            {
                if (output.itemData == null)
                {
                    output.SetItem(product.CreateCopyWithQuantity(1));                                 
                    Debug.Log("Created new output: " + product.itemName + " x1");
                    if (questManager != null && questManager.currentQuestIndex == 3 && advanceQuest)   
                    {
                        questManager.Quests[questManager.currentQuestIndex].Progress(1f);              
                    }
                }
                else
                {
                    int outQty = output.itemData.itemNb + 1;                                           
                    output.UnSetItem();
                    output.SetItem(product.CreateCopyWithQuantity(outQty));                            
                    Debug.Log("Added to existing output: " + product.itemName + " x" + outQty);
                    if (questManager != null && questManager.currentQuestIndex == 3 && advanceQuest)
                    {
                        questManager.Quests[questManager.currentQuestIndex].Progress(1f);              
                    }
                }
            }
            else
            {
                DropItemNearMachine(product);
            }
            yield return new WaitForSeconds(productionInterval);
            elapsed += productionInterval;
        }

        // Terminé après 10 secondes de production
        Debug.Log("Burn process completed");
        burning = false;
    }
    
    void DropItemNearMachine(InventoryItemData product)
    {
        Vector3 dropPosition = new Vector3();
        if (OutputMode.Value == MachineOutputMode.DropBelow) dropPosition = GetComponentInParent<NetworkObject>().transform.position + Vector3.down;
        else if (OutputMode.Value == MachineOutputMode.DropAbove) dropPosition = GetComponentInParent<NetworkObject>().transform.position + Vector3.up;
        else if (OutputMode.Value == MachineOutputMode.DropLeft) dropPosition = GetComponentInParent<NetworkObject>().transform.position + Vector3.left;
        else if (OutputMode.Value == MachineOutputMode.DropRight) dropPosition = GetComponentInParent<NetworkObject>().transform.position + Vector3.right;
        int prefabIndex = 0;
        if (product.itemName == OrOre.itemName)
        {
            prefabIndex = 2;
        }
        if (product.itemName == CopperOre.itemName)
        {
            prefabIndex = 1;
        }

        if (product.itemName == CoalItem.itemName)
        {
            prefabIndex = 3;
        }
        GameObject droppedItem = Instantiate(OutputPrefabs[prefabIndex], dropPosition, Quaternion.identity);
        droppedItem.GetComponent<NetworkObject>().Spawn(); // Pour le multijoueur
    }
}