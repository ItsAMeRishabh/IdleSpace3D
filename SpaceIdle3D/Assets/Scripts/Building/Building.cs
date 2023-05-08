using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class Building : MonoBehaviour
{
    private float clickTimeout = 0.2f;
    private float spawnCheckDelay = 30f;

    private WaitForSeconds clickWait;
    public WaitForSeconds spawnWait;
    private Coroutine clickTimeoutCoroutine;
    private bool clickExplired = false;
    private bool clickedOnMe = false;

    public BuildingSO buildingSO;
    public BuildingData buildingData;

    public VisualEffect upgradeEffect;

    private GameManager gameManager;

    [HideInInspector] public Transform spawnPoint;

    public void Initialize()
    {
        gameManager = FindObjectOfType<GameManager>();
        upgradeEffect = GetComponent<VisualEffect>();

        clickWait = new WaitForSeconds(clickTimeout);
        spawnWait = new WaitForSeconds(spawnCheckDelay);

        StartCoroutine(SpawnCheckCoroutine());
    }

    public void SpawnUpgradeEffect(VisualEffectAsset vfxAsset)
    {
        upgradeEffect.visualEffectAsset = vfxAsset;
        upgradeEffect.Play();
    }

    public IEnumerator SpawnCheckCoroutine()
    {
        while (true)
        {
            yield return spawnWait;

            CheckSpawnConditions();
        }
    }

    public void CheckSpawnConditions()
    {
        if (buildingData.building_Level == 0) return;

        switch (buildingData.building_Name)
        {
            case "TERRA MINE": CheckTerraMineSpawn(); break;
            case "LASERVATORY": CheckLaservatorySpawn(); break;
            case "MEGATRON DOCK": CheckMegatronSpawn(); break;
            case "VAC3000": CheckBlackHoleSpawn(); break;
            case "BOBO'S HUT": CheckBoboSpawn(); break;
            default: Debug.LogError($"Unknown Building {buildingData.building_Name}!"); break;
        }
    }

    public void CheckTerraMineSpawn()
    {
        spawnPoint = transform.parent.GetChild(transform.parent.childCount - 1);

        int troopIndex = UnityEngine.Random.Range(0, buildingData.building_OwnedTroops.Count);

        if (buildingData.building_OwnedTroops[troopIndex].troops_Owned <= 0) return;

        int spawnCount = UnityEngine.Random.Range(1, buildingData.building_OwnedTroops[troopIndex].troops_Owned + 1);

        if (buildingData.building_OwnedTroops[troopIndex].troop_Prefab == null) return;

        List<Transform> list = new List<Transform>();
        Transform waypoints = GameObject.Find("TerramineWaypoints").transform;

        for (int i = 0; i < waypoints.childCount; i++)
        {
            list.Add(waypoints.GetChild(i));
        }

        StartCoroutine(SpawnPrefabs(spawnCount, troopIndex, list));
    }

    public void CheckLaservatorySpawn()
    {
        if (buildingData.building_OwnedTroops[buildingData.building_Level - 1].troops_Owned > 0)
            transform.parent.GetChild(transform.parent.childCount - 1).gameObject.SetActive(true);
    }

    public void CheckMegatronSpawn()
    {
        Transform spawnPoint = GameObject.Find("MegatronSpawn").transform;

        int index = buildingData.building_OwnedTroops.Count - 1;
        while(index > 0) 
        {
            if (buildingData.building_OwnedTroops[index].troops_Owned > 0) break;
        }

        if(spawnPoint.childCount > 0)
            Destroy(spawnPoint.GetChild(0).gameObject);

        Instantiate(buildingData.building_OwnedTroops[index].troop_Prefab, spawnPoint);
    }

    public void CheckBlackHoleSpawn()
    {
        if (buildingData.building_OwnedTroops[0].troops_Owned > 0)
            transform.parent.GetChild(transform.parent.childCount - 1).gameObject.SetActive(true);
    }

    public void CheckBoboSpawn()
    {
        spawnPoint = transform.parent.GetChild(transform.parent.childCount - 1);

        if (buildingData.building_OwnedTroops[0].troops_Owned <= 0) return;

        int spawnCount = UnityEngine.Random.Range(1, buildingData.building_OwnedTroops[0].troops_Owned + 1);

        if (buildingData.building_OwnedTroops[0].troop_Prefab == null) return;

        List<Transform> list = new List<Transform>();
        Transform waypoints = GameObject.Find("BoboWaypoints").transform;

        for (int i = 0; i < waypoints.childCount; i++)
        {
            list.Add(waypoints.GetChild(i));
        }

        StartCoroutine(SpawnPrefabs(spawnCount, 0, list));
    }

    public IEnumerator SpawnPrefabs(int spawnCount, int troopIndex, List<Transform> list)
    {
        while(spawnCount > 0)
        {
            float delay = UnityEngine.Random.Range(1f, 2f);
            yield return new WaitForSeconds(delay);
            GameObject spawnedTroop = Instantiate(buildingData.building_OwnedTroops[troopIndex].troop_Prefab, spawnPoint);
            spawnedTroop.GetComponent<TroopMovement>().StartMove(list, ActionAfterReach.DeleteSelf);
            spawnCount--;
        }
    }

    void OnMouseDown()
    {
        if (!DetectClickOnUI.IsPointerOverUIElement())
        {
            if (clickTimeoutCoroutine != null)
            {
                StopCoroutine(clickTimeoutCoroutine);
                clickTimeoutCoroutine = null;
            }

            clickTimeoutCoroutine = StartCoroutine(ClickTimeoutCoroutine());
            clickedOnMe = true;
        }
    }

    private void OnMouseUp()
    {
        if (!clickExplired && clickedOnMe)
        {
            gameManager.ClickedOnBuilding(this);
        }
        clickedOnMe = false;
    }

    public double GetIridiumPerTick()
    {
        double x = 0;
        foreach (Troop troop in buildingData.building_OwnedTroops)
        {
            x += troop.GetIridiumPerTick() * Math.Pow(buildingSO.building_IridiumBoostPerLevel, buildingData.building_Level - 1);
        }
        return x;
    }

    public double GetDarkElixirPerTick()
    {
        if (buildingData.building_Level == 0) return 0;
        else return buildingSO.building_DarkElixirPerSecond * Math.Pow(buildingSO.building_DarkElixirBoostPerLevel, buildingData.building_Level - 1);
    }

    private IEnumerator ClickTimeoutCoroutine()
    {
        clickExplired = false;

        yield return clickWait;

        clickExplired = true;
    }
}
