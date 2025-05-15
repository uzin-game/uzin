using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FadeIn : MonoBehaviour
{
    [Tooltip("Duration of the fade-in in seconds")]
    public float duration = 1f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        var c = spriteRenderer.color;
        c.a = 0f;
        spriteRenderer.color = c;
    }

    private void Start()
    {
        StartCoroutine(FadeCoroutine());
    }

    private IEnumerator FadeCoroutine()
    {
        float elapsed = 0f;
        var c = spriteRenderer.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsed / duration);
            spriteRenderer.color = c;
            yield return null;
        }

        c.a = 1f;
        spriteRenderer.color = c;
    }
}
