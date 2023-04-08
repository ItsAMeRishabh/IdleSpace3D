using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(GameManager))]
public class UIManager : MonoBehaviour
{
    [Header("Main UI")]
    [SerializeField] private GameObject GameUI;
    [SerializeField] private TMP_Text text_TotalIridium;
    [SerializeField] private TMP_Text text_IridiumPerSecond;
    [SerializeField] private Button button_GetIridium;
    [SerializeField] private Button button_BoostIridiumProduction;
    [SerializeField] private Button button_UpgradeClick;
    [SerializeField] private Button button_OpenBuyBuildings;
    private TMP_Text text_GetIridiumButton;
    private TMP_Text text_UpgradeClickButton;

    [Header("Building UI")]
    [SerializeField] private GameObject buildingUI;
    [SerializeField] private TMP_Text text_BuildingName;
    [SerializeField] private TMP_Text text_BuildingIridiumPerSecond;
    [SerializeField] private Button button_Back_Building;

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
    [SerializeField] private Button newProfileButton;

    [SerializeField] private GameObject profileButtonParent;
    [SerializeField] private GameObject profileButtonPrefab;

    private List<Button> button_Profiles;
    private List<TMP_Text> text_ProfileNames;

    private GameManager gameManager;

    public GameObject BuildingUI => buildingUI;
    public GameObject BuildingBuyUI => buildingBuyUI;

    void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }

    public void InitializeUI()
    {
        text_GetIridiumButton = button_GetIridium.GetComponentInChildren<TMP_Text>();
        text_UpgradeClickButton = button_UpgradeClick.GetComponentInChildren<TMP_Text>();

        button_Back_Building.onClick.AddListener(BackClicked);
        button_OpenBuyBuildings.onClick.AddListener(OpenShop);
        button_Back_BuyBuilding.onClick.AddListener(CloseShop);
        button_GetIridium.onClick.AddListener(gameManager.GetIridiumClicked);
        button_UpgradeClick.onClick.AddListener(gameManager.UpgradeClickClicked);
        button_BoostIridiumProduction.onClick.AddListener(gameManager.BoostIridiumProductionClicked);

        button_Troop = new List<Button>();
        text_TroopNames = new List<TMP_Text>();
        text_TroopCosts = new List<TMP_Text>();
        text_TroopsOwned = new List<TMP_Text>();
        text_TroopIPS = new List<TMP_Text>();

        button_Buildings = new List<Button>();
        text_BuildingNames = new List<TMP_Text>();
        text_BuildingCosts = new List<TMP_Text>();
        text_BuildingsOwned = new List<TMP_Text>();

        GameUI.SetActive(true);
        buildingUI.SetActive(false);
    }

    public void UpdateAllUI()
    {
        text_GetIridiumButton.text = "Get Iridium \n(+" + gameManager.playerData.iridium_PerClick.ToString("0") + " Iridium)";
        text_IridiumPerSecond.text = gameManager.playerData.iridium_PerSecond.ToString("0.0") + " Iridium/s";
        text_TotalIridium.text = gameManager.playerData.iridium_Total.ToString("0") + " Iridium";
        text_UpgradeClickButton.text = "Upgrade Click ($" + gameManager.upgradeClick_CurrentCost.ToString("0") + ")";

        if (buildingUI.activeSelf)
        {
            text_BuildingName.text = gameManager.selectedBuilding.buildingData.building_Name + " (Lvl " + gameManager.selectedBuilding.buildingData.building_Level.ToString("0") + ")";
            text_BuildingIridiumPerSecond.text = (gameManager.selectedBuilding.GetIridiumPerTick() * GameManager.ticksPerSecond).ToString("0.0") + " Iridium/s";
            for (int i = 0; i < gameManager.selectedBuilding.buildingData.ownedTroops.Count; i++)
            {
                text_TroopNames[i].text = gameManager.selectedBuilding.buildingData.ownedTroops[i].troop_Name;
                text_TroopCosts[i].text = "$" + gameManager.selectedBuilding.buildingData.ownedTroops[i].troop_CurrentCost.ToString("0.0");
                text_TroopsOwned[i].text = gameManager.selectedBuilding.buildingData.ownedTroops[i].troops_Owned.ToString("0") + " owned";
                text_TroopIPS[i].text = "+" + (gameManager.selectedBuilding.buildingData.ownedTroops[i].GetIridiumPerTick() * GameManager.ticksPerSecond).ToString("0.0") + "i/s";
            }
        }

        if (buildingBuyUI.activeSelf)
        {
            for (int i = 0; i < gameManager.BuildingManager.buildingLocations.Count; i++)
            {
                text_BuildingNames[i].text = gameManager.BuildingManager.buildingLocations[i].buildingSO.building_Name;
                text_BuildingCosts[i].text = "$" + gameManager.BuildingManager.buildingLocations[i].buildingSO.building_BaseCost.ToString("0.0");

                text_BuildingsOwned[i].text = gameManager.BuildingManager.GetBuildingCount(gameManager.BuildingManager.buildingLocations[i].buildingSO.building_Name).ToString("0") + " owned";
            }
        }
    }

    public void ClickedOnBuilding()
    {
        //GameUI.SetActive(false);
        buildingUI.SetActive(true);
        PopulateBuildingUI();
    }

    public void PopulateBuildingUI()
    {
        CleanUpBuildingUI();

        if (gameManager.selectedBuilding != null)
        {
            button_Troop = new List<Button>();
            text_TroopNames = new List<TMP_Text>();
            text_TroopCosts = new List<TMP_Text>();
            text_TroopsOwned = new List<TMP_Text>();
            text_TroopIPS = new List<TMP_Text>();

            for (int i = 0; i < gameManager.selectedBuilding.buildingData.ownedTroops.Count; i++)
            {
                int j = i;

                GameObject newButton = Instantiate(troopButtonPrefab, troopButtonParent.transform);
                newButton.name = gameManager.selectedBuilding.buildingData.ownedTroops[i].troop_Name + " Button";
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

        BuildingLocations[] buildings = gameManager.BuildingManager.buildingLocations.ToArray();

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

    public void CleanUpBuildingUI()
    {
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

    public void BackClicked()
    {
        //GameUI.SetActive(true);
        buildingUI.SetActive(false);
        gameManager.selectedBuilding = null;
    }

    public void OpenShop()
    {
        buildingBuyUI.SetActive(true);
        PopulateBuyBuildingUI();
    }

    public void CloseShop()
    {
        CleanUpBuyBuildingUI();
        buildingBuyUI.SetActive(false);
    }

    public void GetProfileName()
    {

    }

    public void PrepareProfileList()
    {

    }

    public string ProfileSelected(string profileName)
    {
        return null;
    }
}
