using System.Collections.Generic;
using System.Linq;
using QuestsScrpit;
using UnityEngine;
using UnityEngine.Events;

namespace QuestsScrpit
{
    public class Quest : ScriptableObject
    {
        [System.Serializable]
        public struct Info
        {
            public string Name;
            public string Description;
        }
        
        [Header("Info")] public Info Information;

        public bool Completed { get; protected set; }
        public QuestCompletedEvent QuestCompleted;
        public abstract class QuestGoal : ScriptableObject
        {
            protected string Description;
            public int CurrentAmount { get; protected set; }
            public int RequiredAmount;
            
            public bool Completed { get; protected set; }
            [HideInInspector] public UnityEvent OnCompleted;

            public virtual string GetDescription()
            {
                return Description;
            }

            public virtual void Initialize()
            {
                Completed = false;
            }

            protected void Evaluate()
            {
                if (CurrentAmount >= RequiredAmount)
                {
                    Complete();
                }
            }

            private void Complete()
            {
                Completed = true;
                OnCompleted?.Invoke();
                OnCompleted?.RemoveAllListeners();
            }
        }
        
        public List<QuestGoal> Goals;

        public void Initialize()
        {
            Completed = false;
            QuestCompleted = new QuestCompletedEvent();

            foreach (var goal in Goals)
            {
                goal.Initialize();
                goal.OnCompleted.AddListener(delegate { CheckGoals(); });
            }
        }

        private void CheckGoals()
        {
            Completed = Goals.All(g => g.Completed);
            if (Completed)
            {
                QuestCompleted.Invoke(this);
                QuestCompleted.RemoveAllListeners();
            }
        }

    }
}

public class QuestCompletedEvent : UnityEvent<Quest>
{
    
}