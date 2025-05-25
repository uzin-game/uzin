using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using QuestsScrpit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Quest 
{
    public bool IsActive;
    public string Title;
    public string Description;
    public GameObject Icon;
    [CanBeNull] public Quest NextQuest;
    public float currAmount;
    public float requiredAmount;
    
    public TMP_Text QuestTitle;
    public TMP_Text QuestText;
    public Slider QuestProgress;
    public QuestManager QuestManager;
    public bool isWaiting = false;
    
    public float delay = 1f; // délai en secondes
    public float timer = 0f;



    public Quest(string title, string description, float objectif, TMP_Text questTitle, TMP_Text questText, Slider questProgress, QuestManager questManager)
    {
        IsActive = false;
        Title = title;
        Description = description;
        currAmount = 0;
        requiredAmount = objectif;
        QuestTitle = questTitle;
        QuestText = questText;
        QuestProgress = questProgress;
        QuestManager = questManager;
    }

    public void Initialize()
    {
        QuestTitle.text = Title;
        QuestText.text = Description;
        //QuestProgress.maxValue = requiredAmount;
        QuestProgress.value = 0;

        // Sync to clients
        if (QuestManager.IsServer)
        {
            QuestManager.QuestTitle.Value = Title;
            QuestManager.QuestDescription.Value = Description;
            QuestManager.QuestProgress.Value = 0;
        }

        IsActive = true;
    }

    public void CompleteQuestWithDelay()
    {
        IsActive = false;
        QuestManager.currentQuestIndex++;
        timer = delay;
        isWaiting = true;
    }

    void Update()
    {
        if (isWaiting)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                isWaiting = false;
                QuestManager.Quests[QuestManager.currentQuestIndex].Initialize();
            }
        }
    }

    public void Progress(float progress)
    {
        currAmount += progress;
        float normalizedProgress = Mathf.Clamp01(currAmount / requiredAmount);
        QuestProgress.value = normalizedProgress;

        if (QuestManager.IsServer)
        {
            QuestManager.QuestProgress.Value = normalizedProgress;
        }

        if (currAmount >= requiredAmount)
        {
            QuestManager.CompleteQuestWithDelay();
        }
    }
}
