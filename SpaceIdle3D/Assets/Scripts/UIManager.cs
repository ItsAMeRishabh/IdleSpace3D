using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

[RequireComponent(typeof(GameManager))]
public class UIManager : MonoBehaviour
{
    [Header("Always On UI")]
    [SerializeField] private GameObject alwaysOnUI;
    [SerializeField] private TMP_Text text_TotalIridium;
    [SerializeField] private TMP_Text text_IridiumPerSecond;

    [SerializeField] private TMP_Text text_TotalDarkElixir;
    [SerializeField] private TMP_Text text_DarkElixirPerSecond;

    private List<TMP_Text> text_BuildingCosts;
    private List<Building> levelZeroBuildings;

    [Header("Main UI")]
    [SerializeField] private GameObject mainUI;
    [SerializeField] private Button button_GetIridium;
    [SerializeField] private Button button_GetBoost;
    [SerializeField] private Button button_StocksMenu;
    [SerializeField] private Button button_SellStocksMenu;
    [SerializeField] private Button button_UpgradeClick;

    private TMP_Text text_GetIridiumButton;
    private TMP_Text text_UpgradeClickButton;

    [Header("Boost UI")]
    [SerializeField] private GameObject boostUI;
    [SerializeField] private Button button_Back_Boost;
    [SerializeField] private GameObject boostButtonParent;
    [SerializeField] private GameObject boostButtonPrefab;
    private List<Button> button_Boosts;
    private List<TMP_Text> text_BoostNames;
    private List<TMP_Text> text_BoostDurations;
    private List<Image> image_BoostDurationRemainings;
    private List<TMP_Text> text_BoostDurationRemainings;

    [Header("Building UI")]
    [SerializeField] private GameObject buildingUI;
    [SerializeField] private TMP_Text text_BuildingName;
    [SerializeField] private TMP_Text text_BuildingIridiumPerSecond;
    [SerializeField] private Button button_UpgradeBuilding;
    [SerializeField] private Button button_Back_Building;
    private TMP_Text text_UpgradeBuildingButton;

    [SerializeField] private GameObject troopButtonParent;
    [SerializeField] private GameObject troopButtonPrefab;

    private List<Button> button_Troop;
    private List<TMP_Text> text_TroopNames;
    private List<TMP_Text> text_TroopCosts;
    private List<TMP_Text> text_TroopsOwned;
    private List<TMP_Text> text_TroopIPS;


    [Header("Profile Selection UI")]
    [SerializeField] private GameObject profileSelectionUI;
    [SerializeField] private Button button_newProfile;

    [SerializeField] private GameObject profileButtonParent;
    [SerializeField] private GameObject profileButtonPrefab;

    private List<Button> button_Profiles;
    private List<TMP_Text> text_ProfileNames;

    [Header("Profile Creation UI")]
    [SerializeField] private GameObject profileCreationUI;
    [SerializeField] private Button button_CreateProfile;
    [SerializeField] private TMP_InputField inputField_ProfileName;

    [Header("Stocks UI")]
    [SerializeField] private GameObject stocksUI;
    [SerializeField] private TMP_Text text_StockName;
    [SerializeField] private TMP_Text text_StockNextExpire;
    [SerializeField] private Button nextStock;
    [SerializeField] private Button prevStock;
    [SerializeField] private TMP_Text text_StockOwned;
    [SerializeField] private TMP_Text text_StockNextRefresh;
    [SerializeField] private TMP_Text text_CurrentPrice;
    [SerializeField] private TMP_Text text_AmountToBuy;
    [SerializeField] private Button button_BuyStocks;
    private TMP_Text text_BuyStocksButton;
    [SerializeField] private Button button_Prev;
    [SerializeField] private Button button_MegaPrev;
    [SerializeField] private Button button_Next;
    [SerializeField] private Button button_MegaNext;
    [SerializeField] private TMP_Text text_TotalPrice;
    [SerializeField] private Button button_StocksBack;

    private GameManager gameManager;

    public void WakeUp()
    {
        gameManager = GetComponent<GameManager>();

        InitializeAllUI();
    }

