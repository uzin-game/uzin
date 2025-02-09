using UnityEngine;

namespace CameraScripts
{
    public class PlayerCamera : MonoBehaviour
    {
        public Transform playerTransform; // The player this camera should follow
        public Vector3 offset = new Vector3(0, 0, -10); // Camera offset from the player
        public float smoothSpeed = 0.125f; // Smoothing speed

        private void LateUpdate()
        {
            if (playerTransform != null)
            {
                // Calculate the desired position
                Vector3 desiredPosition = playerTransform.position + offset;

                // Smoothly move the camera to the desired position
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                transform.position = smoothedPosition;
            }
        }
    }
}