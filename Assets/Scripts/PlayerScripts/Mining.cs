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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        }

        if (ironTile == tile)
        {
            InventoryUsing.Increment(Iron);
        }
    }

}