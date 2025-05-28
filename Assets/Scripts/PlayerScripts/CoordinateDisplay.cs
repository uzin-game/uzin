using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CoordinateDisplay : NetworkBehaviour
{
    [Header("UI References")]
    public TMP_Text coordinateText; // Ou TextMeshProUGUI si tu utilises TextMeshPro

    public GameObject Panel;
    
    [Header("Settings")]
    public Transform playerTransform; // Le transform du joueur local
    public float updateInterval = 0.1f; // Mise à jour toutes les 0.1 secondes
    
    private float timer;
    
    void Start()
    {
        if (!IsOwner)
        {
            Panel.SetActive(false);
            coordinateText.text = "";
        }
        // Si playerTransform n'est pas assigné, trouve le joueur local
        if (playerTransform == null)
        {
            // Adapte selon ton système de détection du joueur local
            GameObject localPlayer = GameObject.FindGameObjectWithTag("Player");
            if (localPlayer != null)
            {
                playerTransform = localPlayer.transform;
            }
        }
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= updateInterval && playerTransform != null && coordinateText != null && IsOwner)
        {
            // Affiche les coordonnées arrondies
            int x = Mathf.RoundToInt(playerTransform.position.x);
            int y = Mathf.RoundToInt(playerTransform.position.y);
            
            coordinateText.text = $"X: {x}\nY: {y}";
            
            timer = 0f;
        }
    }
}