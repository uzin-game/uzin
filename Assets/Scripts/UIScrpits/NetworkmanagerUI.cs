using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] private TMP_InputField IPInputField;


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
                    UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                    transport.SetConnectionData(IPInputField.text, 7777);
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