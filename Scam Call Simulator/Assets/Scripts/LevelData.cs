using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScamGame/LevelData")]
public class LevelData : ScriptableObject
{
    public string levelName;
    [TextArea] public string levelDescription;
    public int[] npcRosterIndices; // which indices in GameManager.npcRoster belong to this level
}