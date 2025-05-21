using System.Collections;
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

    public InventoryItemData CoalItem;
    public InventoryItemData IronOre;

    private InventoryItemData product;
    private bool CanMine = false;
    private bool burning = false;

    void Update()
    {
        bool notBurning = !burning;
        bool CoalCardNull = CoalCard.GetComponent<CardManager>().itemData != null;
        var inventoryItemData = CoalCard.GetComponent<CardManager>().itemData;
        bool CoalCardCoal = inventoryItemData != null && inventoryItemData.itemName == CoalItem.itemName;
        
        if (Tile == CoalTile)
        {
            product = CoalItem;
            CanMine = true;
        }
        if (Tile == IronTile)
        {
            product = IronOre;
            CanMine = true;
        }
        
        
        if (notBurning && CoalCardNull && CoalCardCoal && CanMine)
        {
            Debug.Log("Drill");
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
            burning = false;
            yield break;
        }

        // Consommer 1 charbon immédiatement
        int newCoalQty = coal.itemData.itemNb - 1;
        coal.UnSetItem();
        if (newCoalQty > 0) coal.SetItem(CoalItem.CreateCopyWithQuantity(newCoalQty));

        // Pendant 10 secondes, produire toutes les 2 secondes
        while (elapsed < burnDuration)
        {
            // Vérifie que le joueur n’a pas retiré le charbon entre-temps
            if (coal.itemData == null || coal.itemData.itemNb < 0)
            {
                burning = false;
                yield break;
            }

            // Ajout du produit dans la carte de sortie
            if (output.itemData == null)
            {
                output.SetItem(product.CreateCopyWithQuantity(1));
            }
            else
            {
                int outQty = output.itemData.itemNb + 1;
                output.UnSetItem();
                output.SetItem(product.CreateCopyWithQuantity(outQty));
            }

            yield return new WaitForSeconds(productionInterval);
            elapsed += productionInterval;
        }

        // Terminé après 10 secondes de production
        burning = false;
    }
}
