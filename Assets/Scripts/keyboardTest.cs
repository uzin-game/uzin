using UnityEngine;

public class KeyboardInputHandler : MonoBehaviour
{
    public Inventory inventory; // Référence à l'inventaire

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("N");
            Item TestItem = new Item("Cuivre", 2, 200);
            if (inventory != null)
            {
                inventory.AddItem(TestItem);
            }
            else
            {
                Debug.LogError("La référence à Inventory est manquante.");
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("L");
            Item TestItem = new Item("Fer", 4, 200);
            if (inventory != null)
            {
                inventory.AddItem(TestItem);
            }
            else
            {
                Debug.LogError("La reférence a inventory est manquante.");
            }
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("O");
            Item TestItem = new Item("Fer", 4, 200);
            if (inventory != null)
            {
                inventory.RemoveItem(TestItem);
            }
            else
            {
                Debug.LogError("La reférence a inventory est manquante.");
            }
        }
    }
}