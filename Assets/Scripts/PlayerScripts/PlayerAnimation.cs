using Unity.Netcode;
using UnityEngine;

namespace PlayerScripts
{
    public class PlayerAnimation : NetworkBehaviour {
        public Animator animator;
        private Rigidbody2D rb;

        private NetworkVariable<int> animState = new NetworkVariable<int>(0,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        void Start() {
            rb = GetComponent<Rigidbody2D>();

            // Sync animation across clients
            animState.OnValueChanged += (oldState, newState) => {
                animator.SetInteger("State", newState);
            };
        }

        void Update() {
            if (!IsOwner) return; // Only local player updates movement

            Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            rb.linearVelocity = moveInput.normalized * 5f; // Adjust speed as needed

            int newState = GetAnimationState(moveInput);
            if (animState.Value != newState) {
                animState.Value = newState; // Sync across network
            }
        }

        int GetAnimationState(Vector2 direction) {
            if (direction == Vector2.zero) return 0; // Idle
            if (direction.y > 0) return 1; // Walk Up
            if (direction.y < 0) return 2; // Walk Down
            if (direction.x > 0) return 3; // Walk Right
            if (direction.x < 0) return 4; // Walk Left
            return 0;
        }
    }
}