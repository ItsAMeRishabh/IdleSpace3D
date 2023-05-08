using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Unity.VisualScripting;

[RequireComponent(typeof(GameManager))]
public class BuildingManager : MonoBehaviour
{
    [HideInInspector] public Building selectedBuilding;
    [HideInInspector] public int selectedBuildingIndex;

    public List<BuildingLocation> buildingLocations;
    [HideInInspector] public List<Building> ownedBuildings = new List<Building>();

    private Dictionary<Transform, Building> buildingLocationsDict = new Dictionary<Transform, Building>();
    private GameManager gameManager;

    public void WakeUp()
    {
        gameManager = GetComponent<GameManager>();
    }

    public void StartGame()
    {
        CalculateNonSerializedTroop();
        InitializeNewBuildings();

        foreach (Building building in ownedBuildings)
        {
            building.CheckSpawnConditions();
        }
    }

    public void ClickedOnBuilding(Building building)
    {
        if (building == null) return;

        if (building == selectedBuilding) return;

        if (building.buildingData.building_Level == 0)
        {
            UpgradeBuilding(building);
            selectedBuilding = null;
        }
        else
        {
            selectedBuilding = building;
            selectedBuildingIndex = GetBuildingIndex(building);
            gameManager.UIManagerRef.OpenBuildingMenu();
        }
    }

    public int GetBuildingIndex(Building building)
    {
        for (int i = 0; i < ownedBuildings.Count; i++)
        {
            if (ownedBuildings[i] == building)
                return i;
        }

        return -1;
    }

    public void NextBuilding()
    {
        if (selectedBuilding == null) return;

        selectedBuildingIndex = (selectedBuildingIndex + 1) % ownedBuildings.Count;

        while (ownedBuildings[selectedBuildingIndex].buildingData.building_Level == 0)
        {
            selectedBuildingIndex = (selectedBuildingIndex + 1) % ownedBuildings.Count;
        }

        ClickedOnBuilding(ownedBuildings[selectedBuildingIndex]);
    }

    public void PreviousBuilding()
    {
        if (selectedBuilding == null) return;
        selectedBuildingIndex = (selectedBuildingIndex - 1 + ownedBuildings.Count) % ownedBuildings.Count;

        while (ownedBuildings[selectedBuildingIndex].buildingData.building_Level == 0)
        {
            selectedBuildingIndex = (selectedBuildingIndex - 1 + ownedBuildings.Count) % ownedBuildings.Count;
        }

        ClickedOnBuilding(ownedBuildings[selectedBuildingIndex]);
    }

    public void InitializeNewBuildings()
    {
        foreach (BuildingLocation bLo in buildingLocations)
        {
            BuildingSO currentBSO = bLo.buildingSO;

            foreach (Transform spawnPoint in bLo.buildingLocations)
            {
                if (!buildingLocationsDict.ContainsKey(spawnPoint))
                {
                    PlaceBuilding(currentBSO);
                }
            }
        }
    }

    public void CalculateNonSerializedTroop()
    {
        List<BuildingData> ownedBuildingDatas = gameManager.playerData.ownedBuildings;

        for (int i = 0; i < ownedBuildingDatas.Count; i++)
        {
            BuildingSO currentBuildingSO = gameManager.BuildingManagerRef.GetBuildingSO(ownedBuildingDatas[i].building_Name);

            for (int j = 0; j < ownedBuildingDatas[i].building_OwnedTroops.Count; j++)
            {
                TroopSO currentTroop = gameManager.BuildingManagerRef.GetTroopSO(currentBuildingSO, ownedBuildingDatas[i].building_OwnedTroops[j].troop_Name);

                ownedBuildingDatas[i].building_OwnedTroops[j] = new Troop(currentTroop, ownedBuildingDatas[i].building_OwnedTroops[j]);
            }
        }

        SpawnBuildings(ownedBuildingDatas);
    }

    public List<BuildingData> GetBuildingDataList()
    {
        List<BuildingData> buildingDatas = new List<BuildingData>();

        foreach (Building building in ownedBuildings)
        {
            buildingDatas.Add(building.buildingData);
        }

        return buildingDatas;
    }

    public void UpdateCosts()
    {
        foreach (BuildingLocation location in buildingLocations)
        {
            BuildingSO currentBuidling = location.buildingSO;

            currentBuidling.building_UpgradeCosts[0] = currentBuidling.building_BaseCost * Math.Pow(currentBuidling.building_CostMultiplier, GetBuildingCount(currentBuidling.building_Name));
        }

        foreach (Building building in ownedBuildings)
        {
            for (int i = 0; i < building.buildingData.building_OwnedTroops.Count; i++)
            {
                building.buildingData.building_OwnedTroops[i].troop_CurrentCost = building.buildingData.building_OwnedTroops[i].troop_BaseCost * Math.Pow(building.buildingData.building_OwnedTroops[i].troop_CostMultiplier, building.buildingData.building_OwnedTroops[i].troops_Owned);
                building.buildingData.building_OwnedTroops[i].troop_CurrentUpgradeCost = building.buildingData.building_OwnedTroops[i].troop_BaseUpgradeCost * Math.Pow(building.buildingData.building_OwnedTroops[i].troop_UpgradeCostMultiplier, building.buildingData.building_OwnedTroops[i].troop_Level - 1);
            }
        }
    }

