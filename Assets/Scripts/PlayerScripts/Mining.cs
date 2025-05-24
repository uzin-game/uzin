using QuestsScrpit;
using RedstoneinventeGameStudio;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class Mining : MonoBehaviour
{
    GameObject player;
    public InventoryUsing InventoryUsing;

    public TileBase tile; // La tuile ciblée (à miner)

    public TileBase charbonTile;
    public TileBase ironTile;
    public TileBase tileCopper;
    public TileBase tileGold;

    public InventoryItemData coal;
    public InventoryItemData Iron;
    public InventoryItemData copper;
    public InventoryItemData gold;

    private QuestManager qM;

    // Dictionnaire tuile -> minerai
    private Dictionary<TileBase, InventoryItemData> tileToOre;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        qM = player.GetComponent<QuestManager>();

        tileToOre = new Dictionary<TileBase, InventoryItemData>
        {
            { charbonTile, coal },
            { ironTile, Iron },
            { tileCopper, copper },
            { tileGold, gold }
        };
    }

    public void Mine()
    {
        if (tile == null)
        {
            Debug.LogWarning("La tuile à miner est null !");
            return;
        }

        if (InventoryUsing == null)
        {
            Debug.LogWarning("InventoryUsing n’est pas assigné !");
            return;
        }

        if (tileToOre.ContainsKey(tile))
        {
            InventoryItemData minedItem = tileToOre[tile];
            InventoryUsing.Increment(minedItem);

            Debug.Log($"Tuile minée : {tile.name}, objet ajouté : {minedItem.itemName}");
            if (qM != null)
            {
                int questIndex = qM.currentQuestIndex;

                if ((tile == charbonTile && questIndex == 1) ||
                    (tile == ironTile && questIndex == 2) ||
                    (tile == tileCopper && questIndex == 3) ||
                    (tile == tileGold && questIndex == 4))
                {
                    qM.Quests[questIndex].Progress(1f);
                }
            }
        }
        else
        {
            Debug.LogWarning("Tuile inconnue. Aucun minerai associé.");
        }
    }
}
