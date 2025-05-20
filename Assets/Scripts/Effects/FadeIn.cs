using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class FadeIn : MonoBehaviour
{
    [Tooltip("Duration of the fade-in in seconds")]
    public float duration = 1f;

    private SpriteRenderer spriteRenderer;
    private Graphic[] uiGraphics;

    private void Awake()
    {
        // Récupère le SpriteRenderer et initialise son alpha à 0
        spriteRenderer = GetComponent<SpriteRenderer>();
        var spriteColor = spriteRenderer.color;
        spriteColor.a = 0f;
        spriteRenderer.color = spriteColor;

        // Récupère tous les éléments UI (Image, Text, etc.) enfants et initialise leur alpha à 0
        uiGraphics = GetComponentsInChildren<Graphic>(true);
        foreach (var g in uiGraphics)
        {
            var c = g.color;
            c.a = 0f;
            g.color = c;
        }
    }

    private void Start()
    {
        StartCoroutine(FadeCoroutine());
    }

    private IEnumerator FadeCoroutine()
    {
        float elapsed = 0f;
        float startAlpha = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float newAlpha = Mathf.Lerp(startAlpha, 1f, t);

            // Applique l'alpha au SpriteRenderer
            var spriteC = spriteRenderer.color;
            spriteC.a = newAlpha;
            spriteRenderer.color = spriteC;

            // Applique l'alpha à tous les composants UI
            foreach (var g in uiGraphics)
            {
                var uiC = g.color;
                uiC.a = newAlpha;
                g.color = uiC;
            }

            yield return null;
        }

        // Assure alpha = 1 en fin
        var finalSprite = spriteRenderer.color;
        finalSprite.a = 1f;
        spriteRenderer.color = finalSprite;

        foreach (var g in uiGraphics)
        {
            var uiC = g.color;
            uiC.a = 1f;
            g.color = uiC;
        }
    }
}