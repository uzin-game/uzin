using RedstoneinventeGameStudio;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using QuestsScrpit;
using TMPro;
using Unity.Netcode;

public class ConstructeurUsing : NetworkBehaviour
{
    [Header("Craft System")]
    public CraftingRecipes Recipe;
    public GameObject CardIn1;
    public GameObject CardIn2;
    public GameObject CardOut;
    public TMP_Dropdown dropdown;
    public CraftingRecipes[] craftingRecipes;
    public GameObject boutDeChassis;
    public GameObject boutsDeMoteur;
    public GameObject systemeNav;
    
    [Header("UI Elements")]
    public GameObject craftUIPanel; // Panel contenant l'UI du craft
    public Transform requiredItem1Container; // Container pour le premier élément requis
    public Transform requiredItem2Container; // Container pour le deuxième élément requis
    public TextMeshProUGUI recipeNameText; // Nom de la recette
    public TextMeshProUGUI craftStatusText; // Statut du craft (peut crafter ou non)
    private QuestManager questManager;
    private float lastCraftTime = 0f;
    private float craftCooldown = 3f;
    private bool isCrafting = false;
    [Header("UI Elements")]
    public Image craftProgressBar; // Remplacer par Image au lieu de ProgressBar
// OU
    public ProgressBarController progressBarController; // Si vous utilisez le script ci-dessus

    
    void Start()
    {
        // S'assurer que l'UI est initialisée
        if (dropdown != null)
        {
            dropdown.onValueChanged.AddListener(OnRecipeChanged);
            UpdateCraftUI();
        }
    }
    private void UpdateProgressBar()
    {
        if (craftProgressBar == null) return;

        float progress = 0f;
        Color barColor = Color.red;

        if (isCrafting)
        {
            // Pendant le crafting, barre pleine en jaune
            progress = 1f;
            barColor = Color.yellow;
        }
        else
        {
            float timeRemaining = craftCooldown - (Time.time - lastCraftTime);
            
            if (timeRemaining > 0)
            {
                // En cooldown - progression de 0 à 1 pendant le cooldown
                progress = 1f - (timeRemaining / craftCooldown);
                barColor = new Color(1f, 0.5f, 0f); // Orange
            }
            else if (CanCraft())
            {
                // Prêt à crafter - barre pleine en vert
                progress = 1f;
                barColor = Color.green;
            }
            else
            {
                // Pas assez de ressources - barre vide en rouge
                progress = 0f;
                barColor = Color.red;
            }
        }
    
        // Appliquer la progression et la couleur
        craftProgressBar.fillAmount = progress;
        craftProgressBar.color = barColor;
    }

    // Craft automatique sans bouton
    void Update()
    {
        int index = dropdown.value;
        Recipe = craftingRecipes[index];
        if (Recipe == null) return;
        
        // Mettre à jour l'UI
        UpdateCraftUI();
        UpdateProgressBar();
        
        // Craft automatique avec vérifications complètes
        float timeRemaining = craftCooldown - (Time.time - lastCraftTime);
        
        if (!isCrafting && timeRemaining <= 0 && CanCraft())
        {
            Debug.Log("=== CRAFT AUTOMATIQUE DÉCLENCHÉ ===");
            Debug.Log($"Recipe: {Recipe.product?.itemName}, Amount: {Recipe.amount}");
            DoCraft();
            lastCraftTime = Time.time;
        }
    }

    // Version améliorée de TryCraft avec plus de debug
    public void TryCraft()
    {
        Debug.Log("=== TryCraft appelé ===");
        
        float timeRemaining = craftCooldown - (Time.time - lastCraftTime);
        Debug.Log($"Time remaining: {timeRemaining}, isCrafting: {isCrafting}");
        
        if (isCrafting)
        {
            Debug.Log("Craft déjà en cours - ANNULÉ");
            return;
        }
        
        if (timeRemaining > 0)
        {
            Debug.Log($"Cooldown en cours: {timeRemaining:F1}s restantes - ANNULÉ");
            return;
        }
        
        bool canCraft = CanCraft();
        Debug.Log($"CanCraft result: {canCraft}");
        
        if (!canCraft)
        {
            Debug.Log("Pas assez de ressources pour crafter - ANNULÉ");
            return;
        }
        
        Debug.Log("=== DÉBUT DU CRAFT ===");
        DoCraft();
        lastCraftTime = Time.time;
        Debug.Log("=== FIN DU CRAFT ===");
    }
    
