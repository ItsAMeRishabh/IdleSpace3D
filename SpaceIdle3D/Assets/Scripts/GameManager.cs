using System.Collections.Generic;
using UnityEngine.InputSystem;
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

    [Header("Default Values SO")]
    [SerializeField] private DefaultValues defaultValues;

    [Header("Save Configuration")]
    [SerializeField] private bool startFreshOnLaunch = false;
    [SerializeField] private float saveInterval = 5f;
    [SerializeField] private bool autoSave = false;

    private bool gameHathStarted = false;

    private Coroutine tickCoroutine;
    private Coroutine saveCoroutine;
    private Coroutine holdFarmCoroutine;
    private WaitForSeconds tickWait;
    private WaitForSeconds saveWait;
    private WaitForSeconds holdFarmWait;

    private BuildingManager buildingManager;
    private LoadSaveSystem loadSaveSystem;
    private DataProcessor dataProcessor;
    private BoostManager boostManager;
    private InputManager inputManager;
    private UIManager uiManager;

    [HideInInspector] public bool getIridiumButtonPressedDown = false;
    [HideInInspector] public bool canGetIridium = true;

    public BuildingManager BuildingManager => buildingManager;
    public LoadSaveSystem LoadSaveSystem => loadSaveSystem;
    public BoostManager BoostManager => boostManager;
    public UIManager UIManager => uiManager;

    #region Utility Functions

    private void Awake()
    {
        buildingManager = GetComponent<BuildingManager>();
        loadSaveSystem = GetComponent<LoadSaveSystem>();
        boostManager = GetComponent<BoostManager>();
        uiManager = GetComponent<UIManager>();

        inputManager = new InputManager();
        dataProcessor = new DataProcessor();
    }

    private void OnEnable()
    {
        inputManager.Enable();

        inputManager.Input.Farm.started += GetIridiumButton;
        inputManager.Input.Farm.performed += GetIridiumButton;
        inputManager.Input.Farm.canceled += GetIridiumButton;
    }

    private void Start()
    {
        uiManager.InitializeUI();

        if (startFreshOnLaunch)
        {
            uiManager.OpenProfileNamePanel();
        }
        else
        {
            CheckForSaves();
        }
    }

    private void OnDestroy()
    {
        if(autoSave) SaveGame();
    }

    private void StartGame()
    {

        uiManager.OpenMainUI();

        uiManager.CloseProfileUI();

        UpdateIridiumSources(); //Update the iridium per second

        CalculateCosts(); //Calculate all upgrade costs

        uiManager.UpdateAllUI(); //Update all UI

        StartTickCoroutine(); //Setup the coroutine for the tick rate

        StartSaveCoroutine(); //Setup the coroutine for the save
    }

    private void CheckForSaves()
    {
        List<string> profiles = loadSaveSystem.GetProfilesList();

        if (profiles.Count == 0)
        {
            Debug.Log("No saves found. Starting new game");
            uiManager.OpenProfileNamePanel();
        }
        else
        {
            uiManager.PopulateProfileSelectUI(profiles);
        }
    }

    public void StartNewGame(string profileName)
    {
        playerData = new PlayerData();
        playerData.profileName = profileName;
        playerData.iridium_Total = defaultValues.iridium_Total;
        playerData.iridium_PerSecond = defaultValues.iridium_PerSecond;
        playerData.darkElixir_Total = defaultValues.darkElixir_Total;
        playerData.darkElixir_PerSecond = defaultValues.darkElixir_PerSecond;
        playerData.iridium_PerClickLevel = defaultValues.iridium_PerClickLevel;
        playerData.iridium_PerClick = defaultValues.iridium_PerClick;
        playerData.iridium_PerClickRate = defaultValues.iridium_PerClickRate;
        playerData.ownedBuildings = defaultValues.ownedBuildings;
        playerData.activeBoosts = defaultValues.activeBoosts;

        StartGame();
    }

    private void StartTickCoroutine()
    {
        if (tickCoroutine != null)
        {
            StopCoroutine(tickCoroutine);
        }

        tickWait = new WaitForSeconds(1.0f / ticksPerSecond);
        tickCoroutine = StartCoroutine(Tick());
        gameHathStarted = true;
    }

    private void StartSaveCoroutine()
    {
        if (saveCoroutine != null)
            StopCoroutine(saveCoroutine);

        saveWait = new WaitForSeconds(saveInterval);
        saveCoroutine = StartCoroutine(SaveCoroutine());
    }

    public void UpdateIridiumSources()
    {
        playerData.iridium_PerSecond = 0;

        foreach (Building b in buildingManager.ownedBuildings)
        {
            playerData.iridium_PerSecond += b.GetIridiumPerTick() * ticksPerSecond;
        }

        playerData.iridium_PerClickBoosted = playerData.iridium_PerClick;
        playerData.iridium_PerSecondBoosted = playerData.iridium_PerSecond;

        foreach (Boost b in boostManager.activeBoosts)
        {
            playerData.iridium_PerClickBoosted *= b.boost_IridiumPerClick;
            playerData.iridium_PerSecondBoosted *= b.boost_IridiumPerSecond;
        }

        playerData.iridium_PerClick = Math.Max(1, playerData.iridium_PerSecondBoosted * playerData.iridium_PerClickLevel / 100f);
    }

    public void CalculateCosts()
    {
        upgradeClick_CurrentCost = (upgradeClick_BaseCost * Math.Pow(upgradeClick_PriceMultiplier, playerData.iridium_PerClickLevel - 1));
        holdFarmWait = new WaitForSeconds(1 / (float)playerData.iridium_PerClickRate);

        foreach (Building b in buildingManager.ownedBuildings)
        {
            foreach (Troop t in b.buildingData.building_OwnedTroops)
            {
                t.troop_CurrentCost = (t.troop_BaseCost * Math.Pow(t.troop_CostMultiplier, t.troops_Owned));
            }

            b.buildingSO.building_CurrentUpgradeCost = (b.buildingSO.building_UpgradeCosts[b.buildingData.building_Level - 1]);
        }

        foreach (BuildingLocation bl in buildingManager.buildingLocations)
        {
            bl.buildingSO.building_CurrentCost = (bl.buildingSO.building_BaseCost * Math.Pow(bl.buildingSO.building_CostMultiplier, buildingManager.GetBuildingCount(bl.buildingSO.building_Name)));
        }
    }

    #endregion

    private IEnumerator Tick()
    {
        while (true)
        {
            ProcessIridiumAdded();
            ProcessDarkElixirAdded();
            uiManager.UpdateAllUI();
            boostManager.ProcessBoostTimers();
            yield return tickWait;
        }
    }

    private IEnumerator SaveCoroutine()
    {
        while (true)
        {
            yield return saveWait;
            if (autoSave) SaveGame();
        }
    }

    private IEnumerator HoldFarmCoroutine()
    {
        yield return holdFarmWait;

        canGetIridium = true;
    }

    private void ProcessIridiumAdded()
    {
        ProcessIridiumPerBuilding();

        if (getIridiumButtonPressedDown)
        {
            if(canGetIridium)
            {
                canGetIridium = false;
                if(holdFarmCoroutine != null)
                {
                    StopCoroutine(holdFarmCoroutine);
                    holdFarmCoroutine = null;
                }

                ProcessClickedIridium();

                holdFarmCoroutine = StartCoroutine(HoldFarmCoroutine());
            }
        }
    }

    private void ProcessDarkElixirAdded()
    {
        playerData.darkElixir_Total += playerData.darkElixir_PerSecond / ticksPerSecond;
    }

    #region Iridium Processors

    private void ProcessClickedIridium()
    {
        playerData.iridium_Total += playerData.iridium_PerClickBoosted;
    }

    private void ProcessIridiumPerBuilding()
    {
        playerData.iridium_Total += playerData.iridium_PerSecondBoosted / ticksPerSecond;
    }

    #endregion

    #region Button Callbacks

    public void GetIridiumButton(InputAction.CallbackContext context)
    {
        getIridiumButtonPressedDown = context.ReadValueAsButton();

        if (!getIridiumButtonPressedDown)
        {
            canGetIridium = true;
        }
    }

    public void UpgradeClickClicked()
    {
        if (playerData.iridium_Total >= upgradeClick_CurrentCost)
        {
            playerData.iridium_Total -= upgradeClick_CurrentCost;
            upgradeClick_CurrentCost = (int)(upgradeClick_CurrentCost * upgradeClick_PriceMultiplier);
            playerData.iridium_PerClickLevel += 1;
        }
        UpdateIridiumSources();
    }

    public void TroopBuyClicked(int troopIndex)
    {
        if (buildingManager.selectedBuilding == null)
        {
            Debug.LogError("No building selected, cannot buy troop.");
        }
        else
        {
            if (playerData.iridium_Total >= buildingManager.selectedBuilding.buildingData.building_OwnedTroops[troopIndex].troop_CurrentCost)
            {
                playerData.iridium_Total -= buildingManager.selectedBuilding.buildingData.building_OwnedTroops[troopIndex].troop_CurrentCost;
                buildingManager.selectedBuilding.buildingData.building_OwnedTroops[troopIndex].troops_Owned += 1;
                buildingManager.selectedBuilding.buildingData.building_OwnedTroops[troopIndex].troop_CurrentCost = (int)(buildingManager.selectedBuilding.buildingData.building_OwnedTroops[troopIndex].troop_CurrentCost * buildingManager.selectedBuilding.buildingData.building_OwnedTroops[troopIndex].troop_CostMultiplier);
            }
        }

        UpdateIridiumSources();
        CalculateCosts();
    }


    public void BuyBuildingClicked(BuildingSO buildingSO)
    {
        if (playerData.iridium_Total >= buildingSO.building_CurrentCost)
        {
            double buildingPrice = buildingSO.building_CurrentCost;
            bool buildingPlacementSuccessful = buildingManager.PlaceBuilding(buildingSO);

            if (buildingPlacementSuccessful)
            {
                playerData.iridium_Total -= buildingPrice;
            }
        }
    }
    public void ClickedOnBuilding(Building building)
    {
        if (building == null) return;

        if (building == buildingManager.selectedBuilding) return;

        buildingManager.selectedBuilding = building;
        uiManager.OpenBuildingMenu();
    }

    #endregion

    #region Save, Load and Reset

    [ContextMenu("Try Save!")]
    public void SaveGame()
    {
        if (!gameHathStarted)
            return;

        playerData.lastSaveTime = DateTime.Now;
        playerData.ownedBuildings = buildingManager.GetBuildingDataList();
        playerData.activeBoosts = boostManager.GetActiveBoosts();
        loadSaveSystem.Save(playerData);
    }

    public PlayerData GetSaveData()
    {
        return playerData;
    }

    [ContextMenu("Try Load!")]
    public void LoadGame()
    {
        uiManager.CloseAllPanels();

        playerData = loadSaveSystem.Load();
        buildingManager.SpawnBuildings(playerData.ownedBuildings);
        boostManager.LoadBoosts(playerData.activeBoosts);

        StartGame();
    }

    public void LoadGame(string profileName)
    {
        uiManager.CloseAllPanels();

        playerData = loadSaveSystem.LoadProfile(profileName);
        buildingManager.SpawnBuildings(playerData.ownedBuildings);
        boostManager.LoadBoosts(playerData.activeBoosts);

        StartGame();
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

}