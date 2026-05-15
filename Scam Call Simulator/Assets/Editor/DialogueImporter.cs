using UnityEngine;
using UnityEditor;
using System.IO;

public class DialogueImporter : EditorWindow
{
    [MenuItem("Tools/Import Dialogues from JSON")]
    public static void ImportJson()
    {
        // Path to the JSON file inside Assets/Dialogues
        string jsonPath = Application.dataPath + "/Dialogues/dialogue_data.json";

        // Check if the JSON actually exists
        if (!File.Exists(jsonPath))
        {
            Debug.LogError($"Could not find dialogue_data.json at: {jsonPath}. Make sure the file is inside the 'Dialogues' folder!");
            return;
        }

        // Create the Dialogues folder if for some reason it doesn't exist
        if (!AssetDatabase.IsValidFolder("Assets/Dialogues"))
        {
            AssetDatabase.CreateFolder("Assets", "Dialogues");
        }

        string jsonText = File.ReadAllText(jsonPath);

        // This helper class must exist to tell Unity how to parse the list
        DialogueListWrapper data = JsonUtility.FromJson<DialogueListWrapper>(jsonText);

        if (data == null || data.allDialogues == null)
        {
            Debug.LogError("Failed to parse JSON. Check your formatting or variable names.");
            return;
        }

        foreach (var item in data.allDialogues)
        {
            DialogueSO asset = ScriptableObject.CreateInstance<DialogueSO>();

            // Assigning data from JSON to the ScriptableObject
            asset.entryID = item.entryID;
            asset.isScammer = item.isScammer;
            asset.steps = item.steps;
            asset.successEnding = item.successEnding;
            asset.failureEnding = item.failureEnding;

            // Naming convention: Jordan_Scammer.asset or Jordan_Victim.asset
            string suffix = item.isScammer ? "_Scammer" : "_Victim";
            string assetPath = $"Assets/Dialogues/{item.entryID}{suffix}.asset";

            // Save the asset to the project
            AssetDatabase.CreateAsset(asset, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Successfully imported {data.allDialogues.Length} scenarios with grouped naming!");
    }
}