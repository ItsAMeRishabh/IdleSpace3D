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

    [SerializeField] private Material purchasePlotFrame_CanBuy;
    [SerializeField] private Material purchasePlotFill_CanBuy;
    [SerializeField] private Material purchasePlotFrame_CannotBuy;
    [SerializeField] private Material purchasePlotFill_CannotBuy;

    private List<TMP_Text> text_BuildingCosts;
    private List<Building> levelZeroBuildings;
    private List<Transform> frameParents;
    private List<Transform> fillParents;

    [Header("Main UI")]
    [SerializeField] private GameObject mainUI;
    [SerializeField] private Button button_GetIridium;
    [SerializeField] private Button button_GetBoost;
    [SerializeField] private GameObject iridiumBoost_QuickInfo;
    [SerializeField] private GameObject darkElixirBoost_QuickInfo;
    [SerializeField] private Button button_StocksMenu;
    [SerializeField] private Button button_SellStocksMenu;
    [SerializeField] private Button button_UpgradeClick;

    private TMP_Text text_IridiumBoostQuickInfoMultiplier;
    private TMP_Text text_IridiumBoostQuickInfoTimer;
    private TMP_Text text_DarkElixirBoostQuickInfoMultiplier;
    private TMP_Text text_DarkElixirBoostQuickInfoTimer;

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
    [SerializeField] private Button button_NextBuilding;
    [SerializeField] private Button button_PreviousBuilding;
    [SerializeField] private TMP_Text text_BuildingName;
    [SerializeField] private TMP_Text text_BuildingIridiumPerSecond;
    [SerializeField] private Button button_UpgradeBuilding;
    [SerializeField] private Button button_Back_Building;
    private TMP_Text text_UpgradeBuildingButton;

    [SerializeField] private GameObject troopButtonParent;
    [SerializeField] private GameObject troopLockedButtonPrefab;
    [SerializeField] private string lockedHexCode = "FF6F12";

    private List<Image> image_TroopSpriteLocked;
    private List<TMP_Text> text_TroopLockedUpgrade;

    [SerializeField] private GameObject troopUnlockedButtonPrefab;

    private List<Button> button_TroopBuy;
    private List<Button> button_TroopUpgrade;
    private List<Image> image_TroopSpriteUnlocked;
    private List<TMP_Text> text_TroopCosts;
    private List<TMP_Text> text_TroopUpgradeCosts;
    private List<TMP_Text> text_TroopsOwned;
    private List<TMP_Text> text_TroopIPS;

    [Header("Stocks Buy UI")]
    [SerializeField] private GameObject stockBuyUI;
    [SerializeField] private TMP_Text text_BuyStockCurrentValue;
    [SerializeField] private TMP_Text text_AmountToBuy;
    [SerializeField] private Button button_BuyStocks;
    private TMP_Text text_BuyStocksButton;
    [SerializeField] private Button button_BuyPrev;
    [SerializeField] private Button button_BuyMegaPrev;
    [SerializeField] private Button button_BuyNext;
    [SerializeField] private Button button_BuyMegaNext;
    [SerializeField] private Button button_BuyStocksBack;
    [SerializeField] private TMP_Text text_BuyStockNextExpire;

    [Header("Stock Sell UI")]
    [SerializeField] private GameObject stockSellUI;
    [SerializeField] private TMP_Text text_StockOwned;
    [SerializeField] private TMP_Text text_StockNextRefresh;
    [SerializeField] private TMP_Text text_SellStockCurrentValue;
    [SerializeField] private TMP_Text text_AmountToSell;
    [SerializeField] private Button button_SellNext;
    [SerializeField] private Button button_SellMegaNext;
    [SerializeField] private Button button_SellPrev;
    [SerializeField] private Button button_SellMegaPrev;
    [SerializeField] private Button button_SellAllOwned;
    [SerializeField] private Button button_SellStock;
    private TMP_Text text_SellStockButton;
    [SerializeField] private Button button_SellStockBack;
    [SerializeField] private TMP_Text text_SellStockNextExpire;

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

    [Header("AFK Report UI")]
    [SerializeField] private GameObject afkReportUI;
    [SerializeField] private TMP_Text text_AFKTime;
    [SerializeField] private TMP_Text text_IridiumReward;
    [SerializeField] private TMP_Text text_DarkElixirReward;
    [SerializeField] private Button button_NoThanks;
    [SerializeField] private Button button_RewardX2;
    [SerializeField] private Button button_RewardX3;


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

        InitializeStockBuyUI();

        InitializeStockSellUI();

        InitializeProfileSelectUI();

        InitializeProfileCreateUI();
    }

    public void StartGame()
    {
        CloseAllPanels();

        OpenAlwaysOnUI();

        OpenMainUI();
    }

    public void UpdateAllUI()
    {
        UpdateAlwaysOnUI();

        UpdateMainUI();

        UpdateBuildingUI();

        UpdateBoostUI();

        UpdateStockBuyMenu();

        UpdateStockSellMenu();
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
        frameParents = new List<Transform>();
        fillParents = new List<Transform>();

        foreach (Building building in gameManager.BuildingManagerRef.ownedBuildings)
        {
            if (building.buildingData.building_Level != 0)
                continue;

            levelZeroBuildings.Add(building);
            frameParents.Add(building.transform.parent.GetChild(1));
            fillParents.Add(building.transform.parent.GetChild(2));
            text_BuildingCosts.Add(building.transform.parent.GetChild(3).GetChild(1).GetComponent<TMP_Text>());
        }
    }

    public void UpdateAlwaysOnUI()
    {
        text_TotalIridium.text = NumberFormatter.FormatNumber(gameManager.playerData.iridium_Current, FormattingTypes.Iridium);
        text_IridiumPerSecond.text = NumberFormatter.FormatNumber((gameManager.playerData.iridium_PerSecond * gameManager.playerData.iridium_PerSecondBoost), FormattingTypes.IridiumPerSecond) + " /s";

        text_TotalDarkElixir.text = NumberFormatter.FormatNumber(gameManager.playerData.darkElixir_Current, FormattingTypes.DarkElixir);
        text_DarkElixirPerSecond.text = NumberFormatter.FormatNumber(gameManager.playerData.darkElixir_PerSecond * gameManager.playerData.darkElixir_PerSecondBoost, FormattingTypes.DarkElixirPerSecond);

        UpdateBuildingCosts();
    }

    private void UpdateBuildingCosts()
    {
        if (levelZeroBuildings.Count != 0)
        {
            for (int i = 0; i < levelZeroBuildings.Count; i++)
            {
                text_BuildingCosts[i].text = NumberFormatter.FormatNumber(levelZeroBuildings[i].buildingSO.building_UpgradeCosts[0], FormattingTypes.IridiumCost);

                if (gameManager.playerData.iridium_Current >= levelZeroBuildings[i].buildingSO.building_UpgradeCosts[0])
                {
                    for (int j = 0; j < frameParents[i].childCount; j++)
                    {
                        frameParents[i].GetChild(j).GetComponent<MeshRenderer>().material = purchasePlotFrame_CanBuy;
                    }
                    for (int j = 0; j < fillParents[i].childCount; j++)
                    {
                        fillParents[i].GetChild(j).GetComponent<MeshRenderer>().material = purchasePlotFill_CanBuy;
                    }
                }
                else
                {
                    for (int j = 0; j < frameParents[i].childCount; j++)
                    {
                        frameParents[i].GetChild(j).GetComponent<MeshRenderer>().material = purchasePlotFrame_CannotBuy;
                    }
                    for (int j = 0; j < fillParents[i].childCount; j++)
                    {
                        fillParents[i].GetChild(j).GetComponent<MeshRenderer>().material = purchasePlotFill_CannotBuy;
                    }
                }
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

        text_IridiumBoostQuickInfoMultiplier = iridiumBoost_QuickInfo.transform.GetChild(0).GetComponent<TMP_Text>();
        text_IridiumBoostQuickInfoTimer = iridiumBoost_QuickInfo.transform.GetChild(1).GetComponent<TMP_Text>();

        text_DarkElixirBoostQuickInfoMultiplier = darkElixirBoost_QuickInfo.transform.GetChild(0).GetComponent<TMP_Text>();
        text_DarkElixirBoostQuickInfoTimer = darkElixirBoost_QuickInfo.transform.GetChild(1).GetComponent<TMP_Text>();

        button_GetBoost.onClick.AddListener(OpenBoostMenu);
        button_GetBoost.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });

        button_StocksMenu.onClick.AddListener(OpenStockBuyMenu);
        button_StocksMenu.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });

        button_SellStocksMenu.onClick.AddListener(OpenStockSellMenu);
        button_SellStocksMenu.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });

        button_UpgradeClick.onClick.AddListener(gameManager.UpgradeClickClicked);
        button_UpgradeClick.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });

    }

    public void OpenMainUI()
    {
        mainUI.SetActive(true);
    }

    private void UpdateMainUI()
    {
        if (!mainUI.activeSelf) return;

        //text_GetIridiumButton.text = "Get Iridium \n(+" + NumberFormatter.FormatNumber((gameManager.playerData.iridium_PerClick * gameManager.playerData.iridium_PerClickBoost), FormattingTypes.IridiumPerSecond) + " Iridium)";

        text_UpgradeClickButton.text = NumberFormatter.FormatNumber(gameManager.upgradeClick_CurrentCost, FormattingTypes.IridiumCost);

        button_UpgradeClick.interactable = gameManager.playerData.iridium_Current > gameManager.upgradeClick_CurrentCost;

        if (gameManager.BoostManagerRef.iridium_LowestTime == null)
        {
            iridiumBoost_QuickInfo.SetActive(false);
        }
        else
        {
            iridiumBoost_QuickInfo.SetActive(true);

            text_IridiumBoostQuickInfoMultiplier.text = NumberFormatter.FormatNumber(gameManager.playerData.iridium_PerSecondBoost, FormattingTypes.BoostMultiplier) + "X";
            text_IridiumBoostQuickInfoTimer.text = NumberFormatter.FormatNumber(gameManager.BoostManagerRef.iridium_LowestTime.boost_TimeRemaining, FormattingTypes.BoostTimer);
        }

        if (gameManager.BoostManagerRef.darkElixir_LowestTime == null)
        {
            darkElixirBoost_QuickInfo.SetActive(false);
        }
        else
        {
            darkElixirBoost_QuickInfo.SetActive(true);

            text_DarkElixirBoostQuickInfoMultiplier.text = NumberFormatter.FormatNumber(gameManager.playerData.darkElixir_PerSecondBoost, FormattingTypes.BoostMultiplier) + "X";
            text_DarkElixirBoostQuickInfoTimer.text = NumberFormatter.FormatNumber(gameManager.BoostManagerRef.darkElixir_LowestTime.boost_TimeRemaining, FormattingTypes.BoostTimer);
        }
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
        button_Back_Building.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });

        button_NextBuilding.onClick.AddListener(gameManager.BuildingManagerRef.NextBuilding);
        button_NextBuilding.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });

        button_PreviousBuilding.onClick.AddListener(gameManager.BuildingManagerRef.PreviousBuilding);
        button_PreviousBuilding.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });
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
            image_TroopSpriteLocked = new List<Image>();
            image_TroopSpriteUnlocked = new List<Image>();

            button_TroopBuy = new List<Button>();
            button_TroopUpgrade = new List<Button>();

            text_TroopLockedUpgrade = new List<TMP_Text>();
            text_TroopCosts = new List<TMP_Text>();
            text_TroopUpgradeCosts = new List<TMP_Text>();
            text_TroopsOwned = new List<TMP_Text>();
            text_TroopIPS = new List<TMP_Text>();

            button_UpgradeBuilding.onClick.AddListener(() => gameManager.BuildingManagerRef.UpgradeBuilding(gameManager.BuildingManagerRef.selectedBuilding));
            button_UpgradeBuilding.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });

            BuildingSO populatingBuildingSO = gameManager.BuildingManagerRef.selectedBuilding.buildingSO;

            for (int i = 0; i < gameManager.BuildingManagerRef.selectedBuilding.buildingData.building_OwnedTroops.Count; i++)
            {
                int j = i;

                GameObject newButton = Instantiate(troopUnlockedButtonPrefab, troopButtonParent.transform);
                newButton.name = gameManager.BuildingManagerRef.selectedBuilding.buildingData.building_OwnedTroops[i].troop_Name + "_Button";

                button_TroopBuy.Add(newButton.transform.GetChild(0).GetComponent<Button>());
                text_TroopCosts.Add(newButton.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>());

                button_TroopUpgrade.Add(newButton.transform.GetChild(1).GetComponent<Button>());
                text_TroopUpgradeCosts.Add(newButton.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>());

                image_TroopSpriteUnlocked.Add(newButton.transform.GetChild(2).GetComponent<Image>());
                text_TroopsOwned.Add(newButton.transform.GetChild(3).GetComponent<TMP_Text>());
                text_TroopIPS.Add(newButton.transform.GetChild(4).GetComponent<TMP_Text>());

                button_TroopBuy[i].onClick.AddListener(() => gameManager.TroopBuyClicked(j));
                button_TroopBuy[i].onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });

                button_TroopUpgrade[i].onClick.AddListener(() => gameManager.TroopUpgradeClicked(j));
                button_TroopUpgrade[i].onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });
            }

            for (int i = 0; i < populatingBuildingSO.levelUpUnlocks.Count; i++)
            {
                if (gameManager.BuildingManagerRef.selectedBuilding.buildingData.building_Level >= populatingBuildingSO.levelUpUnlocks[i].level) continue;

                for (int j = 0; j < populatingBuildingSO.levelUpUnlocks[i].unlockedTroops.Count; j++)
                {
                    GameObject newButton = Instantiate(troopLockedButtonPrefab, troopButtonParent.transform);
                    newButton.name = populatingBuildingSO.levelUpUnlocks[i].unlockedTroops[j].troop_Name + "_Locked";

                    image_TroopSpriteLocked.Add(newButton.transform.GetChild(0).GetComponent<Image>());
                    text_TroopLockedUpgrade.Add(newButton.transform.GetChild(1).GetComponent<TMP_Text>());
                }
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

            text_UpgradeBuildingButton.text = NumberFormatter.FormatNumber(gameManager.BuildingManagerRef.selectedBuilding.buildingSO.building_UpgradeCosts[gameManager.BuildingManagerRef.selectedBuilding.buildingData.building_Level], FormattingTypes.IridiumCost);
        }

        for (int i = 0; i < gameManager.BuildingManagerRef.selectedBuilding.buildingData.building_OwnedTroops.Count; i++)
        {
            image_TroopSpriteUnlocked[i].sprite = gameManager.BuildingManagerRef.selectedBuilding.buildingData.building_OwnedTroops[i].troop_UnlockedSprite;

            text_TroopCosts[i].text = NumberFormatter.FormatNumber(gameManager.BuildingManagerRef.selectedBuilding.buildingData.building_OwnedTroops[i].troop_CurrentCost, FormattingTypes.IridiumCost);
            if (gameManager.playerData.iridium_Current < gameManager.BuildingManagerRef.selectedBuilding.buildingData.building_OwnedTroops[i].troop_CurrentCost)
            {
                button_TroopBuy[i].interactable = false;
            }
            else
            {
                button_TroopBuy[i].interactable = true;
            }

            text_TroopUpgradeCosts[i].text = NumberFormatter.FormatNumber(gameManager.BuildingManagerRef.selectedBuilding.buildingData.building_OwnedTroops[i].troop_CurrentUpgradeCost, FormattingTypes.DarkElixirCost);
            if (gameManager.playerData.darkElixir_Current < gameManager.BuildingManagerRef.selectedBuilding.buildingData.building_OwnedTroops[i].troop_CurrentUpgradeCost)
            {
                button_TroopUpgrade[i].interactable = false;
            }
            else
            {
                button_TroopUpgrade[i].interactable = true;
            }

            text_TroopsOwned[i].text = NumberFormatter.FormatNumber(gameManager.BuildingManagerRef.selectedBuilding.buildingData.building_OwnedTroops[i].troops_Owned, FormattingTypes.Owned);
            text_TroopIPS[i].text = "+" + NumberFormatter.FormatNumber(gameManager.BuildingManagerRef.selectedBuilding.buildingData.building_OwnedTroops[i].GetIridiumPerTickPerTroop() * GameManager.ticksPerSecond, FormattingTypes.IridiumPerSecond) + " i/s";
        }

        BuildingSO populatingBuildingSO = gameManager.BuildingManagerRef.selectedBuilding.buildingSO;

        for (int i = 0, n = 0; i < populatingBuildingSO.levelUpUnlocks.Count; i++)
        {
            if (gameManager.BuildingManagerRef.selectedBuilding.buildingData.building_Level >= populatingBuildingSO.levelUpUnlocks[i].level) continue;

            for (int j = 0; j < populatingBuildingSO.levelUpUnlocks[i].unlockedTroops.Count; j++)
            {
                image_TroopSpriteLocked[n].sprite = populatingBuildingSO.levelUpUnlocks[i].unlockedTroops[j].troop_LockedSprite;
                text_TroopLockedUpgrade[n].text = $"UPGRADE TO <color=#{lockedHexCode}>{populatingBuildingSO.building_Name} {populatingBuildingSO.levelUpUnlocks[i].level}";
                n++;
            }
        }
    }

    public void CleanUpBuildingUI()
    {
        if (button_UpgradeBuilding != null)
            button_UpgradeBuilding.onClick.RemoveAllListeners();

        if (button_TroopBuy != null)
        {
            foreach (Button button in button_TroopBuy)
            {
                button.onClick.RemoveAllListeners();
            }

            foreach (Button button in button_TroopBuy)
            {
                Destroy(button.transform.parent.gameObject);
            }

            button_TroopBuy.Clear();
        }

        if (button_TroopUpgrade != null)
        {
            foreach (Button button in button_TroopUpgrade)
            {
                button.onClick.RemoveAllListeners();
            }

            foreach (Button button in button_TroopUpgrade)
            {
                Destroy(button.transform.parent.gameObject);
            }

            button_TroopUpgrade.Clear();
        }

        RectTransform rTransform = troopButtonParent.GetComponent<RectTransform>();
        rTransform.anchoredPosition = new Vector3(rTransform.anchoredPosition.x, 0);

        if (image_TroopSpriteLocked != null)
            image_TroopSpriteLocked.Clear();

        if (text_TroopLockedUpgrade != null)
        {
            foreach (TMP_Text text in text_TroopLockedUpgrade)
            {
                Destroy(text.transform.parent.gameObject);
            }

            text_TroopLockedUpgrade.Clear();
        }

        if (image_TroopSpriteUnlocked != null)
            image_TroopSpriteUnlocked.Clear();

        if (text_TroopUpgradeCosts != null)
            text_TroopUpgradeCosts.Clear();

        if (text_TroopCosts != null)
            text_TroopCosts.Clear();

        if (text_TroopsOwned != null)
            text_TroopsOwned.Clear();

        if (text_TroopIPS != null)
            text_TroopIPS.Clear();
    }

    public void CloseBuildingMenu()
    {
        if (!buildingUI.activeSelf) return;

        OpenMainUI();

        buildingUI.SetActive(false);
        gameManager.AudioManagerRef.Play("PanelClose");
        gameManager.BuildingManagerRef.selectedBuilding = null;
    }

    #endregion

    #region Boost UI

    public void InitializeBoostUI()
    {
        button_Back_Boost.onClick.AddListener(CloseBoostMenu);
        button_Back_Boost.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });
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
            button_Boosts[i].onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });
        }
    }

    private void UpdateBoostUI()
    {
        if (!boostUI.activeSelf) return;

        for (int i = 0; i < gameManager.BoostManagerRef.boostSOs.Count; i++)
        {
            text_BoostNames[i].text = gameManager.BoostManagerRef.boostSOs[i].boost_Name;

            text_BoostDurations[i].text = "+ " + NumberFormatter.FormatNumber(gameManager.BoostManagerRef.boostSOs[i].boost_Duration, FormattingTypes.BoostTimer);

            Boost boost = Array.Find(gameManager.BoostManagerRef.activeBoosts.ToArray(), x => x.boost_Name == gameManager.BoostManagerRef.boostSOs[i].boost_Name);

            if (boost != null)
            {
                image_BoostDurationRemainings[i].fillAmount = (float)(boost.boost_TimeRemaining / gameManager.BoostManagerRef.boostSOs[i].boost_MaxDuration);
                text_BoostDurationRemainings[i].text = NumberFormatter.FormatNumber(boost.boost_TimeRemaining, FormattingTypes.BoostTimer) + " Left";
            }
            else
            {
                image_BoostDurationRemainings[i].fillAmount = 0f;
                text_BoostDurationRemainings[i].text = NumberFormatter.FormatNumber(0f, FormattingTypes.BoostTimer) + " Left";
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
        if (!boostUI.activeSelf) return;
        OpenMainUI();

        boostUI.SetActive(false);
        gameManager.AudioManagerRef.Play("PanelClose");
        CleanUpBoostUI();
    }

    #endregion

    #region Stock Buy UI

    public void InitializeStockBuyUI()
    {
        button_BuyStocksBack.onClick.AddListener(CloseStockBuyMenu);
        button_BuyStocksBack.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });


        button_BuyPrev.onClick.AddListener(gameManager.StockManagerRef.PreviousAmountBuy);
        button_BuyPrev.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });

        button_BuyMegaPrev.onClick.AddListener(gameManager.StockManagerRef.MegaPreviousAmountBuy);
        button_BuyMegaPrev.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });


        button_BuyNext.onClick.AddListener(gameManager.StockManagerRef.NextAmountBuy);
        button_BuyNext.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });

        button_BuyMegaNext.onClick.AddListener(gameManager.StockManagerRef.MegaNextAmountBuy);
        button_BuyMegaNext.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });


        button_BuyStocks.onClick.AddListener(gameManager.StockManagerRef.BuyStocks);
        button_BuyStocks.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });

        text_BuyStocksButton = button_BuyStocks.transform.GetChild(0).GetComponent<TMP_Text>();
    }

    public void OpenStockBuyMenu()
    {
        CloseAllPanels();

        for (int i = 0; i < gameManager.StockManagerRef.stocks.Count; i++)
        {
            gameManager.StockManagerRef.stocks[i].amountToBuy = gameManager.StockManagerRef.stocks[i].stockMinimumBuy;
        }

        stockBuyUI.SetActive(true);
    }

    public void UpdateStockBuyMenu()
    {
        if (!stockBuyUI.activeSelf) return;

        StockManager localSM = gameManager.StockManagerRef;
        DateTime localNow = DateTime.Now;

        text_BuyStockCurrentValue.text = NumberFormatter.FormatNumber(localSM.stocks[localSM.selectedStockIndex].stockCurrentBuyValue, FormattingTypes.StocksPrice);

        text_AmountToBuy.text = NumberFormatter.FormatNumber(localSM.stocks[localSM.selectedStockIndex].amountToBuy, FormattingTypes.Stocks);

        text_BuyStocksButton.text = NumberFormatter.FormatNumber(localSM.stocks[localSM.selectedStockIndex].totalPrice, FormattingTypes.StocksPrice);

        button_BuyNext.interactable = gameManager.playerData.iridium_Current >= localSM.stocks[localSM.selectedStockIndex].totalPricePlusNext;
        button_BuyMegaNext.interactable = gameManager.playerData.iridium_Current >= localSM.stocks[localSM.selectedStockIndex].totalPricePlusMegaNext;

        button_BuyStocks.interactable = ((gameManager.playerData.iridium_Current >= localSM.stocks[localSM.selectedStockIndex].totalPrice) && !localSM.stocks[localSM.selectedStockIndex].purchasedThisCycle);

        button_BuyMegaPrev.interactable = localSM.stocks[localSM.selectedStockIndex].amountToBuy > localSM.stocks[localSM.selectedStockIndex].stockMinimumBuy;
        button_BuyPrev.interactable = localSM.stocks[localSM.selectedStockIndex].amountToBuy > localSM.stocks[localSM.selectedStockIndex].stockMinimumBuy;

        text_BuyStocksButton.text = NumberFormatter.FormatNumber(gameManager.StockManagerRef.stocks[localSM.selectedStockIndex].totalPrice, FormattingTypes.StocksPrice);

        text_BuyStockNextExpire.text = NumberFormatter.FormatNumber(((DateTime)localSM.stocks[localSM.selectedStockIndex].nextExpireTime - localNow).TotalSeconds, FormattingTypes.StocksTimer);
    }

    public void CloseStockBuyMenu()
    {
        if (!stockBuyUI.activeSelf) return;

        OpenMainUI();

        stockBuyUI.SetActive(false);
        gameManager.AudioManagerRef.Play("PanelClose");
    }

    #endregion

    #region Stock Sell UI

    public void InitializeStockSellUI()
    {
        button_SellStockBack.onClick.AddListener(CloseStockSellMenu);
        button_SellStockBack.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });

        button_SellPrev.onClick.AddListener(gameManager.StockManagerRef.PreviousAmountSell);
        button_SellPrev.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });

        button_SellMegaPrev.onClick.AddListener(gameManager.StockManagerRef.MegaPreviousAmountSell);
        button_SellMegaPrev.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });


        button_SellNext.onClick.AddListener(gameManager.StockManagerRef.NextAmountSell);
        button_SellNext.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });

        button_SellMegaNext.onClick.AddListener(gameManager.StockManagerRef.MegaNextAmountSell);
        button_SellMegaNext.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });


        button_SellAllOwned.onClick.AddListener(gameManager.StockManagerRef.AllInSell);
        button_SellAllOwned.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });


        button_SellStock.onClick.AddListener(gameManager.StockManagerRef.SellStocks);
        button_SellStock.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });

        text_SellStockButton = button_SellStock.transform.GetChild(0).GetComponent<TMP_Text>();
    }

    public void OpenStockSellMenu()
    {
        CloseAllPanels();

        for (int i = 0; i < gameManager.StockManagerRef.stocks.Count; i++)
        {
            gameManager.StockManagerRef.stocks[i].amountToSell = gameManager.StockManagerRef.stocks[i].stockMinimumSell;
        }

        stockSellUI.SetActive(true);
    }

    public void UpdateStockSellMenu()
    {
        if (!stockSellUI.activeSelf) return;

        StockManager localSM = gameManager.StockManagerRef;
        DateTime localNow = DateTime.Now;

        text_StockOwned.text = NumberFormatter.FormatNumber(localSM.stocks[localSM.selectedStockIndex].stockOwned, FormattingTypes.Stocks);

        text_StockNextRefresh.text = NumberFormatter.FormatNumber(((DateTime)localSM.stocks[localSM.selectedStockIndex].nextRefreshTime - localNow).TotalSeconds, FormattingTypes.StocksTimer);


        text_SellStockCurrentValue.text = NumberFormatter.FormatNumber((localSM.stocks[localSM.selectedStockIndex].stockCurrentSaleValue), FormattingTypes.StocksPrice);

        text_AmountToSell.text = NumberFormatter.FormatNumber(localSM.stocks[localSM.selectedStockIndex].amountToSell, FormattingTypes.Stocks);

        button_SellMegaPrev.interactable = localSM.stocks[localSM.selectedStockIndex].amountToSell > localSM.stocks[localSM.selectedStockIndex].stockMinimumSell;
        button_SellPrev.interactable = localSM.stocks[localSM.selectedStockIndex].amountToSell > localSM.stocks[localSM.selectedStockIndex].stockMinimumSell;

        button_SellNext.interactable = localSM.stocks[localSM.selectedStockIndex].stockOwned >= (localSM.stocks[localSM.selectedStockIndex].amountToSell + localSM.stocks[localSM.selectedStockIndex].nextStep);
        button_SellMegaNext.interactable = localSM.stocks[localSM.selectedStockIndex].stockOwned >= (localSM.stocks[localSM.selectedStockIndex].amountToSell + localSM.stocks[localSM.selectedStockIndex].megaNextStep);

        button_SellAllOwned.interactable = !(localSM.stocks[localSM.selectedStockIndex].stockOwned == localSM.stocks[localSM.selectedStockIndex].amountToSell);

        text_SellStockButton.text = NumberFormatter.FormatNumber(gameManager.StockManagerRef.stocks[localSM.selectedStockIndex].totalSale, FormattingTypes.StocksPrice);
        button_SellStock.interactable = localSM.stocks[localSM.selectedStockIndex].stockOwned >= localSM.stocks[localSM.selectedStockIndex].amountToSell;

        text_SellStockNextExpire.text = NumberFormatter.FormatNumber(((DateTime)localSM.stocks[localSM.selectedStockIndex].nextExpireTime - localNow).TotalSeconds, FormattingTypes.StocksTimer);
    }

    public void CloseStockSellMenu()
    {
        if (!stockSellUI.activeSelf) return;

        OpenMainUI();

        stockSellUI.SetActive(false);
        gameManager.AudioManagerRef.Play("PanelClose");
    }

    #endregion

    #region Profile Select UI

    public void InitializeProfileSelectUI()
    {
        button_newProfile.onClick.AddListener(OpenProfileCreatePanel);
        button_newProfile.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });
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
            button.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });

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
        if (!profileSelectionUI.activeSelf) return;

        profileSelectionUI.SetActive(false);
    }

    #endregion

    #region Profile Create UI

    public void InitializeProfileCreateUI()
    {
        button_CreateProfile.onClick.AddListener(CreateProfile);
        button_CreateProfile.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });
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
        if (!profileCreationUI.activeSelf) return;

        OpenMainUI();

        profileCreationUI.SetActive(false);
    }

    #endregion

    #region AFK Report UI

    public void OpenAFKReportUI(long timeElapsed, double iridiumReward, double darkElixirReward)
    {
        CloseAllPanels();

        button_NoThanks.onClick.AddListener(() => { CloseAFKReportUI(1, iridiumReward, darkElixirReward); });
        button_NoThanks.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });

        button_RewardX2.onClick.AddListener(() => { CloseAFKReportUI(2, iridiumReward, darkElixirReward); });
        button_NoThanks.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });

        button_RewardX3.onClick.AddListener(() => { CloseAFKReportUI(3, iridiumReward, darkElixirReward); });
        button_NoThanks.onClick.AddListener(() => { gameManager.AudioManagerRef.Play("ButtonClick"); });

        text_AFKTime.text = NumberFormatter.FormatNumber(timeElapsed, FormattingTypes.AFKTime);

        text_IridiumReward.text = NumberFormatter.FormatNumber(iridiumReward, FormattingTypes.Iridium);
        text_DarkElixirReward.text = NumberFormatter.FormatNumber(darkElixirReward, FormattingTypes.DarkElixir);

        afkReportUI.SetActive(true);
    }

    public void CloseAFKReportUI(int multiplier, double iridiumReward, double darkElixirReward)
    {
        if (!afkReportUI.activeSelf) return;

        button_NoThanks.onClick.RemoveAllListeners();
        button_RewardX2.onClick.RemoveAllListeners();
        button_RewardX3.onClick.RemoveAllListeners();

        gameManager.playerData.iridium_Current += iridiumReward * multiplier;
        gameManager.playerData.iridium_Total += iridiumReward * multiplier;

        gameManager.playerData.darkElixir_Current += darkElixirReward * multiplier;
        gameManager.playerData.darkElixir_Total += darkElixirReward * multiplier;

        afkReportUI.SetActive(false);
        OpenMainUI();
    }

    #endregion
}