    // Version alternative avec craft asynchrone (optionnel)
    public void StartAsyncCraft()
    {
        if (isCrafting) return;
        
        float timeRemaining = craftCooldown - (Time.time - lastCraftTime);
        if (timeRemaining > 0) return;
        
        if (!CanCraft()) return;
        
        StartCoroutine(CraftCoroutine());
    }
    
    private System.Collections.IEnumerator CraftCoroutine()
    {
        isCrafting = true;
        
        // Simuler un temps de craft (optionnel)
        float craftDuration = 2f; // 2 secondes pour crafter
        float startTime = Time.time;
        
        while (Time.time - startTime < craftDuration)
        {
            // Pendant le craft, on peut montrer une progression
            float craftProgress = (Time.time - startTime) / craftDuration;
            if (craftProgressBar != null)
            {
                craftProgressBar.fillAmount = craftProgress;
                craftProgressBar.color = Color.yellow;
            }
            yield return null;
        }
        
        // Faire le craft réel
        DoCraftImmediate(); // Version sans isCrafting = true au début
        lastCraftTime = Time.time;
        isCrafting = false;
    }
    
    private void DoCraftImmediate()
    {
        // Même logique que DoCraft() mais sans modifier isCrafting
        // (car c'est géré par la coroutine)
        
        // Créer un dictionnaire des ressources à retirer
        Dictionary<string, int> toRemove = GetRequiredResources();
    
        // Sauvegarder les données avant modification
        InventoryItemData card1Item = null;
        InventoryItemData card2Item = null;
        
        if (CardIn1 != null && CardIn1.GetComponent<CardManager>().itemData != null)
        {
            card1Item = CardIn1.GetComponent<CardManager>().itemData;
        }
        
        if (CardIn2 != null && CardIn2.GetComponent<CardManager>().itemData != null)
        {
            card2Item = CardIn2.GetComponent<CardManager>().itemData;
        }
    
        // Retirer les ressources de CardIn1
        if (card1Item != null && toRemove.ContainsKey(card1Item.itemName))
        {
            int amountToRemove = Mathf.Min(toRemove[card1Item.itemName], card1Item.itemNb);
            int remainingQuantity = card1Item.itemNb - amountToRemove;
            
            toRemove[card1Item.itemName] -= amountToRemove;
            if (toRemove[card1Item.itemName] <= 0)
            {
                toRemove.Remove(card1Item.itemName);
            }
            
            CardIn1.GetComponent<CardManager>().UnSetItem();
            if (remainingQuantity > 0)
            {
                CardIn1.GetComponent<CardManager>().SetItem(card1Item.CreateCopyWithQuantity(remainingQuantity));
            }
        }
    
        // Retirer les ressources de CardIn2
        if (card2Item != null && toRemove.ContainsKey(card2Item.itemName))
        {
            int amountToRemove = Mathf.Min(toRemove[card2Item.itemName], card2Item.itemNb);
            int remainingQuantity = card2Item.itemNb - amountToRemove;
            
            toRemove[card2Item.itemName] -= amountToRemove;
            if (toRemove[card2Item.itemName] <= 0)
            {
                toRemove.Remove(card2Item.itemName);
            }
            
            CardIn2.GetComponent<CardManager>().UnSetItem();
            if (remainingQuantity > 0)
            {
                CardIn2.GetComponent<CardManager>().SetItem(card2Item.CreateCopyWithQuantity(remainingQuantity));
            }
        }
    
        // Mettre le résultat dans CardOut
        if (CardOut != null)
        {
            if (CardOut.GetComponent<CardManager>().itemData == null)
            {
                CardOut.GetComponent<CardManager>().SetItem(Recipe.product.CreateCopyWithQuantity(Recipe.amount));
            }
            else if (CardOut.GetComponent<CardManager>().itemData.itemName == Recipe.product.itemName)
            {
                int total = CardOut.GetComponent<CardManager>().itemData.itemNb + Recipe.amount;
                CardOut.GetComponent<CardManager>().UnSetItem();
                CardOut.GetComponent<CardManager>().SetItem(Recipe.product.CreateCopyWithQuantity(total));
            }
        }

        if (Recipe.product.itemName == boutDeChassis.GetComponent<CardManager>().itemData.itemName)
        {
            questManager = FindFirstObjectByType<QuestManager>();
            if (questManager.currentQuestIndex == 8)
            {
                questManager.Quests[8].Progress(1f);
            }
        }
        if (Recipe.product.itemName == boutsDeMoteur.GetComponent<CardManager>().itemData.itemName)
        {
            questManager = FindFirstObjectByType<QuestManager>();
            if (questManager.currentQuestIndex == 9)
            {
                questManager.Quests[9].Progress(1f);
            }
        }
    
        Debug.Log("Craft terminé avec succès");
    }
    