    public void InitializeAllUI()
    {
        InitializeMainUI();

        InitializeBuildingUI();

        InitializeBoostUI();

        InitializeStocksUI();

        InitializeProfileSelectUI();

        InitializeProfileCreateUI();
    }

    public void StartGame()
    {
        CloseAllPanels();

        OpenAlwaysOnUI();
    }

    public void UpdateAllUI()
    {
        UpdateAlwaysOnUI();

        UpdateMainUI();

        UpdateBuildingUI();

        UpdateBoostUI();

        UpdateStocksMenu();
    }

    public void CloseAllPanels()
    {
        CloseMainUI();
        CloseBoostMenu();
        CloseBuildingMenu();
        CloseProfileUI();
        CloseProfileCreatePanel();
    }

    #region Always On UI

    public void OpenAlwaysOnUI()
    {
        alwaysOnUI.SetActive(true);

        UpdateLevelZeroBuildingList();
    }

    public void UpdateLevelZeroBuildingList()
    {
        text_BuildingCosts = new List<TMP_Text>();
        levelZeroBuildings = new List<Building>();

        foreach (Building building in gameManager.BuildingManagerRef.ownedBuildings)
        {
            if (building.buildingData.building_Level != 0)
                continue;

            levelZeroBuildings.Add(building);
            text_BuildingCosts.Add(building.transform.parent.GetChild(2).GetChild(1).GetComponent<TMP_Text>());
        }
    }

    public void UpdateAlwaysOnUI()
    {
        text_TotalIridium.text = NumberFormatter.FormatNumber(gameManager.playerData.iridium_Current, FormattingTypes.Iridium);
        text_IridiumPerSecond.text = NumberFormatter.FormatNumber((gameManager.playerData.iridium_PerSecond * gameManager.playerData.iridium_PerSecondBoost), FormattingTypes.IridiumPerSecond) + " /s";

        text_TotalDarkElixir.text = NumberFormatter.FormatNumber(gameManager.playerData.darkElixir_Total, FormattingTypes.DarkElixer);
        text_DarkElixirPerSecond.text = NumberFormatter.FormatNumber(gameManager.playerData.darkElixir_PerSecond, FormattingTypes.DarkElixer);

        UpdateBuildingCosts();
    }

    private void UpdateBuildingCosts()
    {
        if (levelZeroBuildings.Count != 0)
        {
            for (int i = 0; i < levelZeroBuildings.Count; i++)
            {
                text_BuildingCosts[i].text = NumberFormatter.FormatNumber(levelZeroBuildings[i].buildingSO.building_UpgradeCosts[0], FormattingTypes.Cost);
            }
        }
    }

    public void CloseAlwaysOnUI()
    {
        alwaysOnUI.SetActive(false);

        text_BuildingCosts.Clear();
        levelZeroBuildings.Clear();
    }

    #endregion

    #region Main UI

    public void InitializeMainUI()
    {
        text_GetIridiumButton = button_GetIridium.GetComponentInChildren<TMP_Text>();
        text_UpgradeClickButton = button_UpgradeClick.GetComponentInChildren<TMP_Text>();
        text_UpgradeBuildingButton = button_UpgradeBuilding.GetComponentInChildren<TMP_Text>();

        button_GetBoost.onClick.AddListener(OpenBoostMenu);
        button_StocksMenu.onClick.AddListener(() => OpenStocksMenu(false));
        button_SellStocksMenu.onClick.AddListener(() => OpenStocksMenu(true));
        button_UpgradeClick.onClick.AddListener(gameManager.UpgradeClickClicked);
    }

    public void OpenMainUI()
    {
        mainUI.SetActive(true);
    }

    private void UpdateMainUI()
    {
        if (!mainUI.activeSelf) return;

        text_GetIridiumButton.text = "Get Iridium \n(+" + NumberFormatter.FormatNumber((gameManager.playerData.iridium_PerClick * gameManager.playerData.iridium_PerClickBoost), FormattingTypes.IridiumPerSecond) + " Iridium)";

        text_UpgradeClickButton.text = "Upgrade Click (" + NumberFormatter.FormatNumber(gameManager.upgradeClick_CurrentCost, FormattingTypes.Cost) + ")";

        button_UpgradeClick.interactable = gameManager.playerData.iridium_Current > gameManager.upgradeClick_CurrentCost;
    }

