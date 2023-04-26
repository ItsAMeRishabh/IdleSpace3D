using System;
using UnityEngine;

public class DataProcessor
{
    public PlayerData WelcomeBackPlayer (GameManager gameMamager, PlayerData playerData)
    {
        DateTime saveTime = playerData.lastSaveTime;
        DateTime currentTime = DateTime.Now;
        int timeElapsed = (int)(currentTime - saveTime).TotalSeconds;

        double baseIridiumPerSecond = GetBaseIridiumPerSecond(gameMamager, playerData);

        return null;
    }

    public double GetBaseIridiumPerSecond(GameManager gameMamager, PlayerData playerData)
    {
        double baseIPS = 0;

        foreach(BuildingData data in playerData.ownedBuildings)
        {
            double buildingIPS = 0;

            foreach(Troop troop in data.building_OwnedTroops)
            {
                buildingIPS += troop.GetIridiumPerTick() * troop.troops_Owned * GameManager.ticksPerSecond;
            }

            BuildingSO bSO = gameMamager.BuildingManager.GetBuildingSO(data.building_Name);
            buildingIPS *= Mathf.Pow((float)bSO.building_IridiumBoostPerLevel, data.building_Level - 1);

            baseIPS += buildingIPS;
        }

        return baseIPS;
    }
}
