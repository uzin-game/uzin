using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Quest
{
    public bool IsActive;
    public bool IsCompleted;
    public string Title;
    public string Description;
    public GameObject Icon;
    [CanBeNull] public Quest NextQuest;
    public float currAmount;
    public float requiredAmount;
    
    public TMP_Text QuestTitle;
    public TMP_Text QuestText;
    public Slider QuestProgress;


    public Quest(string title, string description, float objectif, TMP_Text questTitle, TMP_Text questText, Slider questProgress)
    {
        IsActive = false;
        IsCompleted = false;
        Title = title;
        Description = description;
        currAmount = 0;
        requiredAmount = objectif;
        QuestTitle = questTitle;
        QuestText = questText;
        QuestProgress = questProgress;
    }

    public void Initialize()
    {
        IsActive = true;
        QuestTitle.text = Title;
        QuestText.text = Description;
    }

    private void Complete()
    {
        IsActive = false;
        IsCompleted = true;
    }

    public void Progress(float progress)
    {
        currAmount += progress;
        QuestProgress.value = currAmount/requiredAmount;
        if (currAmount >= requiredAmount)
        {
            Complete();
        }
    }
}