    public override void OnNetworkSpawn()
    {
        questManager = FindFirstObjectByType<QuestManager>();
    }
    
    private void OnRecipeChanged(int newIndex)
    {
        UpdateCraftUI();
    }
    
    private void UpdateCraftUI()
    {
        if (Recipe == null) return;
        
        // Mettre à jour le nom de la recette
        if (recipeNameText != null)
        {
            recipeNameText.text = Recipe.product != null ? $"Craft: {Recipe.product.itemName}" : "Aucune recette";
        }
        
        // Créer un dictionnaire des ressources nécessaires
        Dictionary<string, int> requiredResources = GetRequiredResources();
        Dictionary<string, int> availableResources = GetAvailableResources();
        
        // Convertir en liste pour accès par index
        var requiredList = new List<KeyValuePair<string, int>>(requiredResources);
        
        // Mettre à jour le premier container
        if (requiredItem1Container != null)
        {
            if (requiredList.Count > 0)
            {
                var required = requiredList[0];
                int available = availableResources.ContainsKey(required.Key) ? availableResources[required.Key] : 0;
                InventoryItemData requiredItemData = GetItemDataByName(required.Key);
                UpdateRequiredItemContainer(requiredItem1Container, requiredItemData, required.Value, available);
                requiredItem1Container.gameObject.SetActive(true);
            }
            else
            {
                requiredItem1Container.gameObject.SetActive(false);
            }
        }
        
        // Mettre à jour le deuxième container
        if (requiredItem2Container != null)
        {
            if (requiredList.Count > 1)
            {
                var required = requiredList[1];
                int available = availableResources.ContainsKey(required.Key) ? availableResources[required.Key] : 0;
                InventoryItemData requiredItemData = GetItemDataByName(required.Key);
                UpdateRequiredItemContainer(requiredItem2Container, requiredItemData, required.Value, available);
                requiredItem2Container.gameObject.SetActive(true);
            }
            else
            {
                requiredItem2Container.gameObject.SetActive(false);
            }
        }
        
        // Mettre à jour le statut du craft
        UpdateCraftStatus();
    }
    
    private void UpdateRequiredItemContainer(Transform container, InventoryItemData itemData, int requiredAmount, int availableAmount)
    {
        if (container == null) return;
        
        // Récupérer les composants du container selon la structure CardRecipe
        TextMeshProUGUI nameText = container.Find("Name Bar")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI quantityText = container.Find("Text (TMP)")?.GetComponent<TextMeshProUGUI>();
        Image itemIcon = container.Find("Icon")?.GetComponent<Image>();
        Image backgroundImage = container.GetComponent<Image>();
        
        // Si les composants ne sont pas trouvés avec cette structure, essayer l'ancienne
        if (nameText == null)
            nameText = container.Find("ItemName")?.GetComponent<TextMeshProUGUI>();
        if (quantityText == null)
            quantityText = container.Find("Quantity")?.GetComponent<TextMeshProUGUI>();
        
        // Configurer le nom de l'item
        if (nameText != null && itemData != null)
        {
            nameText.text = itemData.itemName;
        }
        else if (nameText != null)
        {
            nameText.text = "Item inconnu";
        }
        
        // Configurer la quantité
        if (quantityText != null)
        {
            quantityText.text = $"{availableAmount}/{requiredAmount}";
            // Changer la couleur selon la disponibilité
            quantityText.color = availableAmount >= requiredAmount ? Color.green : Color.red;
        }
        
        // Configurer l'icône
        if (itemIcon != null && itemData != null)
        {
            if (itemData.itemIcon != null)
            {
                itemIcon.sprite = itemData.itemIcon;
                itemIcon.gameObject.SetActive(true);
            }
            else
            {
                // Si pas d'icône dans itemData, essayer de la récupérer autrement
                Sprite icon = GetItemIcon(itemData.itemName);
                if (icon != null)
                {
                    itemIcon.sprite = icon;
                    itemIcon.gameObject.SetActive(true);
                }
                else
                {
                    itemIcon.gameObject.SetActive(false);
                }
            }
        }
        
        // Changer la couleur de fond selon la disponibilité
        if (backgroundImage != null)
        {
            Color bgColor = availableAmount >= requiredAmount ? 
                new Color(0.2f, 0.8f, 0.2f, 0.3f) : // Vert transparent
                new Color(0.8f, 0.2f, 0.2f, 0.3f);   // Rouge transparent
            backgroundImage.color = bgColor;
        }
        
        // Gérer l'état "No Item" si applicable
        Transform noItemTransform = container.Find("No Item");
        if (noItemTransform != null)
        {
            noItemTransform.gameObject.SetActive(itemData == null);
        }
    }
    
