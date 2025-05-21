using RedstoneinventeGameStudio;
using UnityEngine;
using System.Collections;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public GameObject InputCard;
    public GameObject CoalCard;
    public GameObject OutputCard;

    public InventoryItemData CoalItem;
    public InventoryItemData IronOre;
    public InventoryItemData IronIngot;

    private bool burning = false;

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
            if (output.itemData == null)
            {
                output.SetItem(IronIngot.CreateCopyWithQuantity(1));
            }
            else
            {
                int outQty = output.itemData.itemNb + 1;
                output.UnSetItem();
                output.SetItem(IronIngot.CreateCopyWithQuantity(outQty));
            }

            // Attendre 2 secondes pour la prochaine production
            yield return new WaitForSeconds(productionInterval);
            elapsed += productionInterval;
        }

        // Fin du cycle de combustion du charbon
        burning = false;
    }
}
