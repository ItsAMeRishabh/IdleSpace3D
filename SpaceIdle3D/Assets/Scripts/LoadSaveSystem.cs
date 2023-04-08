using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[RequireComponent(typeof(GameManager))]
public class LoadSaveSystem : MonoBehaviour
{
    [SerializeField] private string folderName = "Saves";
    private string saveDir;

    private GameManager gameManager;

    void Awake()
    {
        saveDir = Application.persistentDataPath + "/" + folderName + "/";
        if (!Directory.Exists(saveDir))
        {
            Directory.CreateDirectory(saveDir);
        }
        gameManager = GetComponent<GameManager>();
    }

    public List<string> GetProfilesList()
    {
        List<string> profilesList = new List<string>();

        if (Directory.Exists(saveDir))
        {
            string[] files = Directory.GetFiles(saveDir);
            foreach (string file in files)
            {
                profilesList.Add(Path.GetFileNameWithoutExtension(file));
            }
        }

        return profilesList;
    }

    public PlayerData LoadProfile(string profileName)
    {
        string jsonData = File.ReadAllText(saveDir + string.Concat(profileName, ".json"));
        PlayerData playerData = JsonUtility.FromJson<PlayerData>(jsonData);
        return playerData;
    }

    public void Save(PlayerData playerData)
    {
        string jsonData = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(saveDir + string.Concat(playerData.profileName, ".json"), jsonData);

        GUIUtility.systemCopyBuffer = saveDir + string.Concat(playerData.profileName, ".json");
    }

    [ContextMenu("Save Game")]
    public void Save()
    {
        string jsonData = JsonUtility.ToJson(gameManager.playerData, true);
        File.WriteAllText(saveDir + string.Concat(gameManager.playerData.profileName, ".json"), jsonData);

        GUIUtility.systemCopyBuffer = saveDir + string.Concat(gameManager.playerData.profileName, ".json");
    }

    [ContextMenu("Load Game")]
    public PlayerData Load()
    {
        string jsonData = File.ReadAllText(saveDir + string.Concat(gameManager.playerData.profileName, ".json"));
        PlayerData playerData = JsonUtility.FromJson<PlayerData>(jsonData);
        return playerData;
    }
}
