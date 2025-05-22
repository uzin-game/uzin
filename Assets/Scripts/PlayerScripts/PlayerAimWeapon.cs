using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;
//using CodeMonkey.Utils;

public class PlayerAimWeapon : NetworkBehaviour {

    private Transform aimTransform;
    public Camera cam;
    private Animator aimAnimator;
    public event EventHandler<OnShootEventArgs> OnShoot;
    private Transform aimGunEndPos;
    
    private float shootCooldown = 0.5f;
    private float lastShootTime = -Mathf.Infinity;



    public class OnShootEventArgs : EventArgs
    {
        public Vector3 gunEndPos;
        public Vector3 shootPos;
    }

    private void Awake() {
        aimTransform = transform.Find("Aim");
        //cam = GetComponent<Camera>();
        aimAnimator = aimTransform.GetComponent<Animator>();
        aimGunEndPos = aimTransform.Find("gunEndPos");
    }

    private void Update() 
    {
        if (!IsOwner)
        {
            return;
        }
        if (Time.deltaTime != 0)
        {
            HandleAiming();
            HandleShooting();
        }
    }

    private void HandleAiming() {
        Vector3 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 aimDirection = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimTransform.eulerAngles = new Vector3(0, 0, angle);

        Vector3 a = Vector3.one;
        if (angle > 90 || angle < -90)
        {
            a.y = -1f;
        }
        else
        {
            a.y = 1f;
        }
        
        aimTransform.localScale = a;
    }

    private void HandleShooting() 
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= lastShootTime + shootCooldown)
        {
            lastShootTime = Time.time;
            
            Vector3 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 1f;
            
            OnShoot?.Invoke(this, new OnShootEventArgs
            {
                gunEndPos = new Vector3(aimGunEndPos.position.x, aimGunEndPos.position.y, 1f),
                shootPos = mousePosition
            });
        }
    }
}
