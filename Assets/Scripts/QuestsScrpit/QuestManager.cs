﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.Collections;
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
        public TMP_Text QuestDescriptionText;
        [SerializeField] public Slider Porgress;
        public int currentQuestIndex = 0;
        public TileBase charbon;
        public GameObject panel;
        public GameObject WinPanel;

        //public Quest CurrentQuest => Quests[currentQuestIndex];
        //public HealthNetwork healthNetwork;

        public NetworkVariable<FixedString128Bytes> QuestTitle = new();
        public NetworkVariable<FixedString128Bytes> QuestDescription = new();
        public NetworkVariable<float> QuestProgress = new();

        public NetworkVariable<float> QuestGoal = new(1f, NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);


        void InitializeFirstQuest()
        {
            //healthNetwork = GetComponent<HealthNetwork>();
            Quests = new List<Quest>();


            Quest frist = new Quest("Explorez les alentours et trouvez du charbon", "Bougez avec z,q,s,d.", 1f,
                QuestText,
                QuestDescriptionText, Porgress, this);
            Quest second = new Quest("Récuperez du charbon", "Minez des minerais avec `A`.", 10f, QuestText,
                QuestDescriptionText, Porgress, this);
            Quest thrid = new Quest("Posez une foreuse sur un filon de fer", "Appuyez sur `P` pour placer une machine, puis posez une foreuse sur du fer.", 1f, QuestText,
                QuestDescriptionText, Porgress, this);
            Quest frouth = new Quest("Minez 10 fer a l'aide de la foreuse",
                "La foreuse marche en brûlant du charbon, que vous devez mettre dans la case orange.", 10f, QuestText,
                QuestDescriptionText, Porgress, this);
            Quest fitfh = new Quest("Faites cuire le fer dans un four", "Commencez par placer un four, puis faites cuire votre fer dedans.", 10f, QuestText,
                QuestDescriptionText, Porgress, this);
            Quest sitxh = new Quest("Craftez maintenant des tôles de fer", "Appuyz sur `C`  pour commencer a crafter",
                1f, QuestText,
                QuestDescriptionText, Porgress, this);
            Quest senevth = new Quest("Il en faut plus !","Placez le constructeur pour crafter plus vite", 1f, QuestText,
                QuestDescriptionText, Porgress, this);
            Quest eigitht = new Quest("Mettez en place l'automatisation",
                "Cliquez sur les boutons jaune et vert et choisissez une sortie sur un four ou une foreuse", 2f, QuestText,
                QuestDescriptionText, Porgress, this);
            Quest ninth = new Quest("Maintenant, a vous de faire!", "Pour commencer la fusée, craftez un navigateur.", 1f, QuestText,
                QuestDescriptionText, Porgress, this);
            Quest tneht = new Quest("Bravo !", "Il nous faut maintenant un système de direction.", 1f, QuestText,
                QuestDescriptionText, Porgress, this);
            Quest eleventh = new Quest("On y est presque !"," Il nous manque juste 1 de bout de chassis pour la fusée.", 1f, QuestText,QuestDescriptionText,Porgress, this);
            Quest twelveth = new Quest("Partons !"," Fabrique cette fusée.",1f, QuestText,QuestDescriptionText,Porgress, this);
            
            Quests.Add(frist);
            Quests.Add(second);
            Quests.Add(thrid);
            Quests.Add(frouth);
            Quests.Add(fitfh);
            Quests.Add(sitxh);
            Quests.Add(senevth);
            Quests.Add(eigitht);
            Quests.Add(ninth);
            Quests.Add(tneht);
            Quests.Add(eleventh);
            Quests.Add(twelveth);

            Quests[currentQuestIndex].Initialize();
           
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                InitializeFirstQuest();
            }

            QuestTitle.OnValueChanged += (oldVal, newVal) => { QuestText.text = newVal.ToString(); };

            QuestDescription.OnValueChanged += (oldVal, newVal) => { QuestDescriptionText.text = newVal.ToString(); };

            QuestProgress.OnValueChanged += (oldVal, newVal) => { Porgress.value = newVal; };
        }

        public void ShowWinScreen()
        {
            WinPanel.SetActive(true);
        }

        public void CompleteQuestWithDelay()
        {
            if (IsServer)
            {
                HealAllPlayers(10f);
            }

            Quests[currentQuestIndex].IsActive = false;
            currentQuestIndex++;
            if (currentQuestIndex == Quests.Count) ShowWinScreen();
            Quests[currentQuestIndex].timer = Quests[currentQuestIndex].delay;
            Quests[currentQuestIndex].isWaiting = true;
        }

        private void HealAllPlayers(float healAmount)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in players)
            {
                HealthNetwork healthNetwork = player.GetComponent<HealthNetwork>();

                if (healthNetwork != null)
                {
                    healthNetwork.ApplyDamage(-healAmount);
                    Debug.Log($"Healed player {player.name} for {healAmount} HP");
                }
            }
        }

        void Update()
        {
            if (IsClient)
            {
                QuestText.text = QuestTitle.Value.ToString();
                QuestDescriptionText.text = QuestDescription.Value.ToString();
                //Porgress.value = QuestProgress.Value;
            }

            if (IsServer && Quests[currentQuestIndex].isWaiting)
            {
                Quests[currentQuestIndex].timer -= Time.deltaTime;
                if (Quests[currentQuestIndex].timer <= 0f)
                {
                    Quests[currentQuestIndex].isWaiting = false;
                    Quests[currentQuestIndex].Initialize();
                }
            }
        }
    }
}