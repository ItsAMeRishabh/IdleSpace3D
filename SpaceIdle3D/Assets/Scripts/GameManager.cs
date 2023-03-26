using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

//[RequireComponent(typeof(LoadSaveSystem))]
public class GameManager : MonoBehaviour
{
    [Header("Profile Name")]
    public static string profileName = "Default";

    [Header("Tick Rate")]
    public static int ticksPerSecond = 30;

    [SerializeField] private List<Building> buildings = new List<Building>();
    private Building selectedBuilding;

    [Header("Balancing")]
    [SerializeField] private double iridiumPerSecondBoostDuration = 10;
    [SerializeField] private double iridiumPerSecondBoostMultiplier = 2;
    [SerializeField] private double upgradeClick_BaseCost = 1000;
    [SerializeField] private double iridiumPerClickPercent = 1;
    private double iridiumPerClick = 1;
    [SerializeField] private double clickUpgradePriceMultiplier = 1.2;
    private double upgradeClick_CurrentCost = 1000;

    [Header("Main UI")]
    [SerializeField] private GameObject GameUI;
    [SerializeField] private TMP_Text totalIridiumText;
    [SerializeField] private TMP_Text iridiumPerSecondText;
    [SerializeField] private Button getIridiumButton;
    [SerializeField] private Button boostIridiumPerSecond_Button;
    private TMP_Text getIridiumButtonText;
    [SerializeField] private Button upgradeClick_Button;
    private TMP_Text upgradeClick_ButtonText;

    [Header("Building UI")]
    [SerializeField] private GameObject buildingUI;
    [SerializeField] private TMP_Text buildingTotalIridiumText;
    [SerializeField] private TMP_Text buildingTotalIPSText;
    [SerializeField] private TMP_Text buildingNameText;
    [SerializeField] private TMP_Text buildingIPSText;
    [SerializeField] private Button backButton;

    [SerializeField] private GameObject troopButtonParent;
    [SerializeField] private GameObject troopButtonPrefab;

    private List<Button> structureButtons;
    private List<TMP_Text> structureNameTexts;
    private List<TMP_Text> structureCostTexts;
    private List<TMP_Text> structureOwnedTexts;
    private List<TMP_Text> structureIPSTexts;


    private bool iridiumPerSecondBoosted = false;
    private bool iridiumClicked = false;
    private double totalIridium = 0;
    private double iridiumPerSecond = 0;
    private string firstLaunchPlayerPref = "FirstLaunch";

    private Coroutine tickCoroutine;
    private Coroutine boostCoroutine;
    private WaitForSeconds tickWait;

    private LoadSaveSystem loadSaveSystem;

    #region Utility Functions

    private void Awake()
    {
        loadSaveSystem = GetComponent<LoadSaveSystem>();
    }

    private void Start()
    {
        StartGame();
    }

    private void OnDestroy()
    {
        //SaveGame();
    }

    private void StartGame()
    {

        buildings = new List<Building>(FindObjectsOfType<Building>());

        UpdateIridiumPerSecond(); //Update the iridium per second

        InitializeUI(); //Initialize all UI Variables

        CalculateCosts(); //Calculate all upgrade costs

        UpdateAllUI(); //Update all UI

        SetupCoroutine(); //Setup the coroutine for the tick rate
    }

    private void SetupCoroutine()
    {
        if (tickCoroutine != null)
            StopCoroutine(tickCoroutine);

        tickWait = new WaitForSeconds(1.0f / ticksPerSecond);
        tickCoroutine = StartCoroutine(Tick());
    }

    private void UpdateIridiumPerSecond()
    {
        iridiumPerSecond = 0;
        foreach (Building b in buildings)
        {
            iridiumPerSecond += b.GetIridiumPerTick() * ticksPerSecond;
        }
        iridiumPerSecond = iridiumPerSecondBoosted ? iridiumPerSecond * iridiumPerSecondBoostMultiplier : iridiumPerSecond;
        iridiumPerClick = Math.Max(1, iridiumPerSecond * iridiumPerClickPercent / 100f);
    }

    private void LoadBuildingSOs()
    {
        var os = Resources.LoadAll("Troops", typeof(TroopSO));

        foreach (var o in os)
        {
            Troop structure = new((TroopSO)o);
            //buildings.Add(structure);
        }

        UpdateIridiumPerSecond();
    }

