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
        public Quest CurrentQuest => Quests[currentQuestIndex];
        public HealthNetwork healthNetwork;

        void Start()
        {
            healthNetwork = GetComponent<HealthNetwork>();
            
            if (!IsOwner) panel.SetActive(false);
            Quests = new List<Quest>();
            
            
            Quest first = new Quest("Explorez les alentours et trouvez du charbon", "Bougez avec z,q,s,d", 1f, QuestText,
                QuestDescription, Porgress, this);
            Quest second = new Quest("Récuperez du charbon", "Minez des minerais avec `A`", 25f, QuestText,
                QuestDescription, Porgress, this);
            Quest thrid = new Quest("Posez une foreuse", "appuyez sur `P` pour placer un machine", 1f, QuestText,
                QuestDescription, Porgress, this);
            Quest frouth = new Quest("Minez 25 fer a l'aide de la foreuse", "La foreuse marche en brûlant du charbon, que vous devez mettre dans la case orange", 25f, QuestText,
                QuestDescription, Porgress, this);
            Quest fitfh = new Quest("Faites cuire le fer dans un four", "Commencez par placer un four", 25f, QuestText,
                QuestDescription, Porgress, this);
            Quests.Add(first);
            Quests.Add(second);
            Quests.Add(thrid);
            Quests.Add(frouth);
            Quests.Add(fitfh);

            Quests[0].Initialize();
        }
        
        public void CompleteQuestWithDelay()
        {
            healthNetwork.ApplyDamage(-10f);
            CurrentQuest.IsActive = false;
            currentQuestIndex++;
            CurrentQuest.timer = CurrentQuest.delay;
            CurrentQuest.isWaiting = true;
        }

        void Update()
        {
            if (CurrentQuest.isWaiting)
            {
                CurrentQuest.timer -= Time.deltaTime;

                if (CurrentQuest.timer <= 0f)
                {
                    CurrentQuest.isWaiting = false;
                    Quests[currentQuestIndex].Initialize();
                }
            }
        }
    }
}