    public void CloseMainUI()
    {
        mainUI.SetActive(false);
    }

    #endregion

    #region Building UI

    public void InitializeBuildingUI()
    {
        button_Back_Building.onClick.AddListener(CloseBuildingMenu);
    }

    public void OpenBuildingMenu()
    {
        Building localSelectedBuilding = gameManager.BuildingManagerRef.selectedBuilding;

        CloseAllPanels();

        gameManager.BuildingManagerRef.selectedBuilding = localSelectedBuilding;
        PopulateBuildingUI();
        buildingUI.SetActive(true);
    }

    public void PopulateBuildingUI()
    {
        CleanUpBuildingUI();

        if (gameManager.BuildingManagerRef.selectedBuilding != null)
        {
            button_Troop = new List<Button>();
            text_TroopNames = new List<TMP_Text>();
            text_TroopCosts = new List<TMP_Text>();
            text_TroopsOwned = new List<TMP_Text>();
            text_TroopIPS = new List<TMP_Text>();

            button_UpgradeBuilding.onClick.AddListener(() => gameManager.BuildingManagerRef.UpgradeBuilding(gameManager.BuildingManagerRef.selectedBuilding));

            for (int i = 0; i < gameManager.BuildingManagerRef.selectedBuilding.buildingData.building_OwnedTroops.Count; i++)
            {
                int j = i;

                GameObject newButton = Instantiate(troopButtonPrefab, troopButtonParent.transform);
                newButton.name = gameManager.BuildingManagerRef.selectedBuilding.buildingData.building_OwnedTroops[i].troop_Name + " Button";

                button_Troop.Add(newButton.transform.GetChild(0).GetComponent<Button>());
                text_TroopNames.Add(newButton.transform.GetChild(1).GetComponent<TMP_Text>());
                text_TroopCosts.Add(newButton.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>());
                text_TroopsOwned.Add(newButton.transform.GetChild(2).GetComponent<TMP_Text>());
                text_TroopIPS.Add(newButton.transform.GetChild(3).GetComponent<TMP_Text>());

                button_Troop[i].onClick.AddListener(() => gameManager.TroopBuyClicked(j));
            }
        }
    }

    private void UpdateBuildingUI()
    {
        if (!buildingUI.activeSelf) return;

        text_BuildingName.text = gameManager.BuildingManagerRef.selectedBuilding.buildingData.building_Name + " (Lvl " + NumberFormatter.FormatNumber(gameManager.BuildingManagerRef.selectedBuilding.buildingData.building_Level, FormattingTypes.Level) + ")";
        text_BuildingIridiumPerSecond.text = NumberFormatter.FormatNumber(gameManager.BuildingManagerRef.selectedBuilding.GetIridiumPerTick() * GameManager.ticksPerSecond, FormattingTypes.IridiumPerSecond) + " Iridium/s";

        if (gameManager.BuildingManagerRef.selectedBuilding.buildingSO.building_UpgradeCosts[gameManager.BuildingManagerRef.selectedBuilding.buildingData.building_Level] == -1)
        {
            button_UpgradeBuilding.gameObject.SetActive(false);
        }
        else
        {
            button_UpgradeBuilding.gameObject.SetActive(true);

            if (gameManager.playerData.iridium_Current < gameManager.BuildingManagerRef.selectedBuilding.buildingSO.building_UpgradeCosts[gameManager.BuildingManagerRef.selectedBuilding.buildingData.building_Level])
            {
                button_UpgradeBuilding.interactable = false;
            }
            else
            {
                button_UpgradeBuilding.interactable = true;
            }

            text_UpgradeBuildingButton.text = "Upgrade (" + NumberFormatter.FormatNumber(gameManager.BuildingManagerRef.selectedBuilding.buildingSO.building_UpgradeCosts[gameManager.BuildingManagerRef.selectedBuilding.buildingData.building_Level], FormattingTypes.Cost) + ")";
        }

        for (int i = 0; i < gameManager.BuildingManagerRef.selectedBuilding.buildingData.building_OwnedTroops.Count; i++)
        {
            text_TroopNames[i].text = gameManager.BuildingManagerRef.selectedBuilding.buildingData.building_OwnedTroops[i].troop_Name;

            text_TroopCosts[i].text = NumberFormatter.FormatNumber(gameManager.BuildingManagerRef.selectedBuilding.buildingData.building_OwnedTroops[i].troop_CurrentCost, FormattingTypes.Cost);
            if (gameManager.playerData.iridium_Current < gameManager.BuildingManagerRef.selectedBuilding.buildingData.building_OwnedTroops[i].troop_CurrentCost)
            {
                button_Troop[i].interactable = false;
            }
            else
            {
                button_Troop[i].interactable = true;
            }

            text_TroopsOwned[i].text = NumberFormatter.FormatNumber(gameManager.BuildingManagerRef.selectedBuilding.buildingData.building_OwnedTroops[i].troops_Owned, FormattingTypes.Owned) + " owned";
            text_TroopIPS[i].text = "+" + NumberFormatter.FormatNumber(gameManager.BuildingManagerRef.selectedBuilding.buildingData.building_OwnedTroops[i].GetIridiumPerTickPerTroop() * GameManager.ticksPerSecond, FormattingTypes.IridiumPerSecond) + "i/s";
        }
    }

