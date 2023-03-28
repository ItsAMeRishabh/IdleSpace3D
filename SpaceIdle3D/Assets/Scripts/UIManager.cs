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
    private TMP_Text text_GetIridiumButton;
    private TMP_Text text_UpgradeClickButton;

    [Header("Building UI")]
    [SerializeField] private GameObject buildingUI;
    [SerializeField] private TMP_Text text_BuildingName;
    [SerializeField] private TMP_Text text_BuildingIridiumPerSecond;
    [SerializeField] private Button button_Back;

    [SerializeField] private GameObject troopButtonParent;
    [SerializeField] private GameObject troopButtonPrefab;

    private List<Button> structureButtons;
    private List<TMP_Text> structureNameTexts;
    private List<TMP_Text> structureCostTexts;
    private List<TMP_Text> structureOwnedTexts;
    private List<TMP_Text> structureIPSTexts;

    private GameManager gameManager;

    void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }

    public void InitializeUI()
    {
        text_GetIridiumButton = button_GetIridium.GetComponentInChildren<TMP_Text>();
        text_UpgradeClickButton = button_UpgradeClick.GetComponentInChildren<TMP_Text>();

        button_Back.onClick.AddListener(BackClicked);
        button_GetIridium.onClick.AddListener(gameManager.GetIridiumClicked);
        button_UpgradeClick.onClick.AddListener(gameManager.UpgradeClickClicked);
        button_BoostIridiumProduction.onClick.AddListener(gameManager.BoostIridiumProductionClicked);

        structureButtons = new List<Button>();
        structureNameTexts = new List<TMP_Text>();
        structureCostTexts = new List<TMP_Text>();
        structureOwnedTexts = new List<TMP_Text>();
        structureIPSTexts = new List<TMP_Text>();

        GameUI.SetActive(true);
        buildingUI.SetActive(false);
    }

    public void UpdateAllUI()
    {
        text_GetIridiumButton.text = "Get Iridium \n(+" + gameManager.playerData.iridium_PerClick.ToString("0") + " Iridium)";
        text_IridiumPerSecond.text = gameManager.playerData.iridium_PerSecond.ToString("0.0") + " Iridium/s";
        text_TotalIridium.text = gameManager.playerData.iridium_Total.ToString("0") + " Iridium";
        text_UpgradeClickButton.text = "Upgrade Click ($" + gameManager.upgradeClick_CurrentCost.ToString("0") + ")";

        if (gameManager.selectedBuilding != null)
        {
            text_BuildingName.text = gameManager.selectedBuilding.building_Name + " (Lvl " + gameManager.selectedBuilding.building_Level.ToString("0") + ")";
            text_BuildingIridiumPerSecond.text = (gameManager.selectedBuilding.GetIridiumPerTick() * GameManager.ticksPerSecond).ToString("0.0") + " Iridium/s";
            for (int i = 0; i < gameManager.selectedBuilding.ownedTroops.Count; i++)
            {
                structureNameTexts[i].text = gameManager.selectedBuilding.ownedTroops[i].troop_Name;
                structureCostTexts[i].text = "$" + gameManager.selectedBuilding.ownedTroops[i].troop_CurrentCost.ToString("0.0");
                structureOwnedTexts[i].text = gameManager.selectedBuilding.ownedTroops[i].troops_Owned.ToString("0") + " owned";
                structureIPSTexts[i].text = "+" + (gameManager.selectedBuilding.ownedTroops[i].GetIridiumPerTick() * GameManager.ticksPerSecond).ToString("0.0") + "i/s";
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
            structureButtons = new List<Button>();
            structureNameTexts = new List<TMP_Text>();
            structureCostTexts = new List<TMP_Text>();
            structureOwnedTexts = new List<TMP_Text>();
            structureIPSTexts = new List<TMP_Text>();

            for (int i = 0; i < gameManager.selectedBuilding.ownedTroops.Count; i++)
            {
                int j = i;

                GameObject newButton = Instantiate(troopButtonPrefab, troopButtonParent.transform);
                newButton.name = gameManager.selectedBuilding.ownedTroops[i].troop_Name + " Button";
                structureButtons.Add(newButton.GetComponent<Button>());
                structureNameTexts.Add(newButton.transform.GetChild(0).GetComponent<TMP_Text>());
                structureCostTexts.Add(newButton.transform.GetChild(1).GetComponent<TMP_Text>());
                structureOwnedTexts.Add(newButton.transform.GetChild(2).GetComponent<TMP_Text>());
                structureIPSTexts.Add(newButton.transform.GetChild(3).GetComponent<TMP_Text>());

                structureButtons[i].onClick.AddListener(() => gameManager.StructureBuyClicked(j));
            }
        }
    }

    public void CleanUpBuildingUI()
    {
        foreach (Button button in structureButtons)
        {
            button.onClick.RemoveAllListeners();
        }

        foreach (Button button in structureButtons)
        {
            Destroy(button.gameObject);
        }

        structureButtons.Clear();
        structureNameTexts.Clear();
        structureCostTexts.Clear();
        structureOwnedTexts.Clear();
        structureIPSTexts.Clear();
    }

    public void BackClicked()
    {
        //GameUI.SetActive(true);
        buildingUI.SetActive(false);
        gameManager.selectedBuilding = null;
    }

    public void OpenShop()
    {

    }

    public void CloseShop()
    {

    }
}
