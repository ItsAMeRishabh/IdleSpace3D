using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

[RequireComponent(typeof(UIManager))]
[RequireComponent(typeof(BoostManager))]
[RequireComponent(typeof(LoadSaveSystem))]
[RequireComponent(typeof(BuildingManager))]
public class GameManager : MonoBehaviour
{
    [Header("Tick Rate")]
    public static int ticksPerSecond = 30;

    [Header("Player Data")]
    public PlayerData playerData;


    [Header("Balancing")]
    [SerializeField] private double upgradeClick_BaseCost = 1000;
    [SerializeField] private double upgradeClick_PriceMultiplier = 1.2;
    [HideInInspector] public double upgradeClick_CurrentCost = 1000;
    [SerializeField] private double iridium_PerSecondBoostMultiplier = 2;
    [SerializeField] private double iridium_PerSecondBoostDuration = 10;

    [Header("Default Values SO")]
    [SerializeField] private DefaultValues defaultValues;

    [Header("Save Configuration")]
    [SerializeField] private float saveInterval = 5f;

    private bool iridium_PerSecondBoosted = false;
    private bool getIridium_ButtonClicked = false;

    private Coroutine tickCoroutine;
    private Coroutine boostCoroutine;
    private Coroutine saveCoroutine;
    private WaitForSeconds tickWait;
    private WaitForSeconds saveWait;

    private BuildingManager buildingManager;
    private LoadSaveSystem loadSaveSystem;
    private BoostManager boostManager;
    private UIManager uiManager;

    public BuildingManager BuildingManager => buildingManager;
    public UIManager UIManager => uiManager;

    #region Utility Functions

    private void Awake()
    {
        buildingManager = GetComponent<BuildingManager>();
        loadSaveSystem = GetComponent<LoadSaveSystem>();
        boostManager = GetComponent<BoostManager>();
        uiManager = GetComponent<UIManager>();
    }

    private void Start()
    {
        //CheckForSaves();
        StartGame();
    }

    private void OnDestroy()
    {
        //SaveGame();
    }

    private void StartGame()
    {

        //buildingManager.ownedBuildings = new List<Building>(FindObjectsOfType<Building>());

        UpdateIridiumPerSecond(); //Update the iridium per second

        uiManager.InitializeUI(); //Initialize all UI Variables

        CalculateCosts(); //Calculate all upgrade costs

        uiManager.UpdateAllUI(); //Update all UI

        StartTickCoroutine(); //Setup the coroutine for the tick rate

        //StartSaveCoroutine(); //Setup the coroutine for the save
    }

    private void CheckForSaves()
    {
        List<string> profiles = loadSaveSystem.GetProfilesList();

        if (profiles.Count == 0)
        {
            StartNewGame();
        }
        else
        {
            //Open UI to choose profile
        }
    }

    public void StartNewGame()
    {
        playerData = new PlayerData();
        playerData.iridium_Total = defaultValues.iridium_Total;
        playerData.iridium_PerClickLevel = defaultValues.iridium_PerClickLevel;
        playerData.iridium_PerSecond = defaultValues.iridium_PerSecond;
        playerData.iridium_PerClick = defaultValues.iridium_PerClick;
        playerData.ownedBuildings = defaultValues.ownedBuildings;

        StartGame();
    }

    private void StartTickCoroutine()
    {
        if (tickCoroutine != null)
            StopCoroutine(tickCoroutine);

        tickWait = new WaitForSeconds(1.0f / ticksPerSecond);
        tickCoroutine = StartCoroutine(Tick());
    }

    public void UpdateIridiumPerSecond()
    {
        playerData.iridium_PerSecond = 0;

        foreach (Building b in buildingManager.ownedBuildings)
        {
            playerData.iridium_PerSecond += b.GetIridiumPerTick() * ticksPerSecond;
        }

        playerData.iridium_PerSecond = iridium_PerSecondBoosted ? playerData.iridium_PerSecond * iridium_PerSecondBoostMultiplier : playerData.iridium_PerSecond;
        playerData.iridium_PerClick = Math.Max(1, playerData.iridium_PerSecond * playerData.iridium_PerClickLevel / 100f);
    }

    public void CalculateCosts()
    {
        upgradeClick_CurrentCost = (int)(upgradeClick_BaseCost * Math.Pow(upgradeClick_PriceMultiplier, playerData.iridium_PerClickLevel - 1));
        playerData.iridium_PerClick = Math.Max(1, playerData.iridium_PerSecond * playerData.iridium_PerClickLevel / 100f);

        foreach (Building b in buildingManager.ownedBuildings)
        {
            foreach (Troop t in b.buildingData.ownedTroops)
            {
                t.troop_CurrentCost = (int)(t.troop_BaseCost * Math.Pow(t.troop_CostMultiplier, t.troops_Owned));
            }

            b.buildingData.building_UpgradeCost = (int)(b.buildingSO.building_UpgradeBaseCost * Mathf.Pow((float)b.buildingSO.building_UpgradeCostMultiplier, b.buildingData.building_Level - 1));
        }

        foreach (BuildingLocation bl in buildingManager.buildingLocations)
        {
            bl.buildingSO.building_CurrentCost = (int)(bl.buildingSO.building_BaseCost * Mathf.Pow((float)bl.buildingSO.building_CostMultiplier, buildingManager.GetBuildingCount(bl.buildingSO.building_Name)));
        }
        //TODO : Calculate building costs
    }