    public void CleanUpBuildingUI()
    {
        if (button_UpgradeBuilding != null)
            button_UpgradeBuilding.onClick.RemoveAllListeners();

        if (button_Troop != null)
        {
            foreach (Button button in button_Troop)
            {
                button.onClick.RemoveAllListeners();
            }

            foreach (Button button in button_Troop)
            {
                Destroy(button.transform.parent.gameObject);
            }

            button_Troop.Clear();
        }

        RectTransform rTransform = troopButtonParent.GetComponent<RectTransform>();
        rTransform.anchoredPosition = new Vector3(rTransform.anchoredPosition.x, 0);

        if (text_TroopNames != null)
            text_TroopNames.Clear();

        if (text_TroopCosts != null)
            text_TroopCosts.Clear();

        if (text_TroopsOwned != null)
            text_TroopsOwned.Clear();

        if (text_TroopIPS != null)
            text_TroopIPS.Clear();
    }

    public void CloseBuildingMenu()
    {
        OpenMainUI();

        buildingUI.SetActive(false);
        gameManager.BuildingManagerRef.selectedBuilding = null;
    }

    #endregion

    #region Boost UI

    public void InitializeBoostUI()
    {
        button_Back_Boost.onClick.AddListener(CloseBoostMenu);
    }

    public void OpenBoostMenu()
    {
        CloseAllPanels();

        PopulateBoostUI();
        boostUI.SetActive(true);
    }

    public void PopulateBoostUI()
    {
        CleanUpBoostUI();

        button_Boosts = new List<Button>();
        text_BoostNames = new List<TMP_Text>();
        text_BoostDurations = new List<TMP_Text>();
        image_BoostDurationRemainings = new List<Image>();
        text_BoostDurationRemainings = new List<TMP_Text>();

        for (int i = 0; i < gameManager.BoostManagerRef.boostSOs.Count; i++)
        {
            int j = i;

            GameObject newButton = Instantiate(boostButtonPrefab, boostButtonParent.transform);
            newButton.name = gameManager.BoostManagerRef.boostSOs[i].boost_Name;

            button_Boosts.Add(newButton.transform.GetChild(2).GetComponent<Button>());
            text_BoostNames.Add(newButton.transform.GetChild(1).GetComponent<TMP_Text>());
            text_BoostDurations.Add(newButton.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>());
            image_BoostDurationRemainings.Add(newButton.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>());
            text_BoostDurationRemainings.Add(newButton.transform.GetChild(3).GetComponent<TMP_Text>());

            button_Boosts[i].onClick.AddListener(() => gameManager.BoostManagerRef.AddBoost(gameManager.BoostManagerRef.boostSOs[j]));
        }
    }