    private void InitializeUI()
    {
        getIridiumButton.onClick.AddListener(GetIridiumClicked);
        upgradeClick_Button.onClick.AddListener(UpgradeClickClicked);
        backButton.onClick.AddListener(BackButtonClicked);
        boostIridiumPerSecond_Button.onClick.AddListener(BoostIridiumPerSecondClicked);
        getIridiumButtonText = getIridiumButton.transform.GetChild(0).GetComponent<TMP_Text>();
        upgradeClick_ButtonText = upgradeClick_Button.transform.GetChild(0).GetComponent<TMP_Text>();
    }

    private void CalculateCosts()
    {
        upgradeClick_CurrentCost = (int)(upgradeClick_BaseCost * Math.Pow(clickUpgradePriceMultiplier, iridiumPerClickPercent - 1));
        iridiumPerClick = Math.Max(1, iridiumPerSecond * iridiumPerClickPercent / 100f);
        foreach (Building b in buildings)
        {
            foreach (Troop t in b.ownedTroops)
            {
                t.structureCurrentCost = (int)(t.structureBaseCost * Math.Pow(t.structureCostMultiplier, t.structureOwned));
            }
        }
    }

    private void UpdateAllUI()
    {
        getIridiumButtonText.text = "Get Iridium \n(+" + iridiumPerClick + " Iridium)";
        iridiumPerSecondText.text = iridiumPerSecond.ToString("000.00") + " Iridium/s";
        totalIridiumText.text = totalIridium.ToString("0") + " Iridium";
        upgradeClick_ButtonText.text = "Upgrade Click ($" + upgradeClick_CurrentCost.ToString() + ")";

        if (selectedBuilding != null)
        {
            buildingNameText.text = selectedBuilding.buildingName + " (Lvl " + selectedBuilding.buildingLevel + ")";
            buildingIPSText.text = (selectedBuilding.GetIridiumPerTick() * ticksPerSecond).ToString() + " Iridium/s";
            buildingTotalIridiumText.text = totalIridium.ToString("0") + " Iridium";
            buildingTotalIPSText.text = iridiumPerSecond.ToString("0.00") + " Iridium/s";
            for (int i = 0; i < selectedBuilding.ownedTroops.Count; i++)
            {
                structureNameTexts[i].text = selectedBuilding.ownedTroops[i].structureName;
                structureCostTexts[i].text = "$" + selectedBuilding.ownedTroops[i].structureCurrentCost.ToString();
                structureOwnedTexts[i].text = selectedBuilding.ownedTroops[i].structureOwned.ToString() + " owned";
                structureIPSTexts[i].text = "+" + (selectedBuilding.ownedTroops[i].GetIridiumPerTick() * ticksPerSecond).ToString("0.0") + "i/s";
            }
        }
    }

    private void PopulateBuildingPanel()
    {
        if (selectedBuilding != null)
        {
            structureButtons = new List<Button>();
            structureNameTexts = new List<TMP_Text>();
            structureCostTexts = new List<TMP_Text>();
            structureOwnedTexts = new List<TMP_Text>();
            structureIPSTexts = new List<TMP_Text>();

            for (int i = 0; i < selectedBuilding.ownedTroops.Count; i++)
            {
                int j = i;

                GameObject newButton = Instantiate(troopButtonPrefab, troopButtonParent.transform);
                newButton.name = selectedBuilding.ownedTroops[i].structureName + " Button";
                structureButtons.Add(newButton.GetComponent<Button>());
                structureNameTexts.Add(newButton.transform.GetChild(0).GetComponent<TMP_Text>());
                structureCostTexts.Add(newButton.transform.GetChild(1).GetComponent<TMP_Text>());
                structureOwnedTexts.Add(newButton.transform.GetChild(2).GetComponent<TMP_Text>());
                structureIPSTexts.Add(newButton.transform.GetChild(3).GetComponent<TMP_Text>());

                structureButtons[i].onClick.AddListener(() => StructureBuyClicked(j));
            }
        }
    }

    void CleanUpPanel()
    {
        if (selectedBuilding == null)
            return;

        foreach (Button b in structureButtons)
        {
            b.onClick.RemoveAllListeners();
        }

        foreach (Button b in structureButtons)
        {
            Destroy(b.gameObject);
        }

        structureButtons.Clear();
        structureNameTexts.Clear();
        structureCostTexts.Clear();
        structureOwnedTexts.Clear();
        structureIPSTexts.Clear();
    }