    private InventoryItemData GetItemDataByName(string itemName)
    {
        // Chercher d'abord dans les ressources de la recette
        if (Recipe?.Ressources != null)
        {
            foreach (var resource in Recipe.Ressources)
            {
                if (resource.itemName == itemName)
                {
                    return resource;
                }
            }
        }
        
        // Si pas trouvé dans la recette, chercher dans les cartes d'entrée
        if (CardIn1?.GetComponent<CardManager>()?.itemData != null)
        {
            var item = CardIn1.GetComponent<CardManager>().itemData;
            if (item.itemName == itemName)
                return item;
        }
        
        if (CardIn2?.GetComponent<CardManager>()?.itemData != null)
        {
            var item = CardIn2.GetComponent<CardManager>().itemData;
            if (item.itemName == itemName)
                return item;
        }
        
        // Si toujours pas trouvé, créer un ItemData basique
        // (vous devriez adapter cette partie selon votre système d'items)
        return CreateBasicItemData(itemName);
    }
    
    private InventoryItemData CreateBasicItemData(string itemName)
    {
        // Cette méthode devrait être adaptée selon votre système
        // Retourne un ItemData basique avec juste le nom
        InventoryItemData basicItem = ScriptableObject.CreateInstance<InventoryItemData>();
        basicItem.itemName = itemName;
        basicItem.itemNb = 0;
        return basicItem;
    }
    
    private void UpdateCraftStatus()
    {
        if (craftStatusText == null) return;
        
        bool canCraft = CanCraft();
        float timeRemaining = craftCooldown - (Time.time - lastCraftTime);
        
        if (isCrafting)
        {
            craftStatusText.text = "Crafting...";
            craftStatusText.color = Color.yellow;
        }
        else if (!canCraft)
        {
            craftStatusText.text = "Ressources insuffisantes";
            craftStatusText.color = Color.red;
        }
        else if (timeRemaining > 0)
        {
            craftStatusText.text = $"Cooldown: {timeRemaining:F1}s";
            craftStatusText.color = new Color(1f, 0.5f, 0f); // Orange
        }
        else
        {
            craftStatusText.text = "Prêt à crafter!";
            craftStatusText.color = Color.green;
        }
        
        // Activer/désactiver selon l'état du craft
        craftStatusText.gameObject.SetActive(true);
    }
    
    
    private Dictionary<string, int> GetRequiredResources()
    {
        Dictionary<string, int> required = new Dictionary<string, int>();
        if (Recipe?.Ressources == null) return required;
        
        foreach (var item in Recipe.Ressources)
        {
            if (required.ContainsKey(item.itemName))
                required[item.itemName] += item.itemNb;
            else
                required[item.itemName] = item.itemNb;
        }
        return required;
    }
    
    private Dictionary<string, int> GetAvailableResources()
    {
        Dictionary<string, int> available = new Dictionary<string, int>();
        
        // Ajouter les ressources de CardIn1
        if (CardIn1?.GetComponent<CardManager>()?.itemData != null)
        {
            var item = CardIn1.GetComponent<CardManager>().itemData;
            available[item.itemName] = item.itemNb;
        }
        
        // Ajouter les ressources de CardIn2
        if (CardIn2?.GetComponent<CardManager>()?.itemData != null)
        {
            var item = CardIn2.GetComponent<CardManager>().itemData;
            if (available.ContainsKey(item.itemName))
                available[item.itemName] += item.itemNb;
            else
                available[item.itemName] = item.itemNb;
        }
        
        return available;
    }
    
    private Sprite GetItemIcon(string itemName)
    {
        // Cette méthode devrait récupérer l'icône de l'item par son nom
        // Vous devrez l'adapter selon votre système d'items
        // Exemple : return ItemDatabase.GetItemByName(itemName)?.icon;
        return null;
    }

