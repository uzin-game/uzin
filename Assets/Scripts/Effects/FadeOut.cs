using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Effects
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class FadeOut : NetworkBehaviour
    {
        [Tooltip("Duration of the fade-in in seconds")]
        public float duration = 1f;
        public Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            var c = spriteRenderer.color;
            c.a = 1f;
            spriteRenderer.color = c;
            rb = GetComponent<Rigidbody2D>();
        }

        void OnEnable()
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.isKinematic = true;
            StartCoroutine(FadeAndDespawnCoroutine()); 
        }

        private IEnumerator FadeAndDespawnCoroutine()
        {
            StartCoroutine(FadeCoroutine()); // Start fading
            yield return new WaitForSeconds(duration); // Wait for the fade to complete
            RequestDeSpawnFly();
        }

        private IEnumerator FadeCoroutine()
        {
            float elapsed = 0f;
            var c = spriteRenderer.color;
            float startAlpha = c.a; // starting alpha (usually 1)

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                c.a = Mathf.Lerp(startAlpha, 0f, t); // fade from current alpha to 0
                spriteRenderer.color = c;
                yield return null;
            }

            c.a = 0f;
            spriteRenderer.color = c;
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void RequestDeSpawnFlyServerRpc()
        {
            if (!IsServer) return;

            GetComponent<NetworkObject>().Despawn();
        }

        public void RequestDeSpawnFly()
        {
            if (IsClient)
            {
                RequestDeSpawnFlyServerRpc();
            }
        }
    }
}