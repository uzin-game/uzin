using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Sword", menuName = "Scriptable Objects/Items/Sword")]
    public class SwordSO : ScriptableObject
    {
        public string swordName;
        public int damage;
        public float attackSpeed;
    }
}