using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UIScrpits
{
    public class NetworkmanagerUI : NetworkBehaviour
    {
        public static NetworkmanagerUI Instance { get; private set; }

        [SerializeField] private Button ServerButton;
        [SerializeField] private Button ClientButton;
        [SerializeField] private Button HostButton;
        [SerializeField] private GameObject NetworkManagerUI;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

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
}