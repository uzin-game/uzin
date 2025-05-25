using System.Collections.Generic;
using RedstoneinventeGameStudio;
using ScriptableObjects;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class FurnaceInteraction : MonoBehaviour
{
    public bool IsInteracting;
    [SerializeField] private GameObject FurnaceUI;
    public GameObject playerInRange;
    public GameObject Panel;
    public DrillUsing drillUsing;
    public Vector3 ItemOutpusPosition;
    public Button NorthButton;
    public Button EastButton;
    public Button SouthButton;
    public FurnaceUsing furnaceScript;
    public Button WestButton;
    public bool IsSelecting;
    public Button SelectButton;
    public bool OutputLeTruc = false;

    void Start()
    {
        IsSelecting = false;
        NorthButton.onClick.AddListener(delegate
        {
            ItemOutpusPosition = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
            Debug.Log("North");
            StopSelecting();
            OutputLeTruc = true;
        });
        EastButton.onClick.AddListener(delegate
        {
            ItemOutpusPosition = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            Debug.Log("East");
            StopSelecting();
            OutputLeTruc = true;
        });
        WestButton.onClick.AddListener(delegate
        {
            ItemOutpusPosition = new Vector3(transform.position.x , transform.position.y - 1, transform.position.z);
            Debug.Log("West");
            StopSelecting();
            OutputLeTruc = true;
        });
        SouthButton.onClick.AddListener(delegate
        {
            ItemOutpusPosition = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
            Debug.Log("South");
            StopSelecting();
            OutputLeTruc = true;
        });
        SelectButton.onClick.AddListener(delegate
        {
            if (IsSelecting)
            {
                Debug.Log("Select");
                StopSelecting();
            }
            else
            {
                Debug.Log("UnSelect");
                Select();
            }
        });
    }

    void Select()
    {
        IsSelecting = true;
        NorthButton.gameObject.SetActive(true);
        EastButton.gameObject.SetActive(true);
        WestButton.gameObject.SetActive(true);
        SouthButton.gameObject.SetActive(true);
    }

    void StopSelecting()
    {
        IsSelecting = false;
        NorthButton.gameObject.SetActive(false);
        EastButton.gameObject.SetActive(false);
        WestButton.gameObject.SetActive(false);
        SouthButton.gameObject.SetActive(false);
    }

    public void Interact()
    {
        if (!IsInteracting)
        {
            if (furnaceScript != null)furnaceScript.OnInterfaceOpen();
            IsInteracting = true;
            SetFurnaceInventory();
            
            FurnaceUI.SetActive(true);
            playerInRange.GetComponent<PlayerAimWeapon>().enabled = false;
        }

        else if (IsInteracting)
        {
            if (furnaceScript != null)furnaceScript.OnInterfaceClose();
            IsInteracting = false;
            SetPlayerBackInventory();
            FurnaceUI.SetActive(false);
            playerInRange.GetComponent<PlayerAimWeapon>().enabled = true;
        }
    }

    void Update()
    {
        if (IsInteracting && Input.GetKeyDown(KeyCode.Escape))
        {
            IsInteracting = false;
            SetPlayerBackInventory();
            FurnaceUI.SetActive(false);
            playerInRange.GetComponent<PlayerAimWeapon>().enabled = true;
        }
    }

    public void SetFurnaceInventory()
    {

        if (playerInRange == null || Panel == null) return;

        // Trouve Inventaire et Panel
        Transform inventaireTransform = FindDeepChild(playerInRange.transform, "Inventaire");
        if (inventaireTransform == null)
        {
            return;
        }

        Transform panelTransform = inventaireTransform.Find("Panel");
        if (panelTransform == null)
        {
            return;
        }

        var playerCards = GetInventoryCards(panelTransform);
        var furnaceCards = GetInventoryCards(Panel.transform);

        int count = Mathf.Min(playerCards.Count, furnaceCards.Count);
        for (int i = 0, j = 0; i < count; i++, j++)
        {
            while (j < furnaceCards.Count && furnaceCards[j].machineProperty)
                j++;

            if (j >= furnaceCards.Count) break;

            furnaceCards[j].SetItem(playerCards[i].itemData);
        }
        foreach (var cm in playerCards)
        {
            cm.UnSetItem();
        }
    }


    public void SetPlayerBackInventory()
    {
        if (playerInRange == null || Panel == null) return;

        Transform inventaireTransform = FindDeepChild(playerInRange.transform, "Inventaire");
        if (inventaireTransform == null)
        {
            return;
        }

        Transform panelTransform = inventaireTransform.Find("Panel");
        if (panelTransform == null)
        {
            return;
        }

        var playerCards = GetInventoryCards(panelTransform);
        var furnaceCards = GetInventoryCards(Panel.transform);

        int count = Mathf.Min(playerCards.Count, furnaceCards.Count);
        for (int i = 0, j = 0; i < count; i++, j++)
        {
            while (j < furnaceCards.Count && furnaceCards[j].machineProperty)
                j++;

            if (j >= furnaceCards.Count) break;

            playerCards[i].SetItem(furnaceCards[j].itemData);
        }

        foreach (var cm in Panel.GetComponentsInChildren<CardManager>())
        {
            if (cm.machineProperty) continue;
            cm.UnSetItem();
        }
    }

    private List<CardManager> GetInventoryCards(Transform root)
    {
        List<CardManager> cards = new();
        foreach (var cm in root.GetComponentsInChildren<CardManager>())
        {
            if (!cm.machineProperty)
                cards.Add(cm);
        }
        return cards;
    }
    public static Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            var result = FindDeepChild(child, name);
            if (result != null) return result;
        }
        return null;
    }


}

