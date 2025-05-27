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
    public bool isDead = false;

    public GameObject DeathPanel;


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

        // Try to find DeathPanel for this client
        if (IsOwner && DeathPanel == null)
        {
            DeathPanel = GameObject.Find("DeathPanel"); // Or use another reliable way
        }
    }

    private void OnHealthVariableChanged(float previousValue, float newValue)
    {
        _healthComponent.SetHealth(newValue);

        if (!isDead && newValue <= 0f)
        {
            isDead = true;

            if (IsServer && CompareTag("Player"))
            {
                HandleDeathServer();
            }
        }
    }

    private void HandleDeathServer()
    {
        Debug.Log($"{name} died on server");

        // Teleport to (0, 0, 0)
        transform.position = Vector3.zero;

        // Notify client to show death panel if it's a player
        if (CompareTag("Player"))
        {
            ShowDeathClientRpc(OwnerClientId);
        }
    }

    [ClientRpc]
    private void ShowDeathClientRpc(ulong clientId)
    {
        if (!IsOwner || NetworkManager.Singleton.LocalClientId != clientId) return;

        Debug.Log("Showing death panel to player " + clientId);

        if (DeathPanel != null)
        {
            DeathPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("DeathPanel not set on player");
        }

        ResetHealthLocally();
    }

    private void ResetHealthLocally()
    {
        if (IsOwner)
        {
            Debug.Log("Resetting health to max locally");

            // Only tell the server to reset health
            ResetHealthServerRpc();
        }
    }

    [ServerRpc]
    private void ResetHealthServerRpc()
    {
        Debug.Log("Resetting health to max on server");
        CurrentHealth.Value = maxHealth;
        isDead = false;

        // Optional: teleport to respawn point again
        transform.position = new Vector3(0, 0, 0);
    }

    private void ApplyDamageInternal(float damage)
    {
        if (!IsServer) return;

        float h = Mathf.Clamp(CurrentHealth.Value - damage, 0f, maxHealth);
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