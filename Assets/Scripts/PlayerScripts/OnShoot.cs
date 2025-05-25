using UnityEngine;
using Unity.Netcode;
using CodeMonkey.Utils;
using Effects;

public class OnShoot : NetworkBehaviour
{
    [SerializeField] private PlayerAimWeapon playerAimWeapon;
    private CameraShake cameraShake;
    public Camera cam;
    public float shakeAmount;
    public Material weaponTracerMaterial;

    private void Start()
    {
        if (IsOwner)
        {
            playerAimWeapon.OnShoot += PlayerAimWeapon_OnShoot;
        }

        if (cam != null)
        {
            cameraShake = cam.GetComponent<CameraShake>();
        }
    }

    private void PlayerAimWeapon_OnShoot(object sender, PlayerAimWeapon.OnShootEventArgs e)
    {
        FireWeaponServerRpc(e.gunEndPos, e.shootPos);
    }

    [ServerRpc]
    private void FireWeaponServerRpc(Vector3 gunEndPos, Vector3 shootPos)
    {
        ProcessDamage(gunEndPos, shootPos);
        ShowWeaponEffectsClientRpc(gunEndPos, shootPos);
    }

    [ClientRpc]
    private void ShowWeaponEffectsClientRpc(Vector3 gunEndPos, Vector3 shootPos)
    {
        CreateWeaponTracer(gunEndPos, shootPos);

        if (IsOwner && cameraShake != null)
        {
            cameraShake.shakeDuration = 0.2f;
            cameraShake.shakeAmount = shakeAmount;
        }
    }

    private void ProcessDamage(Vector3 from, Vector3 target)
    {
        Vector3 direction = (target - from).normalized;
        float distance = Vector3.Distance(from, target);
        RaycastHit2D[] hits = Physics2D.RaycastAll(from, direction, distance, LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Enemy"))
            {
                var enemy = hit.collider;
                var healthNetwork = enemy.GetComponent<HealthNetwork>();
                if (healthNetwork != null)
                {
                    healthNetwork.ApplyDamage(100);
                }
            }
        }
    }

    private void CreateWeaponTracer(Vector3 from, Vector3 target)
    {
        Vector3 direction = (target - from).normalized;
        float distance = Vector3.Distance(from, target);
        float eulerZ = UtilsClass.GetAngleFromVectorFloat(direction);

        Vector3 tracerSpawnPosition = from + direction * (distance * 0.5f);
        Material tmpWeaponTracerMaterial = new Material(weaponTracerMaterial);

        tmpWeaponTracerMaterial.SetTextureScale("_MainTex", new Vector2(distance / 40f, 1f));

        World_Mesh worldMesh = World_Mesh.CreateGood(tracerSpawnPosition, eulerZ, distance, 0.1f,
            tmpWeaponTracerMaterial, null, 10000);
        float timer = .1f;

        FunctionUpdater.Create(() =>
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                worldMesh.DestroySelf();
                return true;
            }

            return false;
        });
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner && playerAimWeapon != null)
        {
            playerAimWeapon.OnShoot -= PlayerAimWeapon_OnShoot;
        }

        base.OnNetworkDespawn();
    }
}