    public void SpawnBuildings(List<BuildingData> buildingDatas)
    {
        foreach (BuildingData buildingData in buildingDatas)
        {
            BuildingSO buildingSO = null;

            foreach (BuildingLocation location in buildingLocations)
            {
                if (location.buildingSO.building_Name.Equals(buildingData.building_Name))
                {
                    buildingSO = location.buildingSO;
                    break;
                }
            }

            if (buildingSO != null)
            {
                PlaceBuilding(buildingSO, buildingData);
            }
            else
            {
                Debug.LogError("BuildingSO not found for " + buildingData.building_Name);
            }
        }
    }

    public bool PlaceBuilding(BuildingSO buildingSO)
    {
        BuildingLocation BLS = Array.Find(buildingLocations.ToArray(), x => x.buildingSO == buildingSO);

        if (BLS == null)
        {
            Debug.LogError("Building not found for " + buildingSO.building_Name);
            return false;
        }

        foreach (Transform t in BLS.buildingLocations)
        {
            if (!buildingLocationsDict.ContainsKey(t))
            {
                GameObject obj = Instantiate(buildingSO.buildingPrefabs[buildingSO.building_Level], t);
                Building building = obj.GetComponentInChildren<Building>();

                building.buildingSO = buildingSO;
                building.buildingData = new BuildingData();
                building.buildingData.building_Name = buildingSO.building_Name;
                building.buildingData.building_Level = buildingSO.building_Level;
                building.buildingData.building_OwnedTroops = new List<Troop>();

                foreach (LevelUpUnlocks levelUpUnlock in buildingSO.levelUpUnlocks)
                {
                    if (building.buildingData.building_Level >= levelUpUnlock.level)
                    {
                        List<Troop> troopsToAdd = new List<Troop>();

                        foreach (TroopSO troop in levelUpUnlock.unlockedTroops)
                        {
                            troopsToAdd.Add(new Troop(troop));
                        }

                        foreach (Troop troop in troopsToAdd)
                        {
                            building.buildingData.building_OwnedTroops.Add(troop);
                        }
                    }
                }
                building.Initialize();

                ownedBuildings.Add(building);
                buildingLocationsDict.Add(t, building);

                gameManager.UIManagerRef.PopulateBuildingUI();
                gameManager.UpdateCosts();
                return true;
            }
        }

        Debug.LogError("No free building locations found for " + buildingSO.building_Name);
        return false;
    }

    public Building PlaceBuilding(BuildingSO buildingSO, BuildingData buildingData)
    {
        BuildingLocation BLS = Array.Find(buildingLocations.ToArray(), x => x.buildingSO == buildingSO);

        if (BLS == null)
        {
            Debug.LogError("Building not found for " + buildingSO.building_Name);
            return null;
        }

        foreach (Transform t in BLS.buildingLocations)
        {
            if (!buildingLocationsDict.ContainsKey(t))
            {
                GameObject obj = Instantiate(buildingSO.buildingPrefabs[buildingData.building_Level], t);
                Building building = obj.GetComponentInChildren<Building>();

                building.buildingSO = buildingSO;
                building.buildingData = buildingData;

                foreach (LevelUpUnlocks levelUpUnlock in buildingSO.levelUpUnlocks)
                {
                    if (building.buildingData.building_Level >= levelUpUnlock.level)
                    {
                        List<Troop> troopsToAdd = new List<Troop>();

                        foreach (TroopSO troop in levelUpUnlock.unlockedTroops)
                        {
                            Troop foundTroop = Array.Find(building.buildingData.building_OwnedTroops.ToArray(), x => x.troop_Name == troop.troop_Name);

                            if (foundTroop != null)
                            {
                                continue;
                            }
                            else
                            {
                                troopsToAdd.Add(new Troop(troop));
                            }
                        }

                        foreach (Troop troop in troopsToAdd)
                        {
                            building.buildingData.building_OwnedTroops.Add(troop);
                        }
                    }
                }

                building.Initialize();

                ownedBuildings.Add(building);
                buildingLocationsDict.Add(t, building);

                gameManager.UIManagerRef.PopulateBuildingUI();
                gameManager.UpdateCosts();
                return building;
            }
        }

        return null;
    }

