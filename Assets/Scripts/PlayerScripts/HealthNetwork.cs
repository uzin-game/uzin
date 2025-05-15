using UnityEngine;
using Unity.Netcode;
using Ilumisoft.HealthSystem;

[RequireComponent(typeof(HealthComponent))]
public class NetworkHealth : NetworkBehaviour
{
    public float maxHealth = 100f;

    public NetworkVariable<float> CurrentHealth = new NetworkVariable<float>(
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

    [ServerRpc(RequireOwnership = false)]
    public void ApplyDamageServerRpc(float damage)
    {
        float h = Mathf.Max(CurrentHealth.Value - damage, 0f);
        CurrentHealth.Value = h;
    }
}
