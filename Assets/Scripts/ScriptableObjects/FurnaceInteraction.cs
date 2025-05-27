using System;
using System.Collections.Generic;
using RedstoneinventeGameStudio;
using ScriptableObjects;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class FurnaceInteraction : NetworkBehaviour
{
    public bool IsInteracting;
    [SerializeField] private GameObject FurnaceUI;
    public GameObject playerInRange;
    public GameObject Panel;
    public DrillUsing drillUsing;
    public FurnaceUsing furnaceScript;
    public ConstructeurUsing constructeurScript;
    public bool IsSelecting;
    public Button SelectButton;
    public bool OutputLeTruc = false;
    public TMP_Dropdown dropdown; 
    public List<GameObject> outputPrefabs; 
    void Start()
    {
        IsSelecting = false;
        SelectButton.onClick.AddListener(delegate
        {
            if (IsSelecting)
            {
                Debug.Log("Select");
                dropdown.gameObject.SetActive(false);
                IsSelecting = false;
            }
            else
            {
                Debug.Log("UnSelect");
                dropdown.gameObject.SetActive(true);
                IsSelecting = true;
            }
        });
    }

    public void SetTargetMachine(FurnaceUsing target)
    {
        if (furnaceScript != null)
        {
            furnaceScript = target;
        
            furnaceScript.OutputPrefabs = outputPrefabs;
        
            dropdown.onValueChanged.RemoveAllListeners();
        
            dropdown.value = (int)furnaceScript.OutputMode.Value;
        
            dropdown.onValueChanged.AddListener(OnOutputModeChanged);
        }
    }

    public void SetTargetDrillUsing(DrillUsing target)
    {
        if (drillUsing != null)
        {
            drillUsing = target;
            
            drillUsing.OutputPrefabs = outputPrefabs;
        
            dropdown.onValueChanged.RemoveAllListeners();
        
            dropdown.value = (int)drillUsing.OutputMode.Value;
        
            dropdown.onValueChanged.AddListener(OnOutputModeChangedDrillUsing);
        }
        
    }
    
    private void OnOutputModeChanged(int newValue)
    {
        Debug.Log("OnOutputModeChanged, calling ServerRPC");
        SetOutputModeServerRpc(newValue);
    }

    private void OnOutputModeChangedDrillUsing(int newValue)
    {
        SetDrillOutputModeServerRpc(newValue);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetDrillOutputModeServerRpc(int newValue)
    {
        if (Enum.IsDefined(typeof(MachineOutputMode), newValue))
        {
            drillUsing.SetOutputMode((MachineOutputMode)newValue);
        }
        else
        {
            Debug.LogWarning("Invalid output mode received from client.");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetOutputModeServerRpc(int newMode)
    {
        if (Enum.IsDefined(typeof(MachineOutputMode), newMode))
        {
            furnaceScript.SetOutputMode((MachineOutputMode)newMode);
        }
        else
        {
            Debug.LogWarning("Invalid output mode received from client.");
        }
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

