using System;
using RedstoneinventeGameStudio;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using QuestsScrpit;
using ScriptableObjects;
using Unity.Netcode;

[System.Serializable]
public class FurnaceSlot
{
    public string itemName;
    public int itemQuantity;
    public InventoryItemData itemReference;
    [SerializeField] public bool isDirty = false; // Flag pour indiquer si le slot a été modifié
    
    public FurnaceSlot()
    {
        itemName = "";
        itemQuantity = 0;
        itemReference = null;
        isDirty = false;
    }
    
    public FurnaceSlot(InventoryItemData item)
    {
        if (item != null)
        {
            itemName = item.itemName;
            itemQuantity = item.itemNb;
            itemReference = item;
        }
        else
        {
            itemName = "";
            itemQuantity = 0;
            itemReference = null;
        }
        isDirty = false;
    }
    
    public bool IsEmpty()
    {
        return string.IsNullOrEmpty(itemName) || itemQuantity <= 0;
    }
    
    public bool IsDirty()
    {
        return isDirty;
    }
    
    public void MarkClean()
    {
        isDirty = false;
    }
    
    public InventoryItemData ToInventoryItemData()
    {
        if (IsEmpty() || itemReference == null) return null;
        return itemReference.CreateCopyWithQuantity(itemQuantity);
    }
    
    public void Clear()
    {
        itemName = "";
        itemQuantity = 0;
        itemReference = null;
        isDirty = true;
    }
    
    public void SetItem(InventoryItemData item)
    {
        if (item != null)
        {
            itemName = item.itemName;
            itemQuantity = item.itemNb;
            itemReference = item;
        }
        else
        {
            Clear();
            return;
        }
        isDirty = true;
    }
    
    public bool AddQuantity(int amount)
    {
        if (IsEmpty()) return false;
        itemQuantity += amount;
        isDirty = true;
        return true;
    }
    
    public bool RemoveQuantity(int amount)
    {
        if (IsEmpty() || itemQuantity < amount) return false;
        itemQuantity -= amount;
        isDirty = true;
        if (itemQuantity <= 0)
        {
            Clear();
        }
        return true;
    }
}

public class FurnaceUsing : NetworkBehaviour
{
    [Header("UI References (peuvent être désactivées)")]
    public GameObject InputCard;
    public GameObject CoalCard;
    public GameObject OutputCard;

    [Header("Item Data")]
    public InventoryItemData CoalItem;
    public InventoryItemData IronOre;
    public InventoryItemData IronIngot;
    public InventoryItemData Gold;
    public InventoryItemData GoldIngot;
    public InventoryItemData Copper;
    public InventoryItemData CopperIngot;

    [Header("Network")]
    public NetworkSpawner networkSpawner;
    public List<GameObject> OutputPrefabs;
    
    [Header("Machine State")]
    public bool burning = false;
    private bool interfaceActive = false;
    private Coroutine burnCoroutine;
    public QuestManager questManager;
    
    // Stockage permanent des données du four
    [SerializeField] private FurnaceSlot furnaceInputSlot = new FurnaceSlot();
    [SerializeField] private FurnaceSlot furnaceCoalSlot = new FurnaceSlot();
    [SerializeField] private FurnaceSlot furnaceOutputSlot = new FurnaceSlot();
    
    // Flag pour éviter la boucle de synchronisation
    private bool isUpdatingUI = false;
    
    public NetworkVariable<MachineOutputMode> OutputMode = new NetworkVariable<MachineOutputMode>(
        MachineOutputMode.Inventory,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

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
        
        Debug.Log("Four initialisé");
        LogFurnaceState("Start");
    }

    void Update()
    {
        CheckAndStartBurning();
    }

    private void CheckAndStartBurning()
    {
        if (burning) return;
        if (furnaceInputSlot.IsEmpty()) return;
        if (furnaceCoalSlot.IsEmpty()) return;

        bool isOre = oreToIngot.ContainsKey(furnaceInputSlot.itemName);
        bool isCoal = furnaceCoalSlot.itemName == CoalItem.itemName;

        if (isOre && isCoal)
        {
            Debug.Log($"Conditions remplies pour démarrer - Ore: {furnaceInputSlot.itemName} x{furnaceInputSlot.itemQuantity}, Coal: {furnaceCoalSlot.itemQuantity}");
            Burn();
        }
    }

