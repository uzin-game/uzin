using System;
using System.Collections;
using ScriptableObjects;
using UnityEngine;

public class Player : MonoBehaviour
{
    // [SerializeField] is used to make private fields visible in the inspector
    [SerializeField] private SwordSO sword;
    [SerializeField] private Rigidbody rb;
    private bool _isAttacking = false;

    private void OnAttack()
    {
        Debug.Log("Key pressed");
        if (!_isAttacking)
            StartCoroutine(AttackCoroutine());
    }
    
    private void OnJump()
    {
        Debug.Log("Jumping");
        rb.AddForce(2 * transform.up, ForceMode.VelocityChange);
    }

    private IEnumerator AttackCoroutine()
    {
        _isAttacking = true;
        Debug.Log($"Attacking with {sword.swordName} for {sword.damage} damage");
        EventManager.TriggerEvent("Attack", sword.damage);
        yield return new WaitForSeconds(1 / sword.attackSpeed);
        _isAttacking = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Player collided with {collision.gameObject.name}");
        // if hit object named wall, the player is pushed backward a lot
        if (collision.gameObject.name == "Wall")
        {
            rb.AddForce(-transform.forward * 10, ForceMode.VelocityChange);
        }
    }

    private void Update()
    {
       rb.AddForce(transform.forward, ForceMode.Acceleration); 
    }
}