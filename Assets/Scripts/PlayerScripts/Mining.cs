using QuestsScrpit;
using RedstoneinventeGameStudio;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Mining : MonoBehaviour
{
    GameObject player;
    public InventoryUsing InventoryUsing;
    public TileBase tile;
    public InventoryItemData coal;
    public InventoryItemData Iron;
    public TileBase ironTile;
    public TileBase charbonTile;
    
    private QuestManager qM;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        qM = player.GetComponent<QuestManager>();
    }
    public void Mine()
    {
        if (tile == null)
        {
            Debug.LogWarning("tile est null !");
            return;
        }

        if (charbonTile == null)
        {
            Debug.LogWarning("charbonTile est null !");
            return;
        }

        if (InventoryUsing == null)
        {
            Debug.LogWarning("InventoryUsing est null !");
            return;
        }

        if (charbonTile == tile)
        {
            InventoryUsing.Increment(coal);
            if (qM != null && qM.currentQuestIndex == 1) qM.Quests[qM.currentQuestIndex].Progress(1f);
        }

        if (ironTile == tile)
        {
            InventoryUsing.Increment(Iron);
            
        }
    }

}