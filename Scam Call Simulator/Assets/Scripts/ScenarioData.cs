using UnityEngine;

[CreateAssetMenu(fileName = "NewScenario", menuName = "ScamGame/Scenario")]
public class ScenarioData : ScriptableObject
{
    public string victimName;
    public DialogueStep[] steps;
    public string resultSummary;
}