    public bool UpgradeBuilding(Building building)
    {
        BuildingSO buildingSO = building.buildingSO;

        if (buildingSO == null)
        {
            Debug.LogError("BuildingSO not found for " + building.buildingData.building_Name);
            return false;
        }

        if (building.buildingData.building_Level == buildingSO.buildingPrefabs.Count)
        {
            Debug.LogError("Building is already at max level");
            return false;
        }

        if (gameManager.playerData.iridium_Current >= building.buildingSO.building_UpgradeCosts[building.buildingData.building_Level])
        {
            if (building.buildingData.building_Level == 0) gameManager.AudioManagerRef.Play("PoofSound");
            else gameManager.AudioManagerRef.Play("BuildingUpgrades");

            gameManager.playerData.iridium_Current -= building.buildingSO.building_UpgradeCosts[building.buildingData.building_Level];
            Transform buildingTransform = buildingLocationsDict.FirstOrDefault(x => x.Value == building).Key;

            ownedBuildings.Remove(building);
            buildingLocationsDict.Remove(buildingTransform);
            BuildingData buildingData = building.buildingData;
            buildingData.building_Level++;
            Destroy(building.transform.parent.gameObject);

            selectedBuilding = PlaceBuilding(buildingSO, buildingData);
            selectedBuilding.SpawnUpgradeEffect(buildingSO.upgradeVisualFX[buildingData.building_Level - 1]);
            gameManager.UpdateResourceSources();
            gameManager.UpdateCosts();
            gameManager.UIManagerRef.UpdateLevelZeroBuildingList();
            return true;
        }
        else
        {
            Debug.Log("Not enough iridium to buy building!");
            return false;
        }

    }

    public int GetBuildingCount(string buildingName)
    {
        int count = 0;

        foreach (Building building in ownedBuildings)
        {
            if (building.buildingData.building_Name.Equals(buildingName) && building.buildingData.building_Level > 0)
            {
                count++;
            }
        }

        return count;
    }

    public BuildingSO GetBuildingSO(string buildingName)
    {
        BuildingLocation BLS = Array.Find(buildingLocations.ToArray(), x => x.buildingSO.building_Name == buildingName);

        return BLS.buildingSO;
    }

    public TroopSO GetTroopSO(string buildingName, string troopName)
    {
        BuildingSO bSO = GetBuildingSO(buildingName);

        if (bSO == null)
        {
            Debug.LogError($"Building \"{buildingName}\" not found!");
            return null;
        }

        return GetTroopSO(bSO, troopName);
    }

    public TroopSO GetTroopSO(BuildingSO buildingSO, string troopName)
    {
        TroopSO tSO = null;

        foreach (LevelUpUnlocks luu in buildingSO.levelUpUnlocks)
        {
            tSO = Array.Find(luu.unlockedTroops.ToArray(), x => x.troop_Name == troopName);

            if (tSO != null)
            {
                return tSO;
            }
        }
        return tSO;
    }

    public void TroopBuyClicked(int troopIndex)
    {
        if (selectedBuilding == null)
        {
            Debug.LogError("No building selected, cannot buy troop.");
        }
        else
        {
            if (gameManager.playerData.iridium_Current >= selectedBuilding.buildingData.building_OwnedTroops[troopIndex].troop_CurrentCost)
            {
                gameManager.playerData.iridium_Current -= selectedBuilding.buildingData.building_OwnedTroops[troopIndex].troop_CurrentCost;
                selectedBuilding.buildingData.building_OwnedTroops[troopIndex].troops_Owned += 1;
                selectedBuilding.buildingData.building_OwnedTroops[troopIndex].troop_CurrentCost = (int)(selectedBuilding.buildingData.building_OwnedTroops[troopIndex].troop_CurrentCost * selectedBuilding.buildingData.building_OwnedTroops[troopIndex].troop_CostMultiplier);
            }

            switch (selectedBuilding.buildingData.building_Name)
            {
                case "TERRA MINE": break;
                case "LASERVATORY": selectedBuilding.CheckLaservatorySpawn(); break;
                case "MEGATRON DOCK": selectedBuilding.CheckMegatronSpawn(); break;
                case "VAC3000": selectedBuilding.CheckBlackHoleSpawn(); break;
                case "BOBO'S HUT": break;
                default: Debug.LogError($"Unknown Building {selectedBuilding.buildingData.building_Name}!"); break;
            }
        }

        gameManager.UpdateResourceSources();
        gameManager.UpdateCosts();
    }

    public void TroopUpgradeClicked(int troopIndex)
    {
        if (selectedBuilding == null)
        {
            Debug.LogError("No building selected, cannot upgrade troop");
        }
        else
        {
            if (gameManager.playerData.darkElixir_Current >= selectedBuilding.buildingData.building_OwnedTroops[troopIndex].troop_CurrentUpgradeCost)
            {
                gameManager.playerData.darkElixir_Current -= selectedBuilding.buildingData.building_OwnedTroops[troopIndex].troop_CurrentUpgradeCost;
                selectedBuilding.buildingData.building_OwnedTroops[troopIndex].troop_Level += 1;
            }
        }
        gameManager.UpdateResourceSources();
        gameManager.UpdateCosts();
    }

    public void BuildingBuyClicked(BuildingSO buildingSO)
    {
        double buildingPrice = buildingSO.building_UpgradeCosts[0];

        if (gameManager.playerData.iridium_Current >= buildingPrice)
        {
            bool buildingPlacementSuccessful = PlaceBuilding(buildingSO);

            if (buildingPlacementSuccessful)
            {
                gameManager.playerData.iridium_Current -= buildingPrice;
            }
        }
    }
}
