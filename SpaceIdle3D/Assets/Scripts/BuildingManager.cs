using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

[RequireComponent(typeof(GameManager))]
public class BuildingManager : MonoBehaviour
{
    public Building selectedBuilding;
    public List<BuildingLocation> buildingLocations;
    public List<Building> ownedBuildings = new List<Building>();

    private Dictionary<Transform, Building> buildingLocationsDict = new Dictionary<Transform, Building>();
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GetComponent<GameManager>();    
    }

    public List<BuildingData> GetBuildingDatas()
    {
        List<BuildingData> buildingDatas = new List<BuildingData>();

        foreach (Building building in ownedBuildings)
        {
            buildingDatas.Add(building.buildingData);
        }

        return buildingDatas;
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
                Building building = Instantiate(buildingSO.buildingPrefabs[buildingSO.building_Level - 1], transform).GetComponent<Building>();
                building.buildingData = buildingData;
                ownedBuildings.Add(building);
            }
            else
            {
                Debug.LogError("BuildingSO not found for " + buildingData.building_Name);
            }
        }
    }

    public void PlaceBuilding(BuildingSO buildingSO)
    {
        BuildingLocation BLS = Array.Find(buildingLocations.ToArray(), x => x.buildingSO == buildingSO);

        if (BLS == null)
        {
            Debug.LogError("Building not found for " + buildingSO.building_Name);
            return;
        }

        foreach (Transform t in BLS.buildingLocations)
        {
            if (!buildingLocationsDict.ContainsKey(t))
            {
                GameObject obj = Instantiate(buildingSO.buildingPrefabs[buildingSO.building_Level - 1], t);
                Building building = obj.GetComponentInChildren<Building>();

                building.buildingSO = buildingSO;
                building.buildingData = new BuildingData();
                building.buildingData.building_Name = buildingSO.building_Name;
                building.buildingData.building_Level = buildingSO.building_Level;
                building.buildingData.building_IridiumBoostPerLevel = buildingSO.building_IridiumBoostPerLevel;
                building.buildingData.ownedTroops = new List<Troop>();

                foreach (LevelUpUnlocks levelUpUnlock in buildingSO.levelUpUnlocks)
                {
                    if(building.buildingData.building_Level >= levelUpUnlock.level)
                    {
                        List<Troop> troopsToAdd = new List<Troop>();

                        foreach(TroopSO troop in levelUpUnlock.unlockedTroops)
                        {
                            troopsToAdd.Add(new Troop(troop));
                        }

                        foreach(Troop troop in troopsToAdd)
                        {
                            building.buildingData.ownedTroops.Add(troop);
                        }
                    }
                }

                gameManager.UIManager.PopulateBuildingUI();
                ownedBuildings.Add(building);
                buildingLocationsDict.Add(t, building);
                gameManager.CalculateCosts();
                return;
            }
        }

        Debug.LogError("No free building locations found for " + buildingSO.building_Name);
        return;
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
                GameObject obj = Instantiate(buildingSO.buildingPrefabs[buildingData.building_Level - 1], t);
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
                            Troop foundTroop = Array.Find(building.buildingData.ownedTroops.ToArray(), x => x.troop_Name == troop.troop_Name);

                            if(foundTroop != null)
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
                            building.buildingData.ownedTroops.Add(troop);
                        }
                    }
                }

                gameManager.UIManager.PopulateBuildingUI();
                ownedBuildings.Add(building);
                buildingLocationsDict.Add(t, building);
                gameManager.CalculateCosts();
                return building;
            }
        }

        return null;
    }

    public void UpgradeBuilding(Building building)
    {
        BuildingSO buildingSO = building.buildingSO;

        if (buildingSO == null)
        {
            Debug.LogError("BuildingSO not found for " + building.buildingData.building_Name);
            return;
        }

        if (building.buildingData.building_Level == buildingSO.buildingPrefabs.Count)
        {
            Debug.LogError("Building is already at max level");
            return;
        }

        Transform buildingTransform = buildingLocationsDict.FirstOrDefault(x => x.Value == building).Key;

        ownedBuildings.Remove(building);
        buildingLocationsDict.Remove(buildingTransform);
        BuildingData buildingData = building.buildingData;
        buildingData.building_Level++;
        selectedBuilding = PlaceBuilding(buildingSO, buildingData);
        gameManager.UpdateIridiumPerSecond();
        gameManager.CalculateCosts();
        Destroy(building.transform.parent.gameObject);
    }

    public int GetBuildingCount(string buildingName)
    {
        int count = 0;

        foreach (Building building in ownedBuildings)
        {
            if (building.buildingData.building_Name.Equals(buildingName))
            {
                count++;
            }
        }

        return count;
    }
}
