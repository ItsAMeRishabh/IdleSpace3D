using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(GameManager))]
public class BuildingManager : MonoBehaviour
{
    public List<BuildingLocations> buildingLocations;
    public List<Building> ownedBuildings = new List<Building>();

    private Dictionary<Transform, Building> buildingLocationsDict = new Dictionary<Transform, Building>();

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

            foreach(BuildingLocations location in buildingLocations)
            {
                if(location.buildingSO.building_Name.Equals(buildingData.building_Name))
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

    public void BuyBuilding(BuildingSO buildingSO)
    {
        BuildingLocations BLS = Array.Find(buildingLocations.ToArray(), x => x.buildingSO == buildingSO);

        if (BLS == null)
        {
            Debug.LogError("Building not found for " + buildingSO.building_Name);
            return;
        }

        foreach (Transform t in BLS.buildingLocations)
        {
            if (!buildingLocationsDict.ContainsKey(t))
            {
                Building building = Instantiate(buildingSO.buildingPrefabs[buildingSO.building_Level - 1], t).GetComponent<Building>();

                building.buildingData = new BuildingData();
                building.buildingData.building_Name = buildingSO.building_Name;
                building.buildingData.building_Level = buildingSO.building_Level;
                building.buildingData.building_IridiumBoostPerLevel = buildingSO.building_IridiumBoostPerLevel;
                building.buildingData.ownedTroops = new List<Troop>();
                ownedBuildings.Add(building);
                buildingLocationsDict.Add(t, building);

                return;
            }
        }

        Debug.LogError("No free building locations found for " + buildingSO.building_Name);
        return;
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