    // Synchronisation explicite depuis l'UI
    public void SyncFromUIExplicit()
    {
        try
        {
            var inputManager = InputCard?.GetComponent<CardManager>();
            var coalManager = CoalCard?.GetComponent<CardManager>();
            var outputManager = OutputCard?.GetComponent<CardManager>();

            if (inputManager != null)
            {
                furnaceInputSlot.SetItem(inputManager.itemData);
                Debug.Log($"Sync depuis UI - Input: {furnaceInputSlot.itemName} x{furnaceInputSlot.itemQuantity}");
            }
            if (coalManager != null)
            {
                furnaceCoalSlot.SetItem(coalManager.itemData);
                Debug.Log($"Sync depuis UI - Coal: {furnaceCoalSlot.itemName} x{furnaceCoalSlot.itemQuantity}");
            }
            if (outputManager != null)
            {
                furnaceOutputSlot.SetItem(outputManager.itemData);
                Debug.Log($"Sync depuis UI - Output: {furnaceOutputSlot.itemName} x{furnaceOutputSlot.itemQuantity}");
            }
            
            LogFurnaceState("Après sync depuis UI");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur lors de la synchronisation depuis l'UI: " + e.Message);
        }
    }

    // MODIFICATION PRINCIPALE : Synchronisation optimisée avec le système dirty
    private void SyncToUI()
    {
        if (isUpdatingUI) return;
        
        // Vérifier si au moins un slot a été modifié
        bool needsSync = furnaceInputSlot.IsDirty() || furnaceCoalSlot.IsDirty() || furnaceOutputSlot.IsDirty();
        if (!needsSync) return;
        
        // Vérifier si les composants UI existent
        if (InputCard == null || CoalCard == null || OutputCard == null) return;

        isUpdatingUI = true;
        
        try
        {
            var inputManager = InputCard?.GetComponent<CardManager>();
            var coalManager = CoalCard?.GetComponent<CardManager>();
            var outputManager = OutputCard?.GetComponent<CardManager>();

            // Synchroniser uniquement les slots modifiés
            if (inputManager != null && furnaceInputSlot.IsDirty())
            {
                inputManager.UnSetItem();
                var inputData = furnaceInputSlot.ToInventoryItemData();
                if (inputData != null)
                {
                    inputManager.SetItem(inputData);
                    Debug.Log($"Sync vers UI - Input: {inputData.itemName} x{inputData.itemNb}");
                }
                furnaceInputSlot.MarkClean();
            }
            
            if (coalManager != null && furnaceCoalSlot.IsDirty())
            {
                coalManager.UnSetItem();
                var coalData = furnaceCoalSlot.ToInventoryItemData();
                if (coalData != null)
                {
                    coalManager.SetItem(coalData);
                    Debug.Log($"Sync vers UI - Coal: {coalData.itemName} x{coalData.itemNb}");
                }
                furnaceCoalSlot.MarkClean();
            }
            
            if (outputManager != null && furnaceOutputSlot.IsDirty())
            {
                outputManager.UnSetItem();
                var outputData = furnaceOutputSlot.ToInventoryItemData();
                if (outputData != null)
                {
                    outputManager.SetItem(outputData);
                    Debug.Log($"Sync vers UI - Output: {outputData.itemName} x{outputData.itemNb}");
                }
                furnaceOutputSlot.MarkClean();
            }
            
            LogFurnaceState("Après sync vers UI optimisée");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur lors de la synchronisation vers l'UI: " + e.Message);
        }
        finally
        {
            isUpdatingUI = false;
        }
    }

    // Méthodes publiques pour l'interaction avec l'UI
    public void OnInterfaceClose()
    {
        Debug.Log("=== FERMETURE INTERFACE ===");
        LogFurnaceState("Avant fermeture");
        
        SyncFromUIExplicit();
        
        LogFurnaceState("Après fermeture et sauvegarde");
        Debug.Log("=== FIN FERMETURE ===");
    }

    public void OnInterfaceOpen()
    {
        Debug.Log("=== OUVERTURE INTERFACE ===");
        LogFurnaceState("Avant ouverture");
        
        // Forcer la synchronisation complète à l'ouverture
        furnaceInputSlot.isDirty = true;
        furnaceCoalSlot.isDirty = true;
        furnaceOutputSlot.isDirty = true;
        SyncToUI();
        
        LogFurnaceState("Après ouverture et restauration");
        Debug.Log("=== FIN OUVERTURE ===");
    }