    private IEnumerator IridiumPerSecondBoost()
    {
        iridiumPerSecondBoosted = true;
        UpdateIridiumPerSecond();
        yield return new WaitForSeconds((float)iridiumPerSecondBoostDuration);
        iridiumPerSecondBoosted = false;
        UpdateIridiumPerSecond();
    }

    #endregion

    private IEnumerator Tick()
    {
        while (true)
        {
            ProcessIridiumAdded();
            UpdateAllUI();
            yield return tickWait;
        }
    }

    private void ProcessIridiumAdded()
    {
        ProcessIridiumPerBuilding();

        if (iridiumClicked)
        {
            ProcessClickedIridium();
            iridiumClicked = false;
        }
    }

    #region Iridium Processors

    private void ProcessClickedIridium()
    {
        iridiumPerClick = Math.Max(1, iridiumPerSecond * iridiumPerClickPercent / 100f);
        totalIridium += iridiumPerClick;
        //UpdateAllUI();
    }

    private void ProcessIridiumPerBuilding()
    {

        totalIridium += iridiumPerSecond / ticksPerSecond;
        //UpdateAllUI();
    }

    #endregion

    #region Button Callbacks
    private void GetIridiumClicked()
    {
        iridiumClicked = true;
    }

    private void UpgradeClickClicked()
    {
        if (totalIridium >= upgradeClick_CurrentCost)
        {
            totalIridium -= upgradeClick_CurrentCost;
            upgradeClick_CurrentCost = (int)(upgradeClick_CurrentCost * clickUpgradePriceMultiplier);
            iridiumPerClickPercent += 1;
            iridiumPerClick = Math.Max(1, iridiumPerSecond * iridiumPerClickPercent / 100f);
        }

        //UpdateAllUI();
    }

    private void StructureBuyClicked(int structureIndex)
    {
        if (selectedBuilding == null)
        {
            Debug.LogError("No building selected, cannot buy structure.");
        }
        else
        {
            if (totalIridium >= selectedBuilding.ownedTroops[structureIndex].structureCurrentCost)
            {
                totalIridium -= selectedBuilding.ownedTroops[structureIndex].structureCurrentCost;
                selectedBuilding.ownedTroops[structureIndex].structureOwned += 1;
                selectedBuilding.ownedTroops[structureIndex].structureCurrentCost = (int)(selectedBuilding.ownedTroops[structureIndex].structureCurrentCost * selectedBuilding.ownedTroops[structureIndex].structureCostMultiplier);
            }
            //UpdateAllUI();
        }

        UpdateIridiumPerSecond();
    }

    private void BackButtonClicked()
    {
        if (selectedBuilding != null)
        {
            GameUI.SetActive(true);
            buildingUI.SetActive(false);

            CleanUpPanel();
            selectedBuilding = null;
        }
    }

    private void BoostIridiumPerSecondClicked()
    {
        if (boostCoroutine != null)
        {
            StopCoroutine(boostCoroutine);
            boostCoroutine = null;
        }

        boostCoroutine = StartCoroutine(IridiumPerSecondBoost());
    }

    #endregion

    #region Save, Load and Reset

    public void SaveGame()
    {
        SaveData saveData = GetSaveData();
        loadSaveSystem.Save(saveData);
    }

    public SaveData GetSaveData()
    {
        SaveData saveData = new SaveData();
        saveData.profileName = profileName;
        saveData.totalIridium = totalIridium;
        saveData.iridiumPerClickPercent = iridiumPerClickPercent;
        saveData.upgradeClick_BaseCost = upgradeClick_BaseCost;

        return saveData;
    }
    public void LoadGame()
    {
        SaveData saveData = loadSaveSystem.Load();

        profileName = saveData.profileName;
        totalIridium = saveData.totalIridium;
        iridiumPerClickPercent = saveData.iridiumPerClickPercent;
        upgradeClick_BaseCost = saveData.upgradeClick_BaseCost;

        SetupCoroutine();
        //UpdateAllUI();
    }

    void Reset()
    {
        totalIridium = 0;
        iridiumPerClickPercent = 1;
        upgradeClick_CurrentCost = upgradeClick_BaseCost;
    }

    #endregion

    public void ClickedOnBuilding(Building building)
    {
        if (building == null) return;

        if (building == selectedBuilding) return;

        CleanUpPanel();
        selectedBuilding = building;
        GameUI.SetActive(false);
        buildingUI.SetActive(true);
        PopulateBuildingPanel();
    }
}