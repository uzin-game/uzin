using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UIScrpits
{
    public class NetworkmanagerUI : NetworkBehaviour
    {
        public static NetworkmanagerUI Instance { get; private set; }

        [SerializeField] private Button ClientButton;
        [SerializeField] private Button HostButton;
        [SerializeField] private Button GoBackButton;
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
            
            GoBackButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            });
        }
    }
}