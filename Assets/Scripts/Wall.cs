using System;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.RegisterEvent("Attack", OnAttack);
    }

    private void OnCollisionEnter(Collision collision)
    {
        transform.localScale *= 1.3f;
    }
    
    private void OnAttack(object damage)
    {
        Debug.Log($"Wall took {damage} damage");
        transform.localScale *= 0.5f;
    }
}