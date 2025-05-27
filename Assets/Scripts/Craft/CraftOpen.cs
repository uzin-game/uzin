using UnityEngine;

public class CraftOpen : MonoBehaviour
{
    public GameObject CraftPanel;
    private GameObject Aim;
    void Start()
    {
        CraftPanel.SetActive(false);
        Aim = GameObject.Find("Aim");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleInventory();
        }
        if (CraftPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleInventory();
        }
    }


    public void ToggleInventory()
    {
        Aim.SetActive(!Aim.activeSelf);
        CraftPanel.SetActive(!CraftPanel.activeSelf);
    }
}
