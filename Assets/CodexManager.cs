using UnityEngine;

public class CodexManager : MonoBehaviour
{
    public CodexRoot codexData;

    void Awake()
    {
        LoadCodexData();
    }

    void LoadCodexData()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("JSON/codex");
        if (jsonText != null)
        {
            codexData = JsonUtility.FromJson<CodexRoot>(jsonText.text);
            Debug.Log("Codex loaded with " + codexData.categories.Count + " categories.");
        }
        else
        {
            Debug.LogError("Codex JSON not found!");
        }
    }
}
