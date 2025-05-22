using UnityEngine;

public class CraftOpen : MonoBehaviour
{
    public GameObject CraftPanel;
    //private bool isOpen = false;
    void Start()
    {
        CraftPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleInventory();
        }
        if (CraftPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape)) ToggleInventory();
    }


    public void ToggleInventory()
    {
        //isOpen = !isOpen;
        CraftPanel.SetActive(!CraftPanel.activeSelf);
    }
}