    private bool CanCraft()
    {
        
        Dictionary<string, int> neededResources = GetRequiredResources();
        Dictionary<string, int> availableResources = GetAvailableResources();


        // Vérifier si on a assez de chaque ressource
        foreach (var needed in neededResources)
        {
            if (!availableResources.ContainsKey(needed.Key) || 
                availableResources[needed.Key] < needed.Value)
            {
                return false;
            }
        }
        
        return true;
    }

    private void DoCraft()
    {
        isCrafting = true;
        
        // Créer un dictionnaire des ressources à retirer
        Dictionary<string, int> toRemove = GetRequiredResources();

        // Sauvegarder les données avant modification pour éviter les références perdues
        InventoryItemData card1Item = null;
        InventoryItemData card2Item = null;
        
        if (CardIn1 != null && CardIn1.GetComponent<CardManager>().itemData != null)
        {
            card1Item = CardIn1.GetComponent<CardManager>().itemData;
        }
        
        if (CardIn2 != null && CardIn2.GetComponent<CardManager>().itemData != null)
        {
            card2Item = CardIn2.GetComponent<CardManager>().itemData;
        }

        // Retirer les ressources de CardIn1 d'abord
        if (card1Item != null && toRemove.ContainsKey(card1Item.itemName))
        {
            int amountToRemove = Mathf.Min(toRemove[card1Item.itemName], card1Item.itemNb);
            int remainingQuantity = card1Item.itemNb - amountToRemove;
            
            toRemove[card1Item.itemName] -= amountToRemove;
            if (toRemove[card1Item.itemName] <= 0)
            {
                toRemove.Remove(card1Item.itemName);
            }
            
            CardIn1.GetComponent<CardManager>().UnSetItem();
            if (remainingQuantity > 0)
            {
                CardIn1.GetComponent<CardManager>().SetItem(card1Item.CreateCopyWithQuantity(remainingQuantity));
            }
            
            Debug.Log($"Retiré {amountToRemove} {card1Item.itemName} de CardIn1, reste: {remainingQuantity}");
        }

        // Retirer les ressources restantes de CardIn2
        if (card2Item != null && toRemove.ContainsKey(card2Item.itemName))
        {
            int amountToRemove = Mathf.Min(toRemove[card2Item.itemName], card2Item.itemNb);
            int remainingQuantity = card2Item.itemNb - amountToRemove;
            
            toRemove[card2Item.itemName] -= amountToRemove;
            if (toRemove[card2Item.itemName] <= 0)
            {
                toRemove.Remove(card2Item.itemName);
            }
            
            CardIn2.GetComponent<CardManager>().UnSetItem();
            if (remainingQuantity > 0)
            {
                CardIn2.GetComponent<CardManager>().SetItem(card2Item.CreateCopyWithQuantity(remainingQuantity));
            }
            
            Debug.Log($"Retiré {amountToRemove} {card2Item.itemName} de CardIn2, reste: {remainingQuantity}");
        }

        // Vérifier que toutes les ressources ont été retirées
        if (toRemove.Count > 0)
        {
            Debug.LogError("Erreur: Toutes les ressources n'ont pas pu être retirées!");
            foreach (var remaining in toRemove)
            {
                Debug.LogError($"Manque encore: {remaining.Value} {remaining.Key}");
            }
            isCrafting = false;
            return;
        }

        // Mettre le résultat dans CardOut
        if (CardOut != null)
        {
            if (CardOut.GetComponent<CardManager>().itemData == null)
            {
                CardOut.GetComponent<CardManager>().SetItem(Recipe.product.CreateCopyWithQuantity(Recipe.amount));
                Debug.Log($"Produit créé: {Recipe.amount} {Recipe.product.itemName}");
            }
            else if (CardOut.GetComponent<CardManager>().itemData.itemName == Recipe.product.itemName)
            {
                int total = CardOut.GetComponent<CardManager>().itemData.itemNb + Recipe.amount;
                CardOut.GetComponent<CardManager>().UnSetItem();
                CardOut.GetComponent<CardManager>().SetItem(Recipe.product.CreateCopyWithQuantity(total));
                Debug.Log($"Produit ajouté: total maintenant {total} {Recipe.product.itemName}");
            }
            else
            {
                Debug.LogWarning("CardOut contient déjà un item différent, impossible d'ajouter le produit");
                isCrafting = false;
                return;
            }
        }
        
        // Progression des quêtes
        if (questManager != null && questManager.currentQuestIndex == 6)
        {
            questManager.Quests[6].Progress(1f);
        }

        isCrafting = false;
        Debug.Log("Craft terminé avec succès");
    }
}