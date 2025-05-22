using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Effects
{
    [RequireComponent(typeof(NetworkObject))]
    public class FadeOut : NetworkBehaviour
    {
        [Tooltip("Duration of the fade-out in seconds")]
        public float duration = 1f;

        public Rigidbody2D rb;

        private SpriteRenderer spriteRenderer;
        private Graphic[] uiGraphics;

        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
            uiGraphics = GetComponentsInChildren<Graphic>(true);

            if (spriteRenderer != null)
            {
                var c = spriteRenderer.color;
                c.a = 1f;
                spriteRenderer.color = c;
            }

            foreach (var g in uiGraphics)
            {
                var c = g.color;
                c.a = 1f;
                g.color = c;
            }

            rb = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }

            StartCoroutine(FadeAndDespawn());
        }

        private IEnumerator FadeAndDespawn()
        {
            yield return StartCoroutine(FadeCoroutine());
            RequestDeSpawnFly();
        }

        private IEnumerator FadeCoroutine()
        {
            float elapsed = 0f;
            float startAlpha = 1f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float newAlpha = Mathf.Lerp(startAlpha, 0f, t);

                if (spriteRenderer != null)
                {
                    var c = spriteRenderer.color;
                    c.a = newAlpha;
                    spriteRenderer.color = c;
                }

                foreach (var g in uiGraphics)
                {
                    var c = g.color;
                    c.a = newAlpha;
                    g.color = c;
                }

                yield return null;
            }

            if (spriteRenderer != null)
            {
                var c = spriteRenderer.color;
                c.a = 0f;
                spriteRenderer.color = c;
            }

            foreach (var g in uiGraphics)
            {
                var c = g.color;
                c.a = 0f;
                g.color = c;
            }
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
                RequestDeSpawnFlyServerRpc();
        }
    }
}
