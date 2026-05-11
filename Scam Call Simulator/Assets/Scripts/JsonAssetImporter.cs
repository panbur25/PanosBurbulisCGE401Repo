using UnityEngine;
using UnityEditor;
using System.IO;

public class JsonAssetImporter : EditorWindow
{
    [MenuItem("Tools/Import Scenarios from JSON")]
    public static void ImportScenarios()
    {
        string jsonPath = EditorUtility.OpenFilePanel("Select Scenario JSON", Application.dataPath, "json");
        if (string.IsNullOrEmpty(jsonPath)) return;

        string json = File.ReadAllText(jsonPath);
        ScenarioData asset = ScriptableObject.CreateInstance<ScenarioData>();

        // JsonUtility can't do top-level objects directly into ScriptableObjects,
        // so we use a plain serializable mirror first
        ScenarioMirror mirror = JsonUtility.FromJson<ScenarioMirror>(json);
        asset.victimName = mirror.victimName;
        asset.steps = mirror.steps;

        string outputFolder = "Assets/Data/Scenarios";
        Directory.CreateDirectory(outputFolder);

        string assetPath = $"{outputFolder}/{asset.victimName}_Scenario.asset";
        AssetDatabase.CreateAsset(asset, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created: {assetPath}");
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    [MenuItem("Tools/Import All Scenarios from Folder")]
    public static void ImportAllScenarios()
    {
        string folderPath = EditorUtility.OpenFolderPanel("Select Folder with Scenario JSONs", Application.dataPath, "");
        if (string.IsNullOrEmpty(folderPath)) return;

        string outputFolder = "Assets/Data/Scenarios";
        Directory.CreateDirectory(outputFolder);

        string[] jsonFiles = Directory.GetFiles(folderPath, "*.json");
        int count = 0;

        foreach (string jsonPath in jsonFiles)
        {
            string json = File.ReadAllText(jsonPath);
            ScenarioMirror mirror = JsonUtility.FromJson<ScenarioMirror>(json);
            if (mirror == null || mirror.victimName == null) continue;

            ScenarioData asset = ScriptableObject.CreateInstance<ScenarioData>();
            asset.victimName = mirror.victimName;
            asset.steps = mirror.steps;

            string assetPath = $"{outputFolder}/{mirror.victimName}_Scenario.asset";
            AssetDatabase.CreateAsset(asset, assetPath);
            count++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Imported {count} scenario(s) to {outputFolder}");
    }

    [MenuItem("Tools/Import LevelData from JSON")]
    public static void ImportLevelData()
    {
        string jsonPath = EditorUtility.OpenFilePanel("Select LevelData JSON", Application.dataPath, "json");
        if (string.IsNullOrEmpty(jsonPath)) return;

        string json = File.ReadAllText(jsonPath);

        // Wrap array for JsonUtility
        LevelDataArray wrapper = JsonUtility.FromJson<LevelDataArray>("{\"levels\":" + json + "}");
        if (wrapper?.levels == null) { Debug.LogError("Failed to parse LevelData JSON."); return; }

        string outputFolder = "Assets/Data/Levels";
        Directory.CreateDirectory(outputFolder);

        foreach (var mirror in wrapper.levels)
        {
            LevelData asset = ScriptableObject.CreateInstance<LevelData>();
            asset.levelName = mirror.levelName;
            asset.levelDescription = mirror.levelDescription;
            asset.npcRosterIndices = mirror.npcRosterIndices;

            string assetPath = $"{outputFolder}/{mirror.levelName.Replace(" ", "_")}.asset";
            AssetDatabase.CreateAsset(asset, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Imported {wrapper.levels.Length} level(s) to {outputFolder}");
    }
}

// --- Mirror classes (JsonUtility can't deserialize directly into ScriptableObjects) ---

[System.Serializable]
public class ScenarioMirror
{
    public string victimName;
    public DialogueStep[] steps;   // reuses your existing DialogueStep & DialogueChoice classes
}

[System.Serializable]
public class LevelMirror
{
    public string levelName;
    public string levelDescription;
    public int[] npcRosterIndices;
}

[System.Serializable]
public class LevelDataArray
{
    public LevelMirror[] levels;
}