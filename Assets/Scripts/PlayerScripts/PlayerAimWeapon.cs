using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerAimWeapon : NetworkBehaviour
{
    private Transform aimTransform;
    public Camera cam;
    private Animator aimAnimator;
    public event EventHandler<OnShootEventArgs> OnShoot;
    private Transform aimGunEndPos;
    private float shootCooldown = 0.5f;
    private float lastShootTime = -Mathf.Infinity;

    private NetworkVariable<float> networkAimAngle = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> networkAimScaleY = new NetworkVariable<float>(1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public class OnShootEventArgs : EventArgs
    {
        public Vector3 gunEndPos;
        public Vector3 shootPos;
    }

    private void Awake()
    {
        aimTransform = transform.Find("Aim");
        aimAnimator = aimTransform.GetComponent<Animator>();
        aimGunEndPos = aimTransform.Find("gunEndPos");
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner)
        {
            networkAimAngle.OnValueChanged += OnAimAngleChanged;
            networkAimScaleY.OnValueChanged += OnAimScaleYChanged;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
        {
            networkAimAngle.OnValueChanged -= OnAimAngleChanged;
            networkAimScaleY.OnValueChanged -= OnAimScaleYChanged;
        }
        base.OnNetworkDespawn();
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

    private void HandleAiming()
    {
        Vector3 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 aimDirection = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        aimTransform.eulerAngles = new Vector3(0, 0, angle);

        Vector3 scale = Vector3.one;
        if (angle > 90 || angle < -90)
        {
            scale.y = -1f;
        }
        else
        {
            scale.y = 1f;
        }
        aimTransform.localScale = scale;

        if (Mathf.Abs(networkAimAngle.Value - angle) > 1f)
        {
            networkAimAngle.Value = angle;
        }

        if (Mathf.Abs(networkAimScaleY.Value - scale.y) > 0.1f)
        {
            networkAimScaleY.Value = scale.y;
        }
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

    private void OnAimAngleChanged(float previousValue, float newValue)
    {
        if (!IsOwner)
        {
            aimTransform.eulerAngles = new Vector3(0, 0, newValue);
        }
    }

    private void OnAimScaleYChanged(float previousValue, float newValue)
    {
        if (!IsOwner)
        {
            Vector3 scale = aimTransform.localScale;
            scale.y = newValue;
            aimTransform.localScale = scale;
        }
    }
}