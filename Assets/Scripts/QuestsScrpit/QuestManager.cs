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

        void Start()
        {
            Quests = new List<Quest>();
            
            
            Quest first = new Quest("Explorez les alentours et trouvez du charbon", "Bougez avec z,q,s,d", 1f, QuestText,
                QuestDescription, Porgress, this);
            Quest second = new Quest("Récuperez du charbon", "Minez des minerais avec `A`", 25f, QuestText,
                QuestDescription, Porgress, this);
            Quest thrid = new Quest("Posez une foreuse", "appuyez sur `P` pour placer un machine", 1f, QuestText,
                QuestDescription, Porgress, this);
            Quests.Add(first);
            Quests.Add(second);
            Quests.Add(thrid);

            Quests[0].Initialize();
        }
        
        public void CompleteQuestWithDelay()
        {
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