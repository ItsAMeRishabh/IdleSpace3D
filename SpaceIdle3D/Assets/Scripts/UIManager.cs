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
    [SerializeField] private GameObject buildingBuyUI;
    [SerializeField] private Button button_Back_BuyBuilding;
    [SerializeField] private GameObject buildingButtonParent;
    [SerializeField] private GameObject buildingButtonPrefab;

    private List<Button> button_Buildings;
    private List<TMP_Text> text_BuildingNames;
    private List<TMP_Text> text_BuildingCosts;
    private List<TMP_Text> text_BuildingsOwned;



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

    private GameManager gameManager;

    void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }

    public void InitializeUI()
    {
        text_GetIridiumButton = button_GetIridium.GetComponentInChildren<TMP_Text>();
        text_UpgradeClickButton = button_UpgradeClick.GetComponentInChildren<TMP_Text>();
        text_UpgradeBuildingButton = button_UpgradeBuilding.GetComponentInChildren<TMP_Text>();

        button_Back_Building.onClick.AddListener(CloseBuildingMenu);

        button_Back_Boost.onClick.AddListener(CloseBoostMenu);
        button_GetBoost.onClick.AddListener(OpenBoostMenu);

        button_OpenBuyBuildings.onClick.AddListener(OpenShop);
        button_Back_BuyBuilding.onClick.AddListener(CloseShop);

        button_GetIridium.onClick.AddListener(gameManager.GetIridiumClicked);
        button_UpgradeClick.onClick.AddListener(gameManager.UpgradeClickClicked);

        button_newProfile.onClick.AddListener(OpenProfileNamePanel);
        button_Profiles = new List<Button>();
        text_ProfileNames = new List<TMP_Text>();

        button_CreateProfile.onClick.AddListener(CreateProfile);

        button_Troop = new List<Button>();
        text_TroopNames = new List<TMP_Text>();
        text_TroopCosts = new List<TMP_Text>();
        text_TroopsOwned = new List<TMP_Text>();
        text_TroopIPS = new List<TMP_Text>();

        button_Buildings = new List<Button>();
        text_BuildingNames = new List<TMP_Text>();
        text_BuildingCosts = new List<TMP_Text>();
        text_BuildingsOwned = new List<TMP_Text>();

        button_Boosts = new List<Button>();
        text_BoostNames = new List<TMP_Text>();
        text_BoostDurations = new List<TMP_Text>();
        text_BoostDurationRemainings = new List<TMP_Text>();
    }

    public void UpdateAllUI()
    {
        text_TotalIridium.text = NumberFormatter.FormatNumber(gameManager.playerData.iridium_Total, FormattingTypes.Iridium);
        text_IridiumPerSecond.text = NumberFormatter.FormatNumber(gameManager.playerData.iridium_PerSecondBoosted, FormattingTypes.IridiumPerSecond) + " /s";

        text_TotalDarkElixir.text = NumberFormatter.FormatNumber(gameManager.playerData.darkElixir_Total, FormattingTypes.DarkElixer);
        text_DarkElixirPerSecond.text = NumberFormatter.FormatNumber(gameManager.playerData.darkElixir_PerSecond, FormattingTypes.DarkElixer);

        text_GetIridiumButton.text = "Get Iridium \n(+" + NumberFormatter.FormatNumber(gameManager.playerData.iridium_PerClickBoosted, FormattingTypes.IridiumPerSecond) + " Iridium)";
        text_UpgradeClickButton.text = "Upgrade Click ($" + NumberFormatter.FormatNumber(gameManager.upgradeClick_CurrentCost, FormattingTypes.Cost) + ")";

        if (buildingUI.activeSelf)
        {
            text_BuildingName.text = gameManager.BuildingManager.selectedBuilding.buildingData.building_Name + " (Lvl " + NumberFormatter.FormatNumber(gameManager.BuildingManager.selectedBuilding.buildingData.building_Level, FormattingTypes.Level) + ")";
            text_BuildingIridiumPerSecond.text = NumberFormatter.FormatNumber(gameManager.BuildingManager.selectedBuilding.GetIridiumPerTick() * GameManager.ticksPerSecond, FormattingTypes.IridiumPerSecond) + " Iridium/s";

            if(gameManager.BuildingManager.selectedBuilding.buildingSO.building_CurrentUpgradeCost == -1)
            {
                text_UpgradeBuildingButton.text = "Max Level";
                button_UpgradeBuilding.gameObject.SetActive(false);
            }
            else
            {
                text_UpgradeBuildingButton.text = "Upgrade ($" + NumberFormatter.FormatNumber(gameManager.BuildingManager.selectedBuilding.buildingSO.building_CurrentUpgradeCost, FormattingTypes.Cost) + ")";
                button_UpgradeBuilding.gameObject.SetActive(true);
            }

            for (int i = 0; i < gameManager.BuildingManager.selectedBuilding.buildingData.building_OwnedTroops.Count; i++)
            {
                text_TroopNames[i].text = gameManager.BuildingManager.selectedBuilding.buildingData.building_OwnedTroops[i].troop_Name;
                text_TroopCosts[i].text = "$" + NumberFormatter.FormatNumber(gameManager.BuildingManager.selectedBuilding.buildingData.building_OwnedTroops[i].troop_CurrentCost, FormattingTypes.Cost);
                text_TroopsOwned[i].text = NumberFormatter.FormatNumber(gameManager.BuildingManager.selectedBuilding.buildingData.building_OwnedTroops[i].troops_Owned, FormattingTypes.Owned) + " owned";
                text_TroopIPS[i].text = "+" + NumberFormatter.FormatNumber(gameManager.BuildingManager.selectedBuilding.buildingData.building_OwnedTroops[i].GetIridiumPerTick() * GameManager.ticksPerSecond, FormattingTypes.IridiumPerSecond) + "i/s";
            }
        }

        if (buildingBuyUI.activeSelf)
        {
            for (int i = 0; i < gameManager.BuildingManager.buildingLocations.Count; i++)
            {
                text_BuildingNames[i].text = gameManager.BuildingManager.buildingLocations[i].buildingSO.building_Name;
                text_BuildingCosts[i].text = "$" + NumberFormatter.FormatNumber(gameManager.BuildingManager.buildingLocations[i].buildingSO.building_CurrentCost, FormattingTypes.Cost);

                text_BuildingsOwned[i].text = NumberFormatter.FormatNumber(gameManager.BuildingManager.GetBuildingCount(gameManager.BuildingManager.buildingLocations[i].buildingSO.building_Name), FormattingTypes.Owned) + " owned";
            }
        }

        if (boostUI.activeSelf)
        {
            for (int i = 0; i < gameManager.BoostManager.boostSOs.Count; i++)
            {
                text_BoostNames[i].text = gameManager.BoostManager.boostSOs[i].boost_Name;

                text_BoostDurations[i].text = NumberFormatter.FormatNumber(gameManager.BoostManager.boostSOs[i].boost_Duration, FormattingTypes.BoostDuration) + " Sec";

                Boost boost = Array.Find(gameManager.BoostManager.activeBoosts.ToArray(), x => x.boost_Name == gameManager.BoostManager.boostSOs[i].boost_Name);
                if (boost != null)
                {
                    text_BoostDurationRemainings[i].text = NumberFormatter.FormatNumber(boost.boost_TimeRemaining, FormattingTypes.BoostDuration) + " Sec Left";
                }
                else
                {
                    text_BoostDurationRemainings[i].text = NumberFormatter.FormatNumber(0.0d, FormattingTypes.BoostDuration) + " Sec Left";
                }
            }
        }
    }

    public void PopulateProfileSelectUI(List<string> profilesList)
    {
        CleanUpProfileSelectUI();

        OpenProfileSelect();

        button_Profiles = new List<Button>();
        text_ProfileNames = new List<TMP_Text>();

        for(int i = 0; i < profilesList.Count; i++)
        {
            int j = i;
            GameObject newButton = Instantiate(profileButtonPrefab, profileButtonParent.transform);
            newButton.name = profilesList[i];

            Button button = newButton.GetComponent<Button>();
            TMP_Text tmp_text = newButton.GetComponentInChildren<TMP_Text>();

            button_Profiles.Add(button);
            text_ProfileNames.Add(tmp_text);

            button.onClick.AddListener(()=> gameManager.LoadGame(profilesList[j]));
            tmp_text.text = profilesList[i];
        }
    }

    public void PopulateBoostUI()
    {
        CleanUpBoostUI();

        button_Boosts = new List<Button>();
        text_BoostNames = new List<TMP_Text>();
        text_BoostDurations = new List<TMP_Text>();
        text_BoostDurationRemainings = new List<TMP_Text>();

        for (int i = 0; i < gameManager.BoostManager.boostSOs.Count; i++)
        {
            int j = i;

            GameObject newButton = Instantiate(boostButtonPrefab, boostButtonParent.transform);
            newButton.name = gameManager.BoostManager.boostSOs[i].boost_Name;

            button_Boosts.Add(newButton.GetComponent<Button>());
            text_BoostNames.Add(newButton.transform.GetChild(0).GetComponent<TMP_Text>());
            text_BoostDurations.Add(newButton.transform.GetChild(1).GetComponent<TMP_Text>());
            text_BoostDurationRemainings.Add(newButton.transform.GetChild(2).GetComponent<TMP_Text>());

            button_Boosts[i].onClick.AddListener(() => gameManager.BoostManager.AddBoost(gameManager.BoostManager.boostSOs[j]));
        }
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
                button_Troop.Add(newButton.GetComponent<Button>());
                text_TroopNames.Add(newButton.transform.GetChild(0).GetComponent<TMP_Text>());
                text_TroopCosts.Add(newButton.transform.GetChild(1).GetComponent<TMP_Text>());
                text_TroopsOwned.Add(newButton.transform.GetChild(2).GetComponent<TMP_Text>());
                text_TroopIPS.Add(newButton.transform.GetChild(3).GetComponent<TMP_Text>());

                button_Troop[i].onClick.AddListener(() => gameManager.TroopBuyClicked(j));
            }
        }
    }

    public void PopulateBuyBuildingUI()
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
    }

    public void CleanUpProfileSelectUI()
    {
        foreach(Button button in button_Profiles)
        {
            button.onClick.RemoveAllListeners();
        }

        foreach(Button button in button_Profiles)
        {
            Destroy(button.gameObject);
        }

        button_Profiles.Clear();
        text_ProfileNames.Clear();
    }

    public void CleanUpBoostUI()
    {
        foreach (Button button in button_Boosts)
        {
            button.onClick.RemoveAllListeners();
        }

        foreach (Button button in button_Boosts)
        {
            Destroy(button.gameObject);
        }

        button_Boosts.Clear();
        text_BoostNames.Clear();
        text_BoostDurations.Clear();
        text_BoostDurationRemainings.Clear();
    }

    public void CleanUpBuildingUI()
    {
        button_UpgradeBuilding.onClick.RemoveAllListeners();

        foreach (Button button in button_Troop)
        {
            button.onClick.RemoveAllListeners();
        }

        foreach (Button button in button_Troop)
        {
            Destroy(button.gameObject);
        }

        button_Troop.Clear();
        text_TroopNames.Clear();
        text_TroopCosts.Clear();
        text_TroopsOwned.Clear();
        text_TroopIPS.Clear();
    }

    public void CleanUpBuyBuildingUI()
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
        text_BuildingNames.Clear();
        text_BuildingCosts.Clear();
        text_BuildingsOwned.Clear();
    }

    public void CloseAllPanels()
    {
        ShowMainUI();
        CLoseProfileNamePanel();
        CloseProfileUI();
        CloseBuildingMenu();
        CloseShop();
        CloseBoostMenu();
    }

    public void OpenMainUI()
    {
        ShowMainUI();
        GameUI.SetActive(true);
    }

    public void OpenProfileSelect()
    {
        HideMainUI();
        profileSelectionUI.SetActive(true);
    }

    public void OpenProfileNamePanel()
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
            CLoseProfileNamePanel();
        }
        else
        {
            Debug.LogError("Profile name empty!");
        }
    }

    public void OpenBuildingMenu()
    {
        boostUI.SetActive(false);
        buildingBuyUI.SetActive(false);
        buildingUI.SetActive(true);
        HideMainUI();
        PopulateBuildingUI();
    }

    public void CloseBuildingMenu()
    {
        ShowMainUI();
        buildingUI.SetActive(false);
        gameManager.BuildingManager.selectedBuilding = null;
    }

    public void OpenShop()
    {
        CloseAllPanels();
        HideMainUI();
        buildingBuyUI.SetActive(true);
        PopulateBuyBuildingUI();
    }

    public void CloseProfileUI()
    {
        ShowMainUI();
        profileSelectionUI.SetActive(false);
    }

    public void CLoseProfileNamePanel()
    {
        ShowMainUI();
        profileCreationUI.SetActive(false);
    }

    public void CloseShop()
    {
        ShowMainUI();
        CleanUpBuyBuildingUI();
        buildingBuyUI.SetActive(false);
    }

    public void OpenBoostMenu()
    {
        CloseAllPanels();
        HideMainUI();
        boostUI.SetActive(true);
        PopulateBoostUI();
    }

    public void CloseBoostMenu()
    {
        boostUI.SetActive(false);
        ShowMainUI();
        CleanUpBoostUI();
    }

    public void HideMainUI()
    {
        button_GetIridium.gameObject.SetActive(false);
        button_UpgradeClick.gameObject.SetActive(false);
        button_OpenBuyBuildings.gameObject.SetActive(false);
        button_GetBoost.gameObject.SetActive(false);
    }

    public void ShowMainUI()
    {
        button_GetIridium.gameObject.SetActive(true);
        button_UpgradeClick.gameObject.SetActive(true);
        button_OpenBuyBuildings.gameObject.SetActive(true);
        button_GetBoost.gameObject.SetActive(true);
    }
}