    private void UpdateBoostUI()
    {
        if (!boostUI.activeSelf) return;

        for (int i = 0; i < gameManager.BoostManagerRef.boostSOs.Count; i++)
        {
            text_BoostNames[i].text = gameManager.BoostManagerRef.boostSOs[i].boost_Name;

            text_BoostDurations[i].text = "+ " + NumberFormatter.FormatNumber(gameManager.BoostManagerRef.boostSOs[i].boost_Duration, FormattingTypes.BoostDuration);

            Boost boost = Array.Find(gameManager.BoostManagerRef.activeBoosts.ToArray(), x => x.boost_Name == gameManager.BoostManagerRef.boostSOs[i].boost_Name);

            if (boost != null)
            {
                image_BoostDurationRemainings[i].fillAmount = (float)(boost.boost_TimeRemaining / gameManager.BoostManagerRef.boostSOs[i].boost_MaxDuration);
                text_BoostDurationRemainings[i].text = NumberFormatter.FormatNumber(boost.boost_TimeRemaining, FormattingTypes.BoostDuration) + " Left";
            }
            else
            {
                image_BoostDurationRemainings[i].fillAmount = 0f;
                text_BoostDurationRemainings[i].text = NumberFormatter.FormatNumber(0f, FormattingTypes.BoostDuration) + " Left";
            }
        }
    }

    public void CleanUpBoostUI()
    {
        if (button_Boosts != null)
        {
            foreach (Button button in button_Boosts)
            {
                button.onClick.RemoveAllListeners();
            }

            foreach (Button button in button_Boosts)
            {
                Destroy(button.transform.parent.gameObject);
            }

            button_Boosts.Clear();
        }

        RectTransform rTransform = boostButtonParent.GetComponent<RectTransform>();
        rTransform.anchoredPosition = new Vector3(rTransform.anchoredPosition.x, 0);

        if (text_BoostNames != null)
            text_BoostNames.Clear();

        if (text_BoostDurations != null)
            text_BoostDurations.Clear();

        if (image_BoostDurationRemainings != null)
            image_BoostDurationRemainings.Clear();

        if (text_BoostDurationRemainings != null)
            text_BoostDurationRemainings.Clear();
    }

    public void CloseBoostMenu()
    {
        OpenMainUI();

        boostUI.SetActive(false);
        CleanUpBoostUI();
    }

    #endregion

    #region Stocks UI

    public void InitializeStocksUI()
    {
        nextStock.onClick.AddListener(gameManager.StockManagerRef.NextStock);
        prevStock.onClick.AddListener(gameManager.StockManagerRef.PreviousStock);

        button_Prev.onClick.AddListener(gameManager.StockManagerRef.PreviousAmountBuy);
        button_MegaPrev.onClick.AddListener(gameManager.StockManagerRef.MegaPreviousAmountBuy);

        button_Next.onClick.AddListener(gameManager.StockManagerRef.NextAmountBuy);
        button_MegaNext.onClick.AddListener(gameManager.StockManagerRef.MegaNextAmountBuy);

        button_StocksBack.onClick.AddListener(CloseStocksMenu);

        text_BuyStocksButton = button_BuyStocks.GetComponentInChildren<TMP_Text>();
    }

    public void OpenStocksMenu(bool sellMode)
    {
        CloseAllPanels();

        gameManager.StockManagerRef.sellMode = sellMode;

        for (int i = 0; i < gameManager.StockManagerRef.stocks.Count; i++)
        {
            gameManager.StockManagerRef.stocks[i].amountToBuy = gameManager.StockManagerRef.stocks[i].stockMinimumBuy;
        }

        button_BuyStocks.onClick.RemoveAllListeners();

        if (sellMode)
        {
            button_BuyStocks.onClick.AddListener(gameManager.StockManagerRef.SellStocks);
        }
        else
        {
            button_BuyStocks.onClick.AddListener(gameManager.StockManagerRef.BuyStocks);
        }

        stocksUI.SetActive(true);
    }

