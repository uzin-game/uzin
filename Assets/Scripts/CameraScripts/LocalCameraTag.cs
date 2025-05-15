using Unity.Netcode;
using UnityEngine;

public class LocalCamera : MonoBehaviour
{
    private void Start()
    {
        // Check if this is the local player's camera
        if (transform.parent != null && transform.parent.GetComponent<NetworkObject>().IsLocalPlayer)
        {
            // Set this camera as the MainCamera
            gameObject.tag = "MainCamera";
        }
        else
        {
            // Disable the camera for remote players
            GetComponent<Camera>().enabled = false;
            //GetComponent<AudioListener>().enabled = false; // Disable audio listener for remote players
        }
    }
}