    // Méthodes pour manipuler directement les données du four
    public void SetFurnaceInput(InventoryItemData item)
    {
        furnaceInputSlot.SetItem(item);
        Debug.Log($"SetFurnaceInput: {furnaceInputSlot.itemName} x{furnaceInputSlot.itemQuantity}");
        SyncToUI(); // Synchronisation automatique grâce au système dirty
    }

    public void SetFurnaceCoal(InventoryItemData item)
    {
        furnaceCoalSlot.SetItem(item);
        Debug.Log($"SetFurnaceCoal: {furnaceCoalSlot.itemName} x{furnaceCoalSlot.itemQuantity}");
        SyncToUI(); // Synchronisation automatique grâce au système dirty
    }

    public void SetFurnaceOutput(InventoryItemData item)
    {
        furnaceOutputSlot.SetItem(item);
        Debug.Log($"SetFurnaceOutput: {furnaceOutputSlot.itemName} x{furnaceOutputSlot.itemQuantity}");
        SyncToUI(); // Synchronisation automatique grâce au système dirty
    }

    public InventoryItemData GetFurnaceInput() => furnaceInputSlot.ToInventoryItemData();
    public InventoryItemData GetFurnaceCoal() => furnaceCoalSlot.ToInventoryItemData();
    public InventoryItemData GetFurnaceOutput() => furnaceOutputSlot.ToInventoryItemData();

    public void StopBurning()
    {
        if (burnCoroutine != null)
        {
            StopCoroutine(burnCoroutine);
            burnCoroutine = null;
        }
        burning = false;
        Debug.Log("Four arrêté");
    }

    public bool ConveyorUsing(InventoryItemData inventoryItem)
    {
        Debug.Log($"ConveyorUsing appelé avec: {inventoryItem.itemName} x{inventoryItem.itemNb}");
        
        if (furnaceInputSlot.IsEmpty())
        {
            furnaceInputSlot.SetItem(inventoryItem);
            Debug.Log($"ConveyorUsing - Nouvel item ajouté: {furnaceInputSlot.itemName} x{furnaceInputSlot.itemQuantity}");
            SyncToUI(); // Synchronisation automatique grâce au système dirty
            return true;
        }

        if (furnaceInputSlot.itemName == inventoryItem.itemName)
        {
            furnaceInputSlot.AddQuantity(inventoryItem.itemNb);
            Debug.Log($"ConveyorUsing - Quantité ajoutée: {furnaceInputSlot.itemName} x{furnaceInputSlot.itemQuantity}");
            SyncToUI(); // Synchronisation automatique grâce au système dirty
            return true;
        }

        Debug.Log("ConveyorUsing - Rejeté (slot occupé par un autre item)");
        return false;
    }
    
    public void SetOutputMode(MachineOutputMode newMode)
    {
        OutputMode.Value = newMode;
    }

    public void Burn()
    {
        if (burning) return;
        
        Debug.Log("=== DÉBUT PROCESSUS DE CUISSON ===");
        LogFurnaceState("Début burn");
        
        burning = true;
        burnCoroutine = StartCoroutine(BurnProcess());
    }

