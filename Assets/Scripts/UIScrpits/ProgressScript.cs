using TMPro;
using UnityEngine;

public class ProgressScript : MonoBehaviour
{
    public TMP_Text targetText;

    public void UpdateText(float value)
    {
        value *= 100f;
        if (targetText != null)
            targetText.text = value.ToString("0") + "%"; // or "0.00" for decimals
    }
}