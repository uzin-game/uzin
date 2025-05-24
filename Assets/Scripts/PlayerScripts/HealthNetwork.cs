using UnityEngine;
using Unity.Netcode;
using Ilumisoft.HealthSystem;
using RedstoneinventeGameStudio;

[RequireComponent(typeof(HealthComponent))]
public class HealthNetwork : NetworkBehaviour
{
    public float maxHealth = 100f;

    public NetworkVariable<float> CurrentHealth = new(
        writePerm: NetworkVariableWritePermission.Server,
        readPerm: NetworkVariableReadPermission.Everyone
    );

    private HealthComponent _healthComponent;

    public GameObject DeathPanel;

    public GameObject Panel;

    void Awake()
    {
        DeathPanel.SetActive(false);
        _healthComponent = GetComponent<HealthComponent>();
    }

    public override void OnNetworkSpawn()
    {
        CurrentHealth.OnValueChanged += OnHealthVariableChanged;

        if (IsServer)
        {
            CurrentHealth.Value = maxHealth;
            _healthComponent.SetHealth(maxHealth);
        }
        else
        {
            _healthComponent.SetHealth(CurrentHealth.Value);
        }
    }

    private void OnHealthVariableChanged(float previousValue, float newValue)
    {
        _healthComponent.SetHealth(newValue);
    }

    private void ApplyDamageInternal(float damage)
    {
        if (!IsServer) return;

        float h = Mathf.Max(CurrentHealth.Value - damage, 0f);
        CurrentHealth.Value = h;
        
        if (h == 0f)
        {
            for (int i = 0; i < Panel.transform.childCount; i++)
            {
                var child = Panel.transform.GetChild(i);
                var card = child.GetComponent<CardManager>();

                if (card != null && card.itemData == null)
                {
                    card.UnSetItem();
                }
            }
            
            DeathPanel.SetActive(true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ApplyDamageServerRpc(float damage)
    {
        ApplyDamageInternal(damage);
    }

    public void ApplyDamage(float damage)
    {
        if (IsServer)
        {
            ApplyDamageInternal(damage);
        }
        else
        {
            ApplyDamageServerRpc(damage);
        }
    }
}