    public void UpdateStocksMenu()
    {
        if (!stocksUI.activeSelf) return;

        StockManager localSM = gameManager.StockManagerRef;
        DateTime localNow = DateTime.Now;

        text_StockName.text = localSM.stocks[localSM.selectedStockIndex].stockName;
        text_StockNextExpire.text = "Expires in: " + NumberFormatter.FormatNumber(((DateTime)localSM.stocks[localSM.selectedStockIndex].nextExpireTime - localNow).TotalSeconds, FormattingTypes.Time);

        text_StockOwned.text = NumberFormatter.FormatNumber(localSM.stocks[localSM.selectedStockIndex].stockOwned, FormattingTypes.Iridium) + " Owned";

        text_StockNextRefresh.text = "Refreshes in: " + NumberFormatter.FormatNumber(((DateTime)localSM.stocks[localSM.selectedStockIndex].nextRefreshTime - localNow).TotalSeconds, FormattingTypes.Time);

        if (localSM.sellMode)
        {
            text_BuyStocksButton.text = "Sell";

            button_Next.interactable = localSM.stocks[localSM.selectedStockIndex].stockOwned >= (localSM.stocks[localSM.selectedStockIndex].amountToBuy + localSM.stocks[localSM.selectedStockIndex].nextStep);
            button_MegaNext.interactable = localSM.stocks[localSM.selectedStockIndex].stockOwned >= (localSM.stocks[localSM.selectedStockIndex].amountToBuy + localSM.stocks[localSM.selectedStockIndex].megaNextStep);

            button_BuyStocks.interactable = localSM.stocks[localSM.selectedStockIndex].stockOwned >= localSM.stocks[localSM.selectedStockIndex].amountToBuy;
        }
        else
        {
            text_BuyStocksButton.text = "Buy";

            button_Next.interactable = gameManager.playerData.iridium_Current >= localSM.stocks[localSM.selectedStockIndex].totalPricePlusNext;
            button_MegaNext.interactable = gameManager.playerData.iridium_Current >= localSM.stocks[localSM.selectedStockIndex].totalPricePlusMegaNext;

            button_BuyStocks.interactable = ((gameManager.playerData.iridium_Current >= localSM.stocks[localSM.selectedStockIndex].totalPrice) && !localSM.stocks[localSM.selectedStockIndex].purchasedThisCycle);
        }

        button_MegaPrev.interactable = localSM.stocks[localSM.selectedStockIndex].amountToBuy > localSM.stocks[localSM.selectedStockIndex].stockMinimumBuy;
        button_Prev.interactable = localSM.stocks[localSM.selectedStockIndex].amountToBuy > localSM.stocks[localSM.selectedStockIndex].stockMinimumBuy;

        text_CurrentPrice.text = NumberFormatter.FormatNumber(gameManager.StockManagerRef.stocks[localSM.selectedStockIndex].stockCurrentValue, FormattingTypes.Stocks);
        text_AmountToBuy.text = NumberFormatter.FormatNumber(gameManager.StockManagerRef.stocks[localSM.selectedStockIndex].amountToBuy, FormattingTypes.Iridium);
        text_TotalPrice.text = NumberFormatter.FormatNumber(gameManager.StockManagerRef.stocks[localSM.selectedStockIndex].totalPrice, FormattingTypes.IridiumPerSecond);
    }

    public void CloseStocksMenu()
    {
        OpenMainUI();

        stocksUI.SetActive(false);
    }

    #endregion

    #region Profile Select UI

    public void InitializeProfileSelectUI()
    {
        button_newProfile.onClick.AddListener(OpenProfileCreatePanel);
    }

    public void OpenProfileSelect()
    {
        profileSelectionUI.SetActive(true);
    }

    public void PopulateProfileSelectUI(List<PlayerData> profilesList)
    {
        CleanUpProfileSelectUI();

        OpenProfileSelect();

        button_Profiles = new List<Button>();
        text_ProfileNames = new List<TMP_Text>();

        for (int i = 0; i < profilesList.Count; i++)
        {
            int j = i;
            GameObject newButton = Instantiate(profileButtonPrefab, profileButtonParent.transform);
            newButton.name = profilesList[i].profileName;

            Button button = newButton.GetComponent<Button>();
            TMP_Text tmp_text = newButton.GetComponentInChildren<TMP_Text>();

            button_Profiles.Add(button);
            text_ProfileNames.Add(tmp_text);

            button.onClick.AddListener(() => gameManager.LoadGame(profilesList[j].profileName));
            tmp_text.text = profilesList[i].profileName;
        }
    }

