using RedstoneinventeGameStudio;
using Unity.Netcode;
using UnityEngine;

public class DroppedItem : NetworkBehaviour
{
    public NetworkObject networkObject;
    public Rigidbody2D rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Machines"))
        {
            if (collision.gameObject.CompareTag("Furnace"))
            {
                FurnaceInteraction furnaceInteraction = collision.gameObject.GetComponent<FurnaceInteraction>();
                FurnaceUsing furnaceUsing = furnaceInteraction.furnaceScript;
                if (furnaceUsing.burning)
                {
                    if (furnaceUsing.InputCard.GetComponent<CardManager>().itemData.itemName == ""){}
                }
            }
        }
    }
}