    private IEnumerator BurnProcess()
    {
        float burnDuration = 10f;
        float productionInterval = 2f;
        float elapsed = 0f;

        if (furnaceCoalSlot.IsEmpty())
        {
            Debug.LogWarning("Pas de charbon - Arrêt");
            burning = false;
            yield break;
        }

        if (!furnaceCoalSlot.RemoveQuantity(1))
        {
            Debug.LogError("Impossible de consommer le charbon");
            burning = false;
            yield break;
        }
        
        Debug.Log($"Burn - Charbon consommé, reste: {furnaceCoalSlot.itemQuantity}");
        
        // Synchronisation automatique grâce au système dirty
        SyncToUI();

        questManager = FindFirstObjectByType<QuestManager>();

        while (elapsed < burnDuration)
        {
            if (furnaceInputSlot.IsEmpty())
            {
                Debug.Log("Plus de minerai - Arrêt du four");
                burning = false;
                yield break;
            }

            var currentOre = furnaceInputSlot.itemName;
            if (!oreToIngot.ContainsKey(currentOre))
            {
                Debug.LogWarning($"Type de minerai non supporté: {currentOre}");
                burning = false;
                yield break;
            }

            InventoryItemData correspondingIngot = oreToIngot[currentOre];

            Debug.Log($"Four: Traitement de {currentOre}, quantité avant: {furnaceInputSlot.itemQuantity}");
            
            if (!furnaceInputSlot.RemoveQuantity(1))
            {
                Debug.LogError("Impossible de consommer le minerai");
                burning = false;
                yield break;
            }
            
            Debug.Log($"Four: Minerai consommé, reste: {furnaceInputSlot.itemQuantity}");

            if (OutputMode.Value == MachineOutputMode.Inventory)
            {
                if (furnaceOutputSlot.IsEmpty())
                {
                    furnaceOutputSlot.SetItem(correspondingIngot.CreateCopyWithQuantity(1));
                    Debug.Log($"Four: Premier lingot produit: {furnaceOutputSlot.itemName} x{furnaceOutputSlot.itemQuantity}");
                }
                else if (furnaceOutputSlot.itemName == correspondingIngot.itemName)
                {
                    furnaceOutputSlot.AddQuantity(1);
                    Debug.Log($"Four: Lingot ajouté: {furnaceOutputSlot.itemName} x{furnaceOutputSlot.itemQuantity}");
                }
                else
                {
                    Debug.LogWarning("Impossible d'empiler : type de lingot différent dans la sortie.");
                    burning = false;
                    yield break;
                }

                if (questManager != null && questManager.currentQuestIndex == 4 && currentOre == IronOre.itemName)
                {
                    questManager.Quests[questManager.currentQuestIndex].Progress(1f);
                }
            }
            else
            {
                DropItemNearMachine(correspondingIngot);
            }
            
            // Synchronisation automatique grâce au système dirty
            SyncToUI();
            
            LogFurnaceState($"Après production (elapsed: {elapsed}s)");
            
            yield return new WaitForSeconds(productionInterval);
            elapsed += productionInterval;
        }

        burning = false;
        burnCoroutine = null;
        Debug.Log("=== FIN PROCESSUS DE CUISSON ===");
        LogFurnaceState("Fin burn");
    }
    
    void DropItemNearMachine(InventoryItemData product)
    {
        Vector3 dropPosition = new Vector3();
        if (OutputMode.Value == MachineOutputMode.DropBelow) dropPosition = GetComponentInParent<NetworkObject>().transform.position + Vector3.down;
        else if (OutputMode.Value == MachineOutputMode.DropAbove) dropPosition = GetComponentInParent<NetworkObject>().transform.position + Vector3.up;
        else if (OutputMode.Value == MachineOutputMode.DropLeft) dropPosition = GetComponentInParent<NetworkObject>().transform.position + Vector3.left;
        else if (OutputMode.Value == MachineOutputMode.DropRight) dropPosition = GetComponentInParent<NetworkObject>().transform.position + Vector3.right;
        
        int prefabIndex = 0;
        if (product.itemName == oreToIngot[Gold.itemName].itemName)
        {
            prefabIndex = 2;
        }
        if (product.itemName == oreToIngot[Copper.itemName].itemName)
        {
            prefabIndex = 1;
        }
        
        GameObject droppedItem = Instantiate(OutputPrefabs[prefabIndex], dropPosition, Quaternion.identity);
        droppedItem.GetComponent<NetworkObject>().Spawn();
    }

    private void LogFurnaceState(string context)
    {
        Debug.Log($"[{context}] État du four:");
        Debug.Log($"  Input: {(furnaceInputSlot.IsEmpty() ? "vide" : $"{furnaceInputSlot.itemName} x{furnaceInputSlot.itemQuantity}")}");
        Debug.Log($"  Coal: {(furnaceCoalSlot.IsEmpty() ? "vide" : $"{furnaceCoalSlot.itemName} x{furnaceCoalSlot.itemQuantity}")}");
        Debug.Log($"  Output: {(furnaceOutputSlot.IsEmpty() ? "vide" : $"{furnaceOutputSlot.itemName} x{furnaceOutputSlot.itemQuantity}")}");
        Debug.Log($"  Burning: {burning}");
        Debug.Log($"  Interface Active: {interfaceActive}");
    }

    void OnDestroy()
    {
        StopBurning();
    }

    void OnDisable()
    {
        StopBurning();
    }
}