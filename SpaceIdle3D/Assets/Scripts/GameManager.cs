using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine;
using System;

[RequireComponent(typeof(UIManager))]
[RequireComponent(typeof(BoostManager))]
[RequireComponent(typeof(StockManager))]
[RequireComponent(typeof(LoadSaveSystem))]
[RequireComponent(typeof(BuildingManager))]
[RequireComponent(typeof(EnemyShipManager))]
public class GameManager : MonoBehaviour
{
    [Header("Tick Rate")]
    public static int ticksPerSecond = 30;

    [Header("Player Data")]
    public PlayerData playerData;


    [Header("Balancing")]
    private double upgradeClick_BaseCost = 1000;
    private double upgradeClick_PriceMultiplier = 1.2;
    [HideInInspector] public double upgradeClick_CurrentCost = 1000;

    [Header("Default Values SO")]
    [SerializeField] private DefaultValues defaultValues;

    private bool gameHathStarted = false;

    private Coroutine tickCoroutine;
    private Coroutine saveCoroutine;
    private Coroutine holdFarmCoroutine;
    private WaitForSeconds tickWait;
    private WaitForSeconds saveWait;
    private WaitForSeconds holdFarmWait;

    private EnemyShipManager enemyShipManager;
    private BuildingManager buildingManager;
    private LoadSaveSystem loadSaveSystem;
    private DataProcessor dataProcessor;
    private StockManager stockManager;
    private BoostManager boostManager;
    private InputManager inputManager;
    private UIManager uiManager;

    [HideInInspector] public bool getIridiumButtonPressedDown = false;
    [HideInInspector] public bool canGetIridium = true;

    public EnemyShipManager EnemyShipManagerRef => enemyShipManager;
    public BuildingManager BuildingManagerRef => buildingManager;
    public LoadSaveSystem LoadSaveSystemRef => loadSaveSystem;
    public DataProcessor DataProcessorRef => dataProcessor;
    public StockManager StockManagerRef => stockManager;
    public BoostManager BoostManagerRef => boostManager;
    public UIManager UIManagerRef => uiManager;

    #region Unity Functions

    private void Awake()
    {
        enemyShipManager = GetComponent<EnemyShipManager>();
        buildingManager = GetComponent<BuildingManager>();
        loadSaveSystem = GetComponent<LoadSaveSystem>();
        boostManager = GetComponent<BoostManager>();
        stockManager = GetComponent<StockManager>();
        uiManager = GetComponent<UIManager>();

        inputManager = new InputManager();
        dataProcessor = new DataProcessor(this);
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
        if (loadSaveSystem.startFreshOnLaunch)
        {
            StartNewGame("Default");
        }
        else
        {
            CheckForSaves();
        }
    }

    private void OnDisable()
    {
        inputManager.Input.Farm.started -= GetIridiumButton;
        inputManager.Input.Farm.performed -= GetIridiumButton;
        inputManager.Input.Farm.canceled -= GetIridiumButton;

        inputManager.Disable();

        if (loadSaveSystem.autoSave) SaveGame();
    }

    #endregion

    private void CheckForSaves()
    {
        List<PlayerData> profiles = loadSaveSystem.GetProfilesList();

        if (profiles.Count == 0)
        {
            Debug.Log("No saves found. Starting new game");
            uiManager.OpenProfileCreatePanel();
        }
        else
        {
            uiManager.PopulateProfileSelectUI(profiles);
        }
    }

    private void StartGame()
    {
        StartAllManagers();

        uiManager.OpenMainUI();

        uiManager.CloseProfileUI();

        UpdateResourceSources(); //Update the iridium per second

        UpdateCosts(); //Calculate all upgrade costs

        StartTickCoroutine(); //Setup the coroutine for the tick rate

        StartSaveCoroutine(); //Setup the coroutine for the save
    }

    private void StartAllManagers()
    {
        stockManager.StartGame();

        buildingManager.StartGame();

        enemyShipManager.StartGame();

        uiManager.StartGame();
    }

