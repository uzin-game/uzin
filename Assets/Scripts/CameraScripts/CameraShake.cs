using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public Transform camTransform;

    public float shakeDuration = 0f;
    public float shakeAmount = 0.3f;
    public float decreaseFactor = 1.0f;

    private Vector3 originalPos;

    void Awake()
    {
        if (camTransform == null)
        {
            camTransform = GetComponent<Transform>();
        }
        
        originalPos.z = 1f;
    }

    void Start()
    {
        originalPos = new Vector3(0f, 0f, -10f);
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            Vector2 shakeOffset = Random.insideUnitCircle * shakeAmount;
            camTransform.localPosition = new Vector3(
                originalPos.x + shakeOffset.x,
                originalPos.y + shakeOffset.y,
                originalPos.z
            );

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            camTransform.localPosition = originalPos;
        }
    }
}