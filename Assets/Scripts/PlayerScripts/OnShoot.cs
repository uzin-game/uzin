using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using CodeMonkey.Utils;
using Effects;

public class OnShoot : MonoBehaviour {

    [SerializeField] private PlayerAimWeapon playerAimWeapon;
    private CameraShake cameraShake;
    public Camera cam;
    public float shakeAmount;
    public Material weaponTracerMaterial;
    //public float meshHeight;
    public float meshWidth;

    private void Start() {
        playerAimWeapon.OnShoot += PlayerAimWeapon_OnShoot;
        cameraShake = cam.GetComponent<CameraShake>();
    }

    private void PlayerAimWeapon_OnShoot(object sender, PlayerAimWeapon.OnShootEventArgs e) {
        cameraShake.shakeDuration = 0.2f;
        cameraShake.shakeAmount = shakeAmount;
        //Debug.DrawLine(e.gunEndPos,e.shootPos, Color.white, 0.1f);
        CreateWeaponTracer(e.gunEndPos,e.shootPos);
    }

    private void CreateWeaponTracer(Vector3 from, Vector3 target)
    {
        Vector3 direction = (target - from).normalized;
        float ogDistance = Vector3.Distance(from, target);
        RaycastHit2D hit = Physics2D.Raycast(from, direction, ogDistance, LayerMask.GetMask("Enemy"));
        if (hit.collider != null && hit.collider.CompareTag("Enemy")) 
        {
            Debug.Log("nice shot : " + hit.collider.name);
            //target = hit.collider.transform.position;
            hit.collider.GetComponent<FadeOut>().enabled = true;
            hit.collider.GetComponent<FlyAI>().IsFrozen = true;
        }
        float distance = Vector3.Distance(from, target);
        float eulerZ = UtilsClass.GetAngleFromVectorFloat(direction);
        Vector3 tracerSpawnPosition = from + direction * (distance * 0.5f);
        Material tmpWeaponTracerMaterial = new Material(weaponTracerMaterial);
        tmpWeaponTracerMaterial.SetTextureScale("_MainTex", new Vector2(distance/40f, 1f));
        World_Mesh worldMesh = World_Mesh.CreateGood(tracerSpawnPosition, eulerZ, distance, 0.1f , tmpWeaponTracerMaterial, null, 10000);
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