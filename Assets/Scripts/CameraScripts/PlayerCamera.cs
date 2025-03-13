using UnityEngine;

namespace CameraScripts
{
    public class PlayerCamera : MonoBehaviour
    {
        [Header("Follow Settings")] public Transform playerTransform; // Le joueur à suivre
        public Vector3 offset = new Vector3(0, 0, -10); // Décalage de la caméra par rapport au joueur
        public float smoothSpeed = 0.125f; // Vitesse de lissage du suivi

        [Header("Zoom Settings")] public float zoomSpeed = 5f; // Vitesse du zoom (multiplicateur)
        [SerializeField] private float minZoom = 1f; // Zoom minimum (pour une caméra orthographique, correspond à orthographicSize)
        [SerializeField] private float maxZoom = 20f; // Zoom maximum (pour une caméra orthographique)

        private Camera cam;

        private void Start()
        {
            // Récupère le composant Camera attaché à cet objet
            cam = GetComponent<Camera>();
            if (cam == null)
            {
                Debug.LogError("PlayerCamera : aucun composant Camera trouvé sur cet objet !");
            }
        }

        private void LateUpdate()
        {
            // Suivi fluide du joueur
            if (playerTransform != null)
            {
                Vector3 desiredPosition = playerTransform.position + offset;
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                transform.position = smoothedPosition;
            }

            // Gestion du zoom via la molette de la souris
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (cam != null)
            {
                if (cam.orthographic)
                {
                    cam.orthographicSize -= scrollInput * zoomSpeed;
                    cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
                }
                else
                {
                    cam.fieldOfView -= scrollInput * zoomSpeed;
                    cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minZoom, maxZoom);
                }
            }
        }
    }
}