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
        [SerializeField] private GameObject StartGameUI;
        [SerializeField] private GameObject MainMenuUI;
        [SerializeField] private GameObject MenuCanvas;



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
                    StartGameUI.SetActive(false);
                    MenuCanvas.SetActive(false);
                }
            });

            HostButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartHost();
                StartGameUI.SetActive(false);
                MenuCanvas.SetActive(false);
            });
            
            GoBackButton.onClick.AddListener(() =>
            {
                MainMenuUI.SetActive(true);
                StartGameUI.SetActive(false);
            });
        }
    }
}