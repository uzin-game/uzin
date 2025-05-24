using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjects
{
    public class Interactable : NetworkBehaviour
    {
        public bool IsInRange;
        public KeyCode InteractKey;
        public UnityEvent InteractAction;
        [SerializeField] public FurnaceInteraction furnaceInteraction;


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
                furnaceInteraction.playerInRange = collision.gameObject;
                Debug.Log("Interacted with " + collision.gameObject.name + ",now in range");
            }
        }
        
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                IsInRange = false;
                if (furnaceInteraction.IsInteracting)
                {
                    furnaceInteraction.Interact();
                }
                furnaceInteraction.playerInRange = null;
                Debug.Log("Interacted with " + collision.gameObject.name + ",now out of range");
            }
        }
    }
}