using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

[RequireComponent(typeof(LoadSaveSystem))]
public class GameManager : MonoBehaviour
{
    [Header("Tick Rate")]
    public static int ticksPerSecond = 30;

    [Header("Player Data")]
    public PlayerData playerData;


    [Header("Balancing")]
    [SerializeField] private double upgradeClick_BaseCost = 1000;
    [SerializeField] private double upgradeClick_PriceMultiplier = 1.2;
    [SerializeField] private double iridium_PerSecondBoostMultiplier = 2;
    [SerializeField] private double iridium_PerSecondBoostDuration = 10;

    [Header("Default Values SO")]
    [SerializeField] private DefaultValues defaultValues;

    public double upgradeClick_CurrentCost = 1000;
    private bool iridium_PerSecondBoosted = false;
    private bool getIridium_ButtonClicked = false;

    private Coroutine tickCoroutine;
    private Coroutine boostCoroutine;
    private WaitForSeconds tickWait;

    public Building selectedBuilding;
    private UIManager uiManager;
    private LoadSaveSystem loadSaveSystem;

    #region Utility Functions

    private void Awake()
    {
        loadSaveSystem = GetComponent<LoadSaveSystem>();
        uiManager = GetComponent<UIManager>();
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

        playerData.ownedBuildings = new List<Building>(FindObjectsOfType<Building>());

        UpdateIridiumPerSecond(); //Update the iridium per second

        uiManager.InitializeUI(); //Initialize all UI Variables

        CalculateCosts(); //Calculate all upgrade costs

        uiManager.UpdateAllUI(); //Update all UI

        StartTickCoroutine(); //Setup the coroutine for the tick rate
    }

    private void StartTickCoroutine()
    {
        if (tickCoroutine != null)
            StopCoroutine(tickCoroutine);

        tickWait = new WaitForSeconds(1.0f / ticksPerSecond);
        tickCoroutine = StartCoroutine(Tick());
    }

    private void UpdateIridiumPerSecond()
    {
        playerData.iridium_PerSecond = 0;

        foreach (Building b in playerData.ownedBuildings)
        {
            playerData.iridium_PerSecond += b.GetIridiumPerTick() * ticksPerSecond;
        }

        playerData.iridium_PerSecond = iridium_PerSecondBoosted ? playerData.iridium_PerSecond * iridium_PerSecondBoostMultiplier : playerData.iridium_PerSecond;
        playerData.iridium_PerClick = Math.Max(1, playerData.iridium_PerSecond * playerData.iridium_PerClickLevel / 100f);
    }

    private void CalculateCosts()
    {
        upgradeClick_CurrentCost = (int)(upgradeClick_BaseCost * Math.Pow(upgradeClick_PriceMultiplier, playerData.iridium_PerClickLevel - 1));
        playerData.iridium_PerClick = Math.Max(1, playerData.iridium_PerSecond * playerData.iridium_PerClickLevel / 100f);
        foreach (Building b in playerData.ownedBuildings)
        {
            foreach (Troop t in b.ownedTroops)
            {
                t.troop_CurrentCost = (int)(t.troop_BaseCost * Math.Pow(t.troop_CostMultiplier, t.troops_Owned));
            }
        }
    }

    private IEnumerator IridiumPerSecondBoost()
    {
        iridium_PerSecondBoosted = true;
        UpdateIridiumPerSecond();

        yield return new WaitForSeconds((float)iridium_PerSecondBoostDuration);

        iridium_PerSecondBoosted = false;
        UpdateIridiumPerSecond();
    }

    #endregion

    private IEnumerator Tick()
    {
        while (true)
        {
            ProcessIridiumAdded();
            uiManager.UpdateAllUI();
            yield return tickWait;
        }
    }

    private void ProcessIridiumAdded()
    {
        ProcessIridiumPerBuilding();

        if (getIridium_ButtonClicked)
        {
            ProcessClickedIridium();
            getIridium_ButtonClicked = false;
        }
    }

    #region Iridium Processors

    private void ProcessClickedIridium()
    {
        playerData.iridium_PerClick = Math.Max(1, playerData.iridium_PerSecond * playerData.iridium_PerClickLevel / 100f);
        playerData.iridium_Total += playerData.iridium_PerClick;
    }

    private void ProcessIridiumPerBuilding()
    {
        playerData.iridium_Total += playerData.iridium_PerSecond / ticksPerSecond;
    }

    #endregion

    #region Button Callbacks
    public void GetIridiumClicked()
    {
        getIridium_ButtonClicked = true;
    }

    public void UpgradeClickClicked()
    {
        if (playerData.iridium_Total >= upgradeClick_CurrentCost)
        {
            playerData.iridium_Total -= upgradeClick_CurrentCost;
            upgradeClick_CurrentCost = (int)(upgradeClick_CurrentCost * upgradeClick_PriceMultiplier);
            playerData.iridium_PerClickLevel += 1;
            playerData.iridium_PerClick = Math.Max(1, playerData.iridium_PerSecond * playerData.iridium_PerClickLevel / 100f);
        }
    }

    public void StructureBuyClicked(int structureIndex)
    {
        if (selectedBuilding == null)
        {
            Debug.LogError("No building selected, cannot buy structure.");
        }
        else
        {
            if (playerData.iridium_Total >= selectedBuilding.ownedTroops[structureIndex].troop_CurrentCost)
            {
                playerData.iridium_Total -= selectedBuilding.ownedTroops[structureIndex].troop_CurrentCost;
                selectedBuilding.ownedTroops[structureIndex].troops_Owned += 1;
                selectedBuilding.ownedTroops[structureIndex].troop_CurrentCost = (int)(selectedBuilding.ownedTroops[structureIndex].troop_CurrentCost * selectedBuilding.ownedTroops[structureIndex].troop_CostMultiplier);
            }
        }

        UpdateIridiumPerSecond();
    }

    public void BoostIridiumProductionClicked()
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
        loadSaveSystem.Save(playerData);
    }

    public PlayerData GetSaveData()
    {
        return playerData;
    }

    public void LoadGame()
    {
        playerData = loadSaveSystem.Load();

        StartTickCoroutine();
    }

    void Reset()
    {
        playerData.iridium_Total = defaultValues.iridium_Total;
        playerData.iridium_PerSecond = defaultValues.iridium_PerSecond;
        playerData.iridium_PerClick = defaultValues.iridium_PerClick;
        playerData.iridium_PerClickLevel = defaultValues.iridium_PerClickLevel;
        playerData.ownedBuildings = defaultValues.ownedBuildings;
    }

    #endregion

    public void ClickedOnBuilding(Building building)
    {
        if (building == null) return;

        if (building == selectedBuilding) return;

        selectedBuilding = building;
        uiManager.ClickedOnBuilding();
    }
}