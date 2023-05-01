using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

[RequireComponent(typeof(GameManager))]
public class UIManager : MonoBehaviour
{
    [Header("Main UI")]
    [SerializeField] private GameObject GameUI;
    [SerializeField] private TMP_Text text_TotalIridium;
    [SerializeField] private TMP_Text text_IridiumPerSecond;
    [SerializeField] private TMP_Text text_TotalDarkElixir;
    [SerializeField] private TMP_Text text_DarkElixirPerSecond;
    [SerializeField] private Button button_GetIridium;
    [SerializeField] private Button button_GetBoost;
    [SerializeField] private Button button_StocksMenu;
    [SerializeField] private Button button_UpgradeClick;
    [SerializeField] private Button button_OpenBuyBuildings;
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


    [Header("Building Buy UI")]
    /*private GameObject buildingBuyUI;
    private Button button_Back_BuyBuilding;
    private GameObject buildingButtonParent;
    private GameObject buildingButtonPrefab;

    private List<Button> button_Buildings;
    private List<TMP_Text> text_BuildingNames;
    private List<TMP_Text> text_BuildingsOwned;*/

    private List<TMP_Text> text_BuildingCosts;
    private List<Building> levelZeroBuildings;


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
    [SerializeField] private TMP_Text text_CurrentPrice;
    [SerializeField] private TMP_Text text_AmountToBuy;
    [SerializeField] private Button button_BuyStocks;
    [SerializeField] private Button button_Prev;
    [SerializeField] private Button button_MegaPrev;
    [SerializeField] private Button button_Next;
    [SerializeField] private Button button_MegaNext;
    [SerializeField] private TMP_Text text_TotalPrice;
    [SerializeField] private Button button_StocksBack;

    private GameManager gameManager;

    public void InitializeUI()
    {
        gameManager = GetComponent<GameManager>();

        InitializeMainUI();

        button_Back_Building.onClick.AddListener(CloseBuildingMenu);

        button_Back_Boost.onClick.AddListener(CloseBoostMenu);

        button_StocksBack.onClick.AddListener(CloseStocksMenu);

        button_newProfile.onClick.AddListener(OpenProfileCreatePanel);

        button_CreateProfile.onClick.AddListener(CreateProfile);
    }

    public void UpdateAllUI()
    {
        UpdateMainUI();
        UpdateBuildingCosts();

        if (buildingUI.activeSelf)
            UpdateBuildingUI();

        if (boostUI.activeSelf)
            UpdateBoostUI();

        if (stocksUI.activeSelf)
            UpdateStocksMenu();
    }

    public void CloseAllPanels()
    {
        ShowMainUI();
        CloseProfileCreatePanel();
        CloseProfileUI();
        CloseBuildingMenu();
        CloseBoostMenu();
    }

    #region Main UI

    public void ShowMainUI()
    {
        button_GetIridium.gameObject.SetActive(true);
        button_UpgradeClick.gameObject.SetActive(true);
        button_GetBoost.gameObject.SetActive(true);
    }

    public void OpenMainUI()
    {
        ShowMainUI();
        GameUI.SetActive(true);
    }

    private void InitializeMainUI()
    {
        text_GetIridiumButton = button_GetIridium.GetComponentInChildren<TMP_Text>();
        text_UpgradeClickButton = button_UpgradeClick.GetComponentInChildren<TMP_Text>();
        text_UpgradeBuildingButton = button_UpgradeBuilding.GetComponentInChildren<TMP_Text>();

        button_GetBoost.onClick.AddListener(OpenBoostMenu);

        button_StocksMenu.onClick.AddListener(OpenStocksMenu);

        button_UpgradeClick.onClick.AddListener(gameManager.UpgradeClickClicked);
    }

    public void InitializeBuildingCosts()
    {
        text_BuildingCosts = new List<TMP_Text>();
        levelZeroBuildings = new List<Building>();
        foreach (Building building in gameManager.BuildingManager.ownedBuildings)
        {
            if (building.buildingData.building_Level != 0)
            {
                continue;
            }

            levelZeroBuildings.Add(building);
            text_BuildingCosts.Add(building.transform.parent.GetChild(2).GetChild(1).GetComponent<TMP_Text>());
        }
    }

