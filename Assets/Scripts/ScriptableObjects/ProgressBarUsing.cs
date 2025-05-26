using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBarController : MonoBehaviour
{
    [Header("Progress Bar Components")]
    public Image fillImage;              // L'image Fill
    public TextMeshProUGUI progressText; // Texte optionnel (ex: "50%")
    
    [Header("Settings")]
    public float animationSpeed = 2f;    // Vitesse d'animation
    public bool smoothTransition = true; // Animation fluide
    
    private float targetProgress = 0f;   // Valeur cible (0-1)
    private float currentProgress = 0f;  // Valeur actuelle
    
    void Update()
    {
        if (smoothTransition)
        {
            // Animation fluide vers la valeur cible
            currentProgress = Mathf.MoveTowards(currentProgress, targetProgress, 
                                              animationSpeed * Time.deltaTime);
        }
        else
        {
            currentProgress = targetProgress;
        }
        
        // Appliquer la valeur à l'image
        if (fillImage != null)
        {
            fillImage.fillAmount = currentProgress;
        }
        
        // Mettre à jour le texte
        if (progressText != null)
        {
            progressText.text = $"{Mathf.RoundToInt(currentProgress * 100)}%";
        }
    }
    
    /// <summary>
    /// Définir le progrès (valeur entre 0 et 1)
    /// </summary>
    public void SetProgress(float progress)
    {
        targetProgress = Mathf.Clamp01(progress);
    }
    
    /// <summary>
    /// Définir le progrès avec valeurs min/max
    /// </summary>
    public void SetProgress(float current, float max)
    {
        if (max <= 0) return;
        SetProgress(current / max);
    }
    
    /// <summary>
    /// Changer la couleur de la barre
    /// </summary>
    public void SetColor(Color color)
    {
        if (fillImage != null)
        {
            fillImage.color = color;
        }
    }
    
    /// <summary>
    /// Définir immédiatement sans animation
    /// </summary>
    public void SetProgressImmediate(float progress)
    {
        targetProgress = Mathf.Clamp01(progress);
        currentProgress = targetProgress;
    }
}