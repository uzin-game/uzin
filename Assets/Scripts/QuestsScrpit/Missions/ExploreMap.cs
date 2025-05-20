namespace QuestsScrpit.Missions
{
    public class ExploreMap : Quest.QuestGoal
    {
        public override string GetDescription()
        {
            return $"Explorez les environnements";
        }
        
        private void OnExplore(){}
    }
}