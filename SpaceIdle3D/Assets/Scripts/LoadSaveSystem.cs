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

    void Start()
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
    public void Save(SaveData saveData)
    {
        string saveDir = Application.persistentDataPath + "/" + folderName + "/";

        if (!Directory.Exists(saveDir))
        {
            Directory.CreateDirectory(saveDir);
        }

        string jsonData = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveDir + string.Concat(GameManager.profileName, ".json"), jsonData);

        GUIUtility.systemCopyBuffer = saveDir + string.Concat(GameManager.profileName, ".json");
    }

    [ContextMenu("Load Game")]
    public SaveData Load()
    {
        string saveDir = Application.persistentDataPath + "/" + folderName + "/";
        string jsonData = File.ReadAllText(saveDir + string.Concat(GameManager.profileName, ".json"));
        SaveData saveData = JsonUtility.FromJson<SaveData>(jsonData);
        return saveData;
        //return null;
        //gameManager.LoadSaveData(saveData);
    }
}
