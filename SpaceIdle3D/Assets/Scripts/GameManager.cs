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

    [Header("Iridium Truck")]
    [SerializeField] private Transform truckRouteParent;
    [SerializeField] private GameObject truckPrefab;
    [SerializeField] private Vector2 truckSpawnDelay;
    [SerializeField] private double truckSpawn_MinimumIPS = 50;
    public float truckMoveSpeed = 5;
    public float truckRotationSpeed = 5;
    [HideInInspector] public List<Transform> truckRoute;
    [HideInInspector] public GameObject iridiumTruck;

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
    private StockManager stockManager;
    private BoostManager boostManager;
    private InputManager inputManager;
    private UIManager uiManager;

    [HideInInspector] public bool getIridiumButtonPressedDown = false;
    [HideInInspector] public bool canGetIridium = true;

    public EnemyShipManager EnemyShipManagerRef => enemyShipManager;
    public BuildingManager BuildingManagerRef => buildingManager;
    public LoadSaveSystem LoadSaveSystemRef => loadSaveSystem;
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

        WakeAllManagers();
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
            loadSaveSystem.autoSave = false;
            StartNewGame("Default");
        }
        else
        {
            loadSaveSystem.autoSave = true;
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

    public void WakeAllManagers()
    {
        enemyShipManager.WakeUp();
        buildingManager.WakeUp();
        loadSaveSystem.WakeUp();
        boostManager.WakeUp();
        stockManager.WakeUp();
        uiManager.WakeUp();
    }

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
        gameHathStarted = true;

        GrabTruckRoute();

        StartAllManagers();

        ResourcesGainedAfterIdle();

        UpdateResourceSources(); //Update the iridium per second

        UpdateCosts(); //Calculate all upgrade costs

        StartTickCoroutine(); //Setup the coroutine for the tick rate

        StartSaveCoroutine(); //Setup the coroutine for the save
    }

    private void StartAllManagers()
    {
        boostManager.StartGame();

        stockManager.StartGame();

        buildingManager.StartGame();

        enemyShipManager.StartGame();

        uiManager.StartGame();
    }

    public void StartNewGame(string profileName)
    {
        gameHathStarted = true;

        playerData = new PlayerData();
        playerData.profileName = profileName;
        playerData.profileCreateTime = DateTime.Now;
        playerData.maxIdleTime = defaultValues.maxIdleTime;
        playerData.iridium_Total = defaultValues.iridium_Total;
        playerData.iridium_Current = defaultValues.iridium_Current;
        playerData.iridium_PerSecond = defaultValues.iridium_PerSecond;
        playerData.darkElixir_Total = defaultValues.darkElixir_Total;
        playerData.darkElixir_Current = defaultValues.darkElixir_Current;
        playerData.darkElixir_PerSecond = defaultValues.darkElixir_PerSecond;
        playerData.iridium_PerClickLevel = defaultValues.iridium_PerClickLevel;
        playerData.iridium_PerClick = defaultValues.iridium_PerClick;
        playerData.iridium_PerClickRate = defaultValues.iridium_PerClickRate;

        if (loadSaveSystem.autoSave) SaveGame();

        StartGame();
    }

    private void GrabTruckRoute()
    {
        for (int i = 0; i < truckRouteParent.childCount; i++)
        {
            int j = i;
            truckRoute.Add(truckRouteParent.GetChild(j));
        }
    }

    private void StartTickCoroutine()
    {
        if (tickCoroutine != null)
        {
            StopCoroutine(tickCoroutine);
        }

        tickWait = new WaitForSeconds(1.0f / ticksPerSecond);
        tickCoroutine = StartCoroutine(Tick());
    }

    private IEnumerator Tick()
    {
        while (true)
        {
            ProcessResourcesAdded();

            boostManager.ProcessBoostTimers();
            stockManager.TickStockRefresh();
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
        UpdateIridiumSources();
        UpdateDarkElixirSources();
    }

    public void UpdateIridiumSources()
    {
        playerData.iridium_PerSecond = GetBaseIridiumPerSecond();
        playerData.iridium_PerSecondBoost = GetIridiumPerSecondBoost();

        playerData.iridium_PerClick = GetBaseIridiumPerClick();
        playerData.iridium_PerClickBoost = GetIridiumPerClickBoost();

        if (iridiumTruck == null && playerData.iridium_PerSecond >= truckSpawn_MinimumIPS)
            StartCoroutine(SpawnTruck());
            
    }

    public void UpdateDarkElixirSources()
    {
        playerData.darkElixir_PerSecond = GetBaseDarkElixirPerSecond();
        playerData.darkElixir_PerSecondBoost = GetDarkElixirPerSecondBoost();
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
        playerData.darkElixir_Current += (playerData.darkElixir_PerSecond * playerData.darkElixir_PerSecondBoost) / ticksPerSecond;
        playerData.darkElixir_Total += (playerData.darkElixir_PerSecond * playerData.darkElixir_PerSecondBoost) / ticksPerSecond;
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

                playerData.iridium_Total += (playerData.iridium_PerClick * playerData.iridium_PerClickBoost);
                playerData.iridium_Current += (playerData.iridium_PerClick * playerData.iridium_PerClickBoost);

                holdFarmWait = new WaitForSeconds(1 / (float)playerData.iridium_PerClickRate);
                holdFarmCoroutine = StartCoroutine(HoldFarmCoroutine());
            }
        }
    }

    private void ProcessIridiumPerBuilding()
    {
        playerData.iridium_Total += (playerData.iridium_PerSecond * playerData.iridium_PerClickBoost) / ticksPerSecond;
        playerData.iridium_Current += (playerData.iridium_PerSecond * playerData.iridium_PerClickBoost) / ticksPerSecond;
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
        buildingManager.TroopBuyClicked(troopIndex);
    }

    public void TroopUpgradeClicked(int troopIndex)
    {
        buildingManager.TroopUpgradeClicked(troopIndex);
    }

    public void BuyBuildingClicked(BuildingSO buildingSO)
    {
        buildingManager.BuildingBuyClicked(buildingSO);
    }

    public void ClickedOnBuilding(Building building)
    {
        buildingManager.ClickedOnBuilding(building);
    }

    #endregion

    public IEnumerator SpawnTruck()
    {
        float delay = UnityEngine.Random.Range(truckSpawnDelay.x, truckSpawnDelay.y);

        yield return new WaitForSeconds(delay);

        iridiumTruck = Instantiate(truckPrefab, truckRouteParent);
        iridiumTruck.GetComponent<IridiumTruck>().StartMove(this);
    }
    public double GetBaseIridiumPerSecond()
    {
        double baseIPS = 0;

        foreach (Building building in buildingManager.ownedBuildings)
        {
            baseIPS += (building.GetIridiumPerTick() * ticksPerSecond);
        }

        return baseIPS;
    }

    public double GetIridiumPerSecondBoost()
    {
        double IPS_Boost = 1;

        foreach (Boost boost in boostManager.activeBoosts)
        {
            IPS_Boost *= boost.boost_IridiumPerSecond;
        }

        return IPS_Boost;
    }

    public double GetBaseIridiumPerClick()
    {
        double baseIPC = Math.Max(1, (playerData.darkElixir_PerSecond * playerData.iridium_PerSecondBoost) * playerData.iridium_PerClickLevel / 100f);

        return baseIPC;
    }

    public double GetIridiumPerClickBoost()
    {
        double IPC_Boost = 1;

        foreach (Boost boost in boostManager.activeBoosts)
        {
            IPC_Boost *= boost.boost_IridiumPerClick;
        }

        return IPC_Boost;
    }

    public double GetBaseDarkElixirPerSecond()
    {
        double baseDEPS = defaultValues.darkElixir_PerSecond;

        foreach (Building building in buildingManager.ownedBuildings)
        {
            baseDEPS += building.GetDarkElixirPerTick();
        }

        return baseDEPS;
    }

    public double GetDarkElixirPerSecondBoost()
    {
        double DEPS_Boost = 1;

        foreach (Boost boost in boostManager.activeBoosts)
        {
            DEPS_Boost *= boost.boost_DarkElixirPerSecond;
        }

        return DEPS_Boost;
    }

    public void ResourcesGainedAfterIdle()
    {
        if (loadSaveSystem.startFreshOnLaunch)
            return;

        long timeElapsed = (long)(DateTime.Now - (DateTime)playerData.lastSaveTime).TotalSeconds;
        timeElapsed = (long)Math.Min(timeElapsed, playerData.maxIdleTime);

        if (timeElapsed <= 5)
        {
            return;
        }

        double baseIPS = GetBaseIridiumPerSecond();
        double baseDEPS = GetBaseDarkElixirPerSecond();
        List<Boost> localBoostList = boostManager.GetActiveBoosts();

        long timeToProcess = timeElapsed;

        double iridiumToAdd = 0;
        double darkelixerToAdd = 0;

        while (timeToProcess > 0)
        {
            double boostedIPS = baseIPS;
            double boostedDEPS = baseDEPS;

            Boost lowestTimeBoost = null;

            for (int i = 0; i < localBoostList.Count; i++)
            {
                if (lowestTimeBoost == null)
                {
                    lowestTimeBoost = localBoostList[i];
                }
                else
                {
                    if (localBoostList[i].boost_TimeRemaining < lowestTimeBoost.boost_TimeRemaining)
                    {
                        lowestTimeBoost = localBoostList[i];
                    }
                }

                boostedIPS *= localBoostList[i].boost_IridiumPerSecond;
                boostedDEPS *= localBoostList[i].boost_DarkElixirPerSecond;
            }

            if (lowestTimeBoost != null)
            {
                if (timeToProcess > lowestTimeBoost.boost_TimeRemaining)
                {
                    iridiumToAdd += boostedIPS * lowestTimeBoost.boost_TimeRemaining;
                    darkelixerToAdd += boostedDEPS * lowestTimeBoost.boost_TimeRemaining;

                    Debug.Log($"{lowestTimeBoost.boost_Name} lasted for {lowestTimeBoost.boost_TimeRemaining} seconds");
                    timeToProcess -= (int)lowestTimeBoost.boost_TimeRemaining;

                    for (int i = 0; i < localBoostList.Count; i++)
                    {
                        localBoostList[i].boost_TimeRemaining -= lowestTimeBoost.boost_TimeRemaining;
                    }

                    localBoostList.Remove(lowestTimeBoost);

                    lowestTimeBoost = null;
                }
                else
                {
                    for (int i = 0; i < localBoostList.Count; i++)
                    {
                        localBoostList[i].boost_TimeRemaining -= timeToProcess;
                        Debug.Log($"Used up {timeToProcess} seconds of boost: {localBoostList[i].boost_Name}");
                    }

                    iridiumToAdd += boostedIPS * timeToProcess;
                    darkelixerToAdd += boostedDEPS * timeToProcess;

                    timeToProcess = 0;
                }
            }
            else
            {
                Debug.Log("No boosts active!");
                iridiumToAdd += boostedIPS * timeToProcess;
                darkelixerToAdd += boostedDEPS * timeToProcess;
                timeToProcess = 0;
            }
        }

        boostManager.LoadBoosts(localBoostList);

        Debug.Log($"{playerData.profileName} was idle for {timeElapsed} seconds");

        Debug.Log($"Iridium Added: {iridiumToAdd}");

        playerData.iridium_Current += iridiumToAdd;
        playerData.iridium_Total += iridiumToAdd;

        Debug.Log($"Dark Elixir Added: {darkelixerToAdd}");
        playerData.darkElixir_Current += darkelixerToAdd;
        playerData.darkElixir_Total += darkelixerToAdd;
    }

    [ContextMenu("Try Save!")]
    public void SaveGame()
    {
        if (!gameHathStarted)
            return;

        playerData.lastSaveTime = DateTime.Now;
        playerData.ownedBuildings = buildingManager.GetBuildingDataList();
        playerData.activeBoosts = boostManager.GetActiveBoosts();
        playerData.ownedStocks = stockManager.GetStocks();

        loadSaveSystem.Save(playerData);
    }

    public void LoadGame(string profileName)
    {
        playerData = loadSaveSystem.LoadProfile(profileName);

        StartGame();
    }
}