using UnityEngine;
using CodeMonkey.Utils;
using Effects;

public class OnShoot : MonoBehaviour
{

    [SerializeField] private PlayerAimWeapon playerAimWeapon;
    private CameraShake cameraShake;
    public Camera cam;
    public float shakeAmount;
    public Material weaponTracerMaterial;
    //public float meshHeight

    private void Start()
    {
        playerAimWeapon.OnShoot += PlayerAimWeapon_OnShoot;
        cameraShake = cam.GetComponent<CameraShake>();
    }

    private void PlayerAimWeapon_OnShoot(object sender, PlayerAimWeapon.OnShootEventArgs e)
    {
        cameraShake.shakeDuration = 0.2f;
        cameraShake.shakeAmount = shakeAmount;
        CreateWeaponTracer(e.gunEndPos, e.shootPos);
    }

    private void CreateWeaponTracer(Vector3 from, Vector3 target)
    {
        
        Vector3 direction = (target - from).normalized;
        float ogDistance = Vector3.Distance(from, target);
        RaycastHit2D[] hits = Physics2D.RaycastAll(from, direction, ogDistance, LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Enemy"))
            {
                var enemy = hit.collider;

                enemy.GetComponent<HealthNetwork>().ApplyDamage(100);
            }
        }
        float distance = Vector3.Distance(from, target);
        float eulerZ = UtilsClass.GetAngleFromVectorFloat(direction);

        Vector3 tracerSpawnPosition = from + direction * (distance * 0.5f);
        Material tmpWeaponTracerMaterial = new Material(weaponTracerMaterial);

        tmpWeaponTracerMaterial.SetTextureScale("_MainTex", new Vector2(distance / 40f, 1f));

        World_Mesh worldMesh = World_Mesh.CreateGood(tracerSpawnPosition, eulerZ, distance, 0.1f, tmpWeaponTracerMaterial, null, 10000);
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
}