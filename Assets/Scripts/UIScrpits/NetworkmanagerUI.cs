using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.UI;

public class NetworkmanagerUI : NetworkBehaviour
{
    [SerializeField] private Button ServerButton;
    [SerializeField] private Button ClientButton;
    [SerializeField] private Button HostButton;
    [SerializeField] private GameObject NetworkManagerUI;


    private void Awake()
    {
        ServerButton.onClick.AddListener(() =>
        {
            if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.StartServer();
                NetworkManagerUI.SetActive(false);

            }
        });
        
        ClientButton.onClick.AddListener(() =>
        {
            if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.StartClient();
                NetworkManagerUI.SetActive(false);

            }
        });
        
        HostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            NetworkManagerUI.SetActive(false);

        });
    }
}
