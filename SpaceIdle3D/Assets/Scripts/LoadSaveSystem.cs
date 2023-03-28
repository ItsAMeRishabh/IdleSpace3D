using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[RequireComponent(typeof(GameManager))]
public class LoadSaveSystem : MonoBehaviour
{
    [SerializeField] private float saveInterval = 5f;
    [SerializeField] private string folderName = "Saves";
    private Coroutine saveCoroutine;
    private WaitForSeconds saveWait;

    private GameManager gameManager;

    void Awake()
    {
        gameManager = GetComponent<GameManager>();
        saveWait = new WaitForSeconds(saveInterval);
        saveCoroutine = StartCoroutine(SaveCoroutine());
    }

    IEnumerator SaveCoroutine()
    {
        while (true)
        {
            //Save(gameManager.GetSaveData());
            yield return saveWait;
        }
    }

    [ContextMenu("Save Game")]
    public void Save(PlayerData playerData)
    {
        string saveDir = Application.persistentDataPath + "/" + folderName + "/";

        if (!Directory.Exists(saveDir))
        {
            Directory.CreateDirectory(saveDir);
        }

        string jsonData = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(saveDir + string.Concat(playerData.profileName, ".json"), jsonData);

        GUIUtility.systemCopyBuffer = saveDir + string.Concat(playerData.profileName, ".json");
    }

    [ContextMenu("Load Game")]
    public PlayerData Load()
    {
        string saveDir = Application.persistentDataPath + "/" + folderName + "/";
        string jsonData = File.ReadAllText(saveDir + string.Concat(gameManager.playerData.profileName, ".json"));
        PlayerData playerData = JsonUtility.FromJson<PlayerData>(jsonData);
        return playerData;
        //return null;
        //gameManager.LoadSaveData(saveData);
    }
}