    public void CleanUpProfileSelectUI()
    {
        if (button_Profiles != null)
        {
            foreach (Button button in button_Profiles)
            {
                button.onClick.RemoveAllListeners();
            }

            foreach (Button button in button_Profiles)
            {
                Destroy(button.transform.parent.gameObject);
            }
            button_Profiles.Clear();
        }

        RectTransform rTransform = profileButtonParent.GetComponent<RectTransform>();
        rTransform.anchoredPosition = new Vector3(rTransform.anchoredPosition.x, 0);

        if (text_ProfileNames != null)
            text_ProfileNames.Clear();
    }

    public void CloseProfileUI()
    {
        profileSelectionUI.SetActive(false);
    }

    #endregion

    #region Profile Create UI

    public void InitializeProfileCreateUI()
    {
        button_CreateProfile.onClick.AddListener(CreateProfile);
    }

    public void OpenProfileCreatePanel()
    {
        CloseProfileUI();

        profileCreationUI.SetActive(true);
    }

    public void CreateProfile()
    {
        string profileName = inputField_ProfileName.text;

        if (profileName.Length > 0)
        {
            gameManager.StartNewGame(profileName);
            CloseProfileCreatePanel();
        }
        else
        {
            Debug.LogError("Profile name empty!");
        }
    }

    public void CloseProfileCreatePanel()
    {
        OpenMainUI();

        profileCreationUI.SetActive(false);
    }

    #endregion

    #region Deprecated Code

    /*[Header("Building Buy UI")]
    private GameObject buildingBuyUI;
    private Button button_Back_BuyBuilding;
    private GameObject buildingButtonParent;
    private GameObject buildingButtonPrefab;

    private List<Button> button_Buildings;
    private List<TMP_Text> text_BuildingNames;
    private List<TMP_Text> text_BuildingsOwned;*/

    /*public void CloseAllPanels()
    {
        ShowMainUI();
        CLoseProfileNamePanel();
        CloseProfileUI();
        CloseBuildingMenu();
        CloseShop();
        CloseBoostMenu();
    }*/

    /*public void PopulateBuyBuildingUI()
    {
        CleanUpBuyBuildingUI();

        button_Buildings = new List<Button>();
        text_BuildingNames = new List<TMP_Text>();
        text_BuildingCosts = new List<TMP_Text>();
        text_BuildingsOwned = new List<TMP_Text>();

        BuildingLocation[] buildings = gameManager.BuildingManager.buildingLocations.ToArray();

        for (int i = 0; i < buildings.Length; i++)
        {
            int j = i;

            GameObject newButton = Instantiate(buildingButtonPrefab, buildingButtonParent.transform);
            newButton.name = buildings[i].buildingSO.building_Name + " Button";
            button_Buildings.Add(newButton.GetComponent<Button>());
            text_BuildingNames.Add(newButton.transform.GetChild(0).GetComponent<TMP_Text>());
            text_BuildingCosts.Add(newButton.transform.GetChild(1).GetComponent<TMP_Text>());
            text_BuildingsOwned.Add(newButton.transform.GetChild(2).GetComponent<TMP_Text>());

            button_Buildings[i].onClick.AddListener(() => gameManager.BuyBuildingClicked(buildings[j].buildingSO));
        }
    }*/

    /*public void CleanUpBuyBuildingUI()
    {
        if (button_Buildings != null)
        {
            foreach (Button button in button_Buildings)
            {
                button.onClick.RemoveAllListeners();
            }

            foreach (Button button in button_Buildings)
            {
                Destroy(button.gameObject);
            }

            button_Buildings.Clear();
        }

        if (text_BuildingNames != null)
            text_BuildingNames.Clear();

        if (text_BuildingCosts != null)
            text_BuildingCosts.Clear();

        if (text_BuildingsOwned != null)
            text_BuildingsOwned.Clear();
    }*/

    /*public void OpenShop()
    {
        CloseAllPanels();
        HideMainUI();
        buildingBuyUI.SetActive(true);
        PopulateBuyBuildingUI();
    }*/

    /*public void CloseShop()
    {
        ShowMainUI();
        CleanUpBuyBuildingUI();
        buildingBuyUI.SetActive(false);
    }*/

    #endregion
}
