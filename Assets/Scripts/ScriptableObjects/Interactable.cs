using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjects
{
    public class Interactable : MonoBehaviour
    {
        public bool IsInRange;
        public KeyCode InteractKey;
        public UnityEvent InteractAction;


        void Update()
        {
            if (IsInRange)
            {
                if (Input.GetKeyDown(InteractKey))
                {
                    InteractAction.Invoke();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                IsInRange = true;
                Debug.Log("Interacted with " + collision.gameObject.name + ",now in range");
            }
        }
        
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                IsInRange = false;
                Debug.Log("Interacted with " + collision.gameObject.name + ",now out of range");
            }
        }
    }
}