    private IEnumerator IridiumPerSecondBoost()
    {
        iridium_PerSecondBoosted = true;
        UpdateIridiumPerSecond();

        yield return new WaitForSeconds((float)iridium_PerSecondBoostDuration);

        iridium_PerSecondBoosted = false;
        UpdateIridiumPerSecond();
    }

    #endregion

    private IEnumerator Tick()
    {
        while (true)
        {
            ProcessIridiumAdded();
            uiManager.UpdateAllUI();
            yield return tickWait;
        }
    }

    private void ProcessIridiumAdded()
    {
        ProcessIridiumPerBuilding();

        if (getIridium_ButtonClicked)
        {
            ProcessClickedIridium();
            getIridium_ButtonClicked = false;
        }
    }

    #region Iridium Processors

    private void ProcessClickedIridium()
    {
        double iridiumToAdd = 0;
        playerData.iridium_PerClick = Math.Max(1, playerData.iridium_PerSecond * playerData.iridium_PerClickLevel / 100f);
        iridiumToAdd += playerData.iridium_PerClick;
        foreach (Boost b in playerData.boosts)
        {
            if (b.boost_IsActive)
            {
                iridiumToAdd *= b.boost_IridiumPerClick;
            }
        }
        playerData.iridium_Total += playerData.iridium_PerClick;
    }

    private void ProcessIridiumPerBuilding()
    {
        double iridiumToAdd = 0;
        iridiumToAdd += playerData.iridium_PerSecond;
        foreach (Boost b in playerData.boosts)
        {
            if (b.boost_IsActive)
            {
                iridiumToAdd *= b.boost_IridiumPerSecond;
            }
        }
        playerData.iridium_Total += playerData.iridium_PerSecond / ticksPerSecond;
    }

    #endregion

    #region Button Callbacks
    public void GetIridiumClicked()
    {
        getIridium_ButtonClicked = true;
    }

    public void UpgradeClickClicked()
    {
        if (playerData.iridium_Total >= upgradeClick_CurrentCost)
        {
            playerData.iridium_Total -= upgradeClick_CurrentCost;
            upgradeClick_CurrentCost = (int)(upgradeClick_CurrentCost * upgradeClick_PriceMultiplier);
            playerData.iridium_PerClickLevel += 1;
            playerData.iridium_PerClick = Math.Max(1, playerData.iridium_PerSecond * playerData.iridium_PerClickLevel / 100f);
        }
    }

    public void TroopBuyClicked(int troopIndex)
    {
        if (buildingManager.selectedBuilding == null)
        {
            Debug.LogError("No building selected, cannot buy troop.");
        }
        else
        {
            if (playerData.iridium_Total >= buildingManager.selectedBuilding.buildingData.ownedTroops[troopIndex].troop_CurrentCost)
            {
                playerData.iridium_Total -= buildingManager.selectedBuilding.buildingData.ownedTroops[troopIndex].troop_CurrentCost;
                buildingManager.selectedBuilding.buildingData.ownedTroops[troopIndex].troops_Owned += 1;
                buildingManager.selectedBuilding.buildingData.ownedTroops[troopIndex].troop_CurrentCost = (int)(buildingManager.selectedBuilding.buildingData.ownedTroops[troopIndex].troop_CurrentCost * buildingManager.selectedBuilding.buildingData.ownedTroops[troopIndex].troop_CostMultiplier);
            }
        }

        UpdateIridiumPerSecond();
        CalculateCosts();
    }

    public void BoostIridiumProductionClicked()
    {
        if (boostCoroutine != null)
        {
            StopCoroutine(boostCoroutine);
            boostCoroutine = null;
        }

        boostCoroutine = StartCoroutine(IridiumPerSecondBoost());
    }

    public void BuyBuildingClicked(BuildingSO buildingSO)
    {
        if (playerData.iridium_Total >= buildingSO.building_BaseCost)
        {
            playerData.iridium_Total -= buildingSO.building_BaseCost;
            buildingManager.PlaceBuilding(buildingSO);
        }
    }

    #endregion

    #region Save, Load and Reset

    [ContextMenu("Try Save!")]
    public void SaveGame()
    {
        playerData.ownedBuildings = buildingManager.GetBuildingDatas();
        loadSaveSystem.Save(playerData);
        playerData.ownedBuildings.Clear();
    }

    public PlayerData GetSaveData()
    {
        return playerData;
    }

    [ContextMenu("Try Load!")]
    public void LoadGame()
    {
        playerData = loadSaveSystem.Load();
        buildingManager.SpawnBuildings(playerData.ownedBuildings);
        //UpdateIridiumPerSecond();
        StartGame();
        //StartTickCoroutine();
    }

    void ResetGame()
    {
        playerData.iridium_Total = defaultValues.iridium_Total;
        playerData.iridium_PerSecond = defaultValues.iridium_PerSecond;
        playerData.iridium_PerClick = defaultValues.iridium_PerClick;
        playerData.iridium_PerClickLevel = defaultValues.iridium_PerClickLevel;
        playerData.ownedBuildings = defaultValues.ownedBuildings;
    }

    #endregion

    public void ClickedOnBuilding(Building building)
    {
        if (building == null) return;

        if (building == buildingManager.selectedBuilding) return;

        buildingManager.selectedBuilding = building;
        uiManager.ClickedOnBuilding();
    }
}