    public void StartNewGame(string profileName)
    {
        playerData = new PlayerData();
        playerData.profileName = profileName;
        playerData.profileCreateTime = DateTime.Now;
        playerData.maxIdleTime = defaultValues.maxIdleTime;
        playerData.iridium_Total = defaultValues.iridium_Total;
        playerData.iridium_Current = defaultValues.iridium_Current;
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

    private IEnumerator Tick()
    {
        while (true)
        {
            ProcessResourcesAdded();

            boostManager.ProcessBoostTimers();
            stockManager.TickCheckRefreshStockPrices();
            uiManager.UpdateAllUI();
            
            yield return tickWait;
        }
    }

    private void StartSaveCoroutine()
    {
        if (saveCoroutine != null)
            StopCoroutine(saveCoroutine);

        saveWait = new WaitForSeconds(loadSaveSystem.saveInterval);
        saveCoroutine = StartCoroutine(SaveCoroutine());
    }

    private IEnumerator SaveCoroutine()
    {
        while (true)
        {
            yield return saveWait;
            if (loadSaveSystem.autoSave) SaveGame();
        }
    }

    private IEnumerator HoldFarmCoroutine()
    {
        yield return holdFarmWait;

        canGetIridium = true;
    }

    public void UpdateCosts()
    {
        upgradeClick_CurrentCost = upgradeClick_BaseCost * Math.Pow(upgradeClick_PriceMultiplier, playerData.iridium_PerClickLevel - 1);

        buildingManager.UpdateCosts();
    }

    public void UpdateResourceSources()
    {
        dataProcessor.UpdateResourceSources(playerData);
    }

    private void ProcessResourcesAdded()
    {
        ProcessIridiumAdded();
        ProcessDarkElixirAdded();
    }

    private void ProcessIridiumAdded()
    {
        ProcessIridiumPerBuilding();

        ProcessClickedIridium();
    }

    private void ProcessDarkElixirAdded()
    {
        playerData.darkElixir_Total += playerData.darkElixir_PerSecondBoosted / ticksPerSecond;
    }

    #region Iridium Processors

    private void ProcessClickedIridium()
    {
        if (getIridiumButtonPressedDown)
        {
            if (canGetIridium)
            {
                canGetIridium = false;
                if (holdFarmCoroutine != null)
                {
                    StopCoroutine(holdFarmCoroutine);
                    holdFarmCoroutine = null;
                }

                playerData.iridium_Total += playerData.iridium_PerClickBoosted;
                playerData.iridium_Current += playerData.iridium_PerClickBoosted;

                holdFarmWait = new WaitForSeconds(1 / (float)playerData.iridium_PerClickRate);
                holdFarmCoroutine = StartCoroutine(HoldFarmCoroutine());
            }
        }
    }

    private void ProcessIridiumPerBuilding()
    {
        playerData.iridium_Total += playerData.iridium_PerSecondBoosted / ticksPerSecond;
        playerData.iridium_Current += playerData.iridium_PerSecondBoosted / ticksPerSecond;
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
        if (playerData.iridium_Current >= upgradeClick_CurrentCost)
        {
            playerData.iridium_Current -= upgradeClick_CurrentCost;
            upgradeClick_CurrentCost = (int)(upgradeClick_CurrentCost * upgradeClick_PriceMultiplier);
            playerData.iridium_PerClickLevel += 1;
        }

        UpdateResourceSources();
    }

    public void TroopBuyClicked(int troopIndex)
    {
        if (buildingManager.selectedBuilding == null)
        {
            Debug.LogError("No building selected, cannot buy troop.");
        }
        else
        {
            if (playerData.iridium_Current >= buildingManager.selectedBuilding.buildingData.building_OwnedTroops[troopIndex].troop_CurrentCost)
            {
                playerData.iridium_Current -= buildingManager.selectedBuilding.buildingData.building_OwnedTroops[troopIndex].troop_CurrentCost;
                buildingManager.selectedBuilding.buildingData.building_OwnedTroops[troopIndex].troops_Owned += 1;
                buildingManager.selectedBuilding.buildingData.building_OwnedTroops[troopIndex].troop_CurrentCost = (int)(buildingManager.selectedBuilding.buildingData.building_OwnedTroops[troopIndex].troop_CurrentCost * buildingManager.selectedBuilding.buildingData.building_OwnedTroops[troopIndex].troop_CostMultiplier);
            }
        }

        UpdateResourceSources();
        UpdateCosts();
    }


    public void BuyBuildingClicked(BuildingSO buildingSO)
    {
        double buildingPrice = buildingSO.building_UpgradeCosts[0];

        if (playerData.iridium_Current >= buildingPrice)
        {
            bool buildingPlacementSuccessful = buildingManager.PlaceBuilding(buildingSO);

            if (buildingPlacementSuccessful)
            {
                playerData.iridium_Current -= buildingPrice;
            }
        }
    }
    public void ClickedOnBuilding(Building building)
    {
        buildingManager.ClickedOnBuilding(building);
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

        playerData = dataProcessor.WelcomeBackPlayer(playerData);

        UpdateCosts();

        StartGame();
    }

    void ResetGame()
    {
        playerData.iridium_Total = defaultValues.iridium_Total;
        playerData.iridium_Current = defaultValues.iridium_Current;
        playerData.iridium_PerSecond = defaultValues.iridium_PerSecond;
        playerData.iridium_PerClick = defaultValues.iridium_PerClick;
        playerData.iridium_PerClickLevel = defaultValues.iridium_PerClickLevel;
        playerData.ownedBuildings = defaultValues.ownedBuildings;
    }

    #endregion

}