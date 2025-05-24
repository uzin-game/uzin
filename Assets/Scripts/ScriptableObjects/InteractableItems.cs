using RedstoneinventeGameStudio;
using UnityEngine;

public class InteractableItems : MonoBehaviour
{
    public InventoryItemData inventoryItem;
    public Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.MovePosition(rb.linearVelocity);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Furnace"))
        {
            bool enter = other.GetComponent<FurnaceInteraction>().drillUsing.GetComponent<FurnaceUsing>()
                .ConveyorUsing(inventoryItem);
            if (enter) Destroy(this.gameObject);
        }
    }
}
