using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace QuestsScrpit
{
    public class QuestManager : NetworkBehaviour
    {
        [SerializeField] public List<Quest> Quests;
        public TMP_Text QuestText;
        public TMP_Text QuestDescription;
        [SerializeField] public Slider Porgress;
        public int currentQuestIndex = 0;
        public TileBase charbon;
        public GameObject panel;

        void Start()
        {
            Quests = new List<Quest>();
            Quest first = new Quest("Bienvenue", "Explorez les alentours et trouvez du charbon", 1f, QuestText,
                QuestDescription, Porgress);
            Quests.Add(first);
            Quests[0].Initialize();
        }
        
        public override void OnNetworkSpawn()
        {
            
        }

        private void Update()
        {
            if (Quests[currentQuestIndex].IsCompleted)
            {
                currentQuestIndex++;
                Quests[currentQuestIndex].Initialize();
            }
        }
    }
}