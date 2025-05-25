using Effects;
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


    void Awake()
    {
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
            Debug.Log("Enemy died on server.");
            HandleDeathClientRpc();
        }
    }     
    
    [ClientRpc]
    private void HandleDeathClientRpc()
    {
        var fadeOut = GetComponent<FadeOut>();
        var ai = GetComponent<FlyAI>();

        if (fadeOut != null)
            fadeOut.enabled = true;

        if (ai != null)
            ai.IsFrozen = true;
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