    private void UpdateMainUI()
    {
        text_TotalIridium.text = NumberFormatter.FormatNumber(gameManager.playerData.iridium_Current, FormattingTypes.Iridium);
        text_IridiumPerSecond.text = NumberFormatter.FormatNumber(gameManager.playerData.iridium_PerSecondBoosted, FormattingTypes.IridiumPerSecond) + " /s";

        text_TotalDarkElixir.text = NumberFormatter.FormatNumber(gameManager.playerData.darkElixir_Total, FormattingTypes.DarkElixer);
        text_DarkElixirPerSecond.text = NumberFormatter.FormatNumber(gameManager.playerData.darkElixir_PerSecond, FormattingTypes.DarkElixer);

        text_GetIridiumButton.text = "Get Iridium \n(+" + NumberFormatter.FormatNumber(gameManager.playerData.iridium_PerClickBoosted, FormattingTypes.IridiumPerSecond) + " Iridium)";

        text_UpgradeClickButton.text = "Upgrade Click (" + NumberFormatter.FormatNumber(gameManager.upgradeClick_CurrentCost, FormattingTypes.Cost) + ")";

        if (gameManager.playerData.iridium_Current < gameManager.upgradeClick_CurrentCost)
        {
            button_UpgradeClick.interactable = false;
        }
        else
        {
            button_UpgradeClick.interactable = true;
        }
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

    public void HideMainUI()
    {
        button_GetIridium.gameObject.SetActive(false);
        button_UpgradeClick.gameObject.SetActive(false);
        button_GetBoost.gameObject.SetActive(false);
    }

    #endregion

    #region Building UI

    public void OpenBuildingMenu()
    {
        boostUI.SetActive(false);
        buildingUI.SetActive(true);
        HideMainUI();
        PopulateBuildingUI();
    }

    public void PopulateBuildingUI()
    {
        CleanUpBuildingUI();

        if (gameManager.BuildingManager.selectedBuilding != null)
        {
            button_Troop = new List<Button>();
            text_TroopNames = new List<TMP_Text>();
            text_TroopCosts = new List<TMP_Text>();
            text_TroopsOwned = new List<TMP_Text>();
            text_TroopIPS = new List<TMP_Text>();

            button_UpgradeBuilding.onClick.AddListener(() => gameManager.BuildingManager.UpgradeBuilding(gameManager.BuildingManager.selectedBuilding));

            for (int i = 0; i < gameManager.BuildingManager.selectedBuilding.buildingData.building_OwnedTroops.Count; i++)
            {
                int j = i;

                GameObject newButton = Instantiate(troopButtonPrefab, troopButtonParent.transform);
                newButton.name = gameManager.BuildingManager.selectedBuilding.buildingData.building_OwnedTroops[i].troop_Name + " Button";

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
        text_BuildingName.text = gameManager.BuildingManager.selectedBuilding.buildingData.building_Name + " (Lvl " + NumberFormatter.FormatNumber(gameManager.BuildingManager.selectedBuilding.buildingData.building_Level, FormattingTypes.Level) + ")";
        text_BuildingIridiumPerSecond.text = NumberFormatter.FormatNumber(gameManager.BuildingManager.selectedBuilding.GetIridiumPerTick() * GameManager.ticksPerSecond, FormattingTypes.IridiumPerSecond) + " Iridium/s";

        if (gameManager.BuildingManager.selectedBuilding.buildingSO.building_UpgradeCosts[gameManager.BuildingManager.selectedBuilding.buildingData.building_Level] == -1)
        {
            button_UpgradeBuilding.gameObject.SetActive(false);
        }
        else
        {
            button_UpgradeBuilding.gameObject.SetActive(true);

            if (gameManager.playerData.iridium_Current < gameManager.BuildingManager.selectedBuilding.buildingSO.building_UpgradeCosts[gameManager.BuildingManager.selectedBuilding.buildingData.building_Level])
            {
                button_UpgradeBuilding.interactable = false;
            }
            else
            {
                button_UpgradeBuilding.interactable = true;
            }

            text_UpgradeBuildingButton.text = "Upgrade (" + NumberFormatter.FormatNumber(gameManager.BuildingManager.selectedBuilding.buildingSO.building_UpgradeCosts[gameManager.BuildingManager.selectedBuilding.buildingData.building_Level], FormattingTypes.Cost) + ")";
        }

        for (int i = 0; i < gameManager.BuildingManager.selectedBuilding.buildingData.building_OwnedTroops.Count; i++)
        {
            text_TroopNames[i].text = gameManager.BuildingManager.selectedBuilding.buildingData.building_OwnedTroops[i].troop_Name;

            text_TroopCosts[i].text = NumberFormatter.FormatNumber(gameManager.BuildingManager.selectedBuilding.buildingData.building_OwnedTroops[i].troop_CurrentCost, FormattingTypes.Cost);
            if (gameManager.playerData.iridium_Current < gameManager.BuildingManager.selectedBuilding.buildingData.building_OwnedTroops[i].troop_CurrentCost)
            {
                button_Troop[i].interactable = false;
            }
            else
            {
                button_Troop[i].interactable = true;
            }

            text_TroopsOwned[i].text = NumberFormatter.FormatNumber(gameManager.BuildingManager.selectedBuilding.buildingData.building_OwnedTroops[i].troops_Owned, FormattingTypes.Owned) + " owned";
            text_TroopIPS[i].text = "+" + NumberFormatter.FormatNumber(gameManager.BuildingManager.selectedBuilding.buildingData.building_OwnedTroops[i].GetIridiumPerTickPerTroop() * GameManager.ticksPerSecond, FormattingTypes.IridiumPerSecond) + "i/s";
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
        ShowMainUI();
        buildingUI.SetActive(false);
        gameManager.BuildingManager.selectedBuilding = null;
    }

    #endregion

    #region Boost UI

    public void OpenBoostMenu()
    {
        CloseAllPanels();
        HideMainUI();
        boostUI.SetActive(true);
        PopulateBoostUI();
    }

    public void PopulateBoostUI()
    {
        CleanUpBoostUI();

        button_Boosts = new List<Button>();
        text_BoostNames = new List<TMP_Text>();
        text_BoostDurations = new List<TMP_Text>();
        image_BoostDurationRemainings = new List<Image>();
        text_BoostDurationRemainings = new List<TMP_Text>();

        for (int i = 0; i < gameManager.BoostManager.boostSOs.Count; i++)
        {
            int j = i;

            GameObject newButton = Instantiate(boostButtonPrefab, boostButtonParent.transform);
            newButton.name = gameManager.BoostManager.boostSOs[i].boost_Name;

            button_Boosts.Add(newButton.transform.GetChild(2).GetComponent<Button>());
            text_BoostNames.Add(newButton.transform.GetChild(1).GetComponent<TMP_Text>());
            text_BoostDurations.Add(newButton.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>());
            image_BoostDurationRemainings.Add(newButton.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>());
            text_BoostDurationRemainings.Add(newButton.transform.GetChild(3).GetComponent<TMP_Text>());

            button_Boosts[i].onClick.AddListener(() => gameManager.BoostManager.AddBoost(gameManager.BoostManager.boostSOs[j]));
        }
    }

    private void UpdateBoostUI()
    {
        for (int i = 0; i < gameManager.BoostManager.boostSOs.Count; i++)
        {
            text_BoostNames[i].text = gameManager.BoostManager.boostSOs[i].boost_Name;

            text_BoostDurations[i].text = "+ " + NumberFormatter.FormatNumber(gameManager.BoostManager.boostSOs[i].boost_Duration, FormattingTypes.BoostDuration);

            Boost boost = Array.Find(gameManager.BoostManager.activeBoosts.ToArray(), x => x.boost_Name == gameManager.BoostManager.boostSOs[i].boost_Name);

            if (boost != null)
            {
                image_BoostDurationRemainings[i].fillAmount = (float)(boost.boost_TimeRemaining / gameManager.BoostManager.boostSOs[i].boost_MaxDuration);
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
        boostUI.SetActive(false);
        ShowMainUI();
        CleanUpBoostUI();
    }

    #endregion

    #region StocksUI

    public void OpenStocksMenu()
    {
        CloseAllPanels();
        HideMainUI();

        stocksUI.SetActive(true);

        button_BuyStocks.onClick.AddListener(gameManager.StockManager.BuyStocks);

        button_Prev.onClick.AddListener(gameManager.StockManager.PreviousButton);
        button_MegaPrev.onClick.AddListener(gameManager.StockManager.MegaPreviousButton);

        button_Next.onClick.AddListener(gameManager.StockManager.NextButton);
        button_MegaNext.onClick.AddListener(gameManager.StockManager.MegaNextButton);

        button_StocksBack.onClick.AddListener(CloseStocksMenu);
    }

    public void UpdateStocksMenu()
    {
        StockManager localSM = gameManager.StockManager;

        button_MegaPrev.interactable = (localSM.amountToBuy - localSM.megaPreviousStep) >= localSM.stockMinimumBuy;
        button_Prev.interactable = (localSM.amountToBuy - localSM.previousStep) >= localSM.stockMinimumBuy;

        button_BuyStocks.interactable = gameManager.playerData.iridium_Current >= localSM.totalPrice;

        button_Next.interactable = gameManager.playerData.iridium_Current >= localSM.totalPricePlusNext;
        button_MegaNext.interactable = gameManager.playerData.iridium_Current >= localSM.totalPricePlusMegaNext;

        text_CurrentPrice.text = NumberFormatter.FormatNumber(gameManager.StockManager.stockCurrentValue, FormattingTypes.Stocks);
        text_AmountToBuy.text = NumberFormatter.FormatNumber(gameManager.StockManager.amountToBuy, FormattingTypes.Iridium);
        text_TotalPrice.text = NumberFormatter.FormatNumber(gameManager.StockManager.totalPrice, FormattingTypes.IridiumPerSecond);
    }

    public void CloseStocksMenu()
    {
        button_BuyStocks.onClick.RemoveAllListeners();

        button_Prev.onClick.RemoveAllListeners();
        button_MegaPrev.onClick.RemoveAllListeners();

        button_Next.onClick.RemoveAllListeners();
        button_MegaNext.onClick.RemoveAllListeners();

        button_StocksBack.onClick.RemoveAllListeners();

        stocksUI.SetActive(false);
        ShowMainUI();
    }

    #endregion

    #region Profile Select UI

    public void OpenProfileSelect()
    {
        HideMainUI();
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
        ShowMainUI();
        profileSelectionUI.SetActive(false);
    }

    #endregion

    #region Profile Create UI

    public void OpenProfileCreatePanel()
    {
        CloseAllPanels();
        HideMainUI();
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
        ShowMainUI();
        profileCreationUI.SetActive(false);
    }

    #endregion

    #region Deprecated Code

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
