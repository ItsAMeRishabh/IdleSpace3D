using System;
using UnityEngine;

public class DataProcessor
{
    private GameManager gameManager;

    public DataProcessor(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public PlayerData WelcomeBackPlayer(PlayerData playerData)
    {
        CalculateNonserializedVariables(playerData);

        DistributeAllVariables(playerData);

        UpdateResourceSources(playerData);

        ResourcesGainedAfterIdle(playerData);

        return playerData;
    }

    public void CalculateNonserializedVariables(PlayerData playerData)
    {
        CalculateNonSerializedTroop(playerData);

        CalculateNonSerializedBoost(playerData);

        CalculateNonserializedBasic(playerData);
    }

    public void CalculateNonSerializedTroop(PlayerData playerData)
    {
        for (int i = 0; i < playerData.ownedBuildings.Count; i++)
        {
            BuildingSO currentBuildingSO = gameManager.BuildingManagerRef.GetBuildingSO(playerData.ownedBuildings[i].building_Name);

            for (int j = 0; j < playerData.ownedBuildings[i].building_OwnedTroops.Count; j++)
            {
                TroopSO currentTroop = gameManager.BuildingManagerRef.GetTroopSO(currentBuildingSO, playerData.ownedBuildings[i].building_OwnedTroops[j].troop_Name);

                playerData.ownedBuildings[i].building_OwnedTroops[j].troop_BaseCost = currentTroop.troop_BaseCost;
                playerData.ownedBuildings[i].building_OwnedTroops[j].troop_BaseIridiumPerSecond = currentTroop.troop_BaseIridiumPerSecond;
                playerData.ownedBuildings[i].building_OwnedTroops[j].troop_IridiumBoostPerLevel = currentTroop.troop_IridiumBoostPerLevel;
                playerData.ownedBuildings[i].building_OwnedTroops[j].troop_CostMultiplier = currentTroop.troop_CostMultiplier;

                playerData.ownedBuildings[i].building_OwnedTroops[j].troop_IridiumMultiplier = 1 * Math.Pow(playerData.ownedBuildings[i].building_OwnedTroops[j].troop_IridiumBoostPerLevel, playerData.ownedBuildings[i].building_OwnedTroops[j].troop_Level - 1);
                playerData.ownedBuildings[i].building_OwnedTroops[j].troop_CurrentCost = currentTroop.troop_BaseCost * Math.Pow(playerData.ownedBuildings[i].building_OwnedTroops[j].troop_CostMultiplier, playerData.ownedBuildings[i].building_OwnedTroops[j].troops_Owned);
            }
        }
    }

    public void CalculateNonSerializedBoost(PlayerData playerData)
    {
        for (int i = 0; i < playerData.activeBoosts.Count; i++)
        {
            BoostSO currentBoostSO = gameManager.BoostManagerRef.GetBoostSO(playerData.activeBoosts[i].boost_Name);

            playerData.activeBoosts[i].boost_IridiumPerClick = currentBoostSO.boost_IridiumPerClick;
            playerData.activeBoosts[i].boost_IridiumPerSecond = currentBoostSO.boost_IridiumPerSecond;
            playerData.activeBoosts[i].boost_DarkElixirPerSecond = currentBoostSO.boost_DarkElixirPerSecond;
        }
    }

    public void CalculateNonserializedBasic(PlayerData playerData)
    {
        playerData.iridium_PerSecond = GetBaseIridiumPerSecond(playerData);
    }

    public void DistributeAllVariables(PlayerData playerData)
    {
        gameManager.BuildingManagerRef.SpawnBuildings(playerData.ownedBuildings);
        gameManager.BoostManagerRef.LoadBoosts(playerData.activeBoosts);
    }

    public void UpdateResourceSources(PlayerData playerData)
    {
        UpdateIridiumSources(playerData);
        UpdateDarkElixirSources(playerData);
    }

    public void UpdateIridiumSources(PlayerData playerData)
    {
        playerData.iridium_PerSecond = GetBaseIridiumPerSecond(playerData);
        playerData.iridium_PerSecondBoost = GetIridiumPerSecondBoost(playerData);

        playerData.iridium_PerClick = GetBaseIridiumPerClick(playerData);
        playerData.iridium_PerClickBoost = GetIridiumPerClickBoost(playerData);
    }

    public void UpdateDarkElixirSources(PlayerData playerData)
    {
        playerData.darkElixir_PerSecond = GetBaseDarkElixirPerSecond(playerData);
        playerData.darkElixir_PerSecondBoost = GetDarkElixirPerSecondBoost(playerData);
    }

    public double GetBaseIridiumPerSecond(PlayerData playerData)
    {
        double baseIPS = 0;

        foreach (Building building in gameManager.BuildingManagerRef.ownedBuildings)
        {
            BuildingData data = building.buildingData;
            double buildingIPS = 0;

            foreach (Troop troop in data.building_OwnedTroops)
            {
                buildingIPS += troop.GetIridiumPerTick() * GameManager.ticksPerSecond;
            }

            BuildingSO bSO = gameManager.BuildingManagerRef.GetBuildingSO(data.building_Name);
            buildingIPS *= Mathf.Pow((float)bSO.building_IridiumBoostPerLevel, data.building_Level - 1);

            baseIPS += buildingIPS;
        }

        return baseIPS;
    }

    public double GetIridiumPerSecondBoost(PlayerData playerData)
    {
        double IPS_Boost = 1;

        foreach (Boost boost in gameManager.BoostManagerRef.activeBoosts)
        {
            IPS_Boost *= boost.boost_IridiumPerSecond;
        }

        return IPS_Boost;
    }

    public double GetBaseIridiumPerClick(PlayerData playerData)
    {
        double baseIPC = Math.Max(1, (playerData.darkElixir_PerSecond * playerData.iridium_PerSecondBoost) * playerData.iridium_PerClickLevel / 100f);

        return baseIPC;
    }

    public double GetIridiumPerClickBoost(PlayerData playerData)
    {
        double IPC_Boost = 1;

        foreach (Boost boost in gameManager.BoostManagerRef.activeBoosts)
        {
            IPC_Boost *= boost.boost_IridiumPerClick;
        }

        return IPC_Boost;
    }

    public double GetBaseDarkElixirPerSecond(PlayerData playerData)
    {
        return playerData.darkElixir_PerSecond;
    }

    public double GetDarkElixirPerSecondBoost(PlayerData playerData)
    {
        double DEPS_Boost = 1;

        foreach (Boost boost in gameManager.BoostManagerRef.activeBoosts)
        {
            DEPS_Boost *= boost.boost_DarkElixirPerSecond;
        }

        return DEPS_Boost;
    }

    public PlayerData ResourcesGainedAfterIdle(PlayerData playerData)
    {
        int timeElapsed = (int)(DateTime.Now - (DateTime)playerData.lastSaveTime).TotalSeconds;
        timeElapsed = (int)Math.Min(timeElapsed, playerData.maxIdleTime);

        double baseIPS = GetBaseIridiumPerSecond(playerData);

        int timeToProcess = timeElapsed;

        double iridiumToAdd = 0;
        double darkelixerToAdd = 0;

        while (timeToProcess > 0)
        {
            double boostedIPS = baseIPS;
            double boostedDEPS = playerData.darkElixir_PerSecond;

            Boost lowestTimeBoost = null;

            for (int i = 0; i < playerData.activeBoosts.Count; i++)
            {
                if (lowestTimeBoost == null)
                {
                    lowestTimeBoost = playerData.activeBoosts[i];
                }
                else
                {
                    if (playerData.activeBoosts[i].boost_TimeRemaining < lowestTimeBoost.boost_TimeRemaining)
                    {
                        lowestTimeBoost = playerData.activeBoosts[i];
                    }
                }

                boostedIPS *= playerData.activeBoosts[i].boost_IridiumPerSecond;
                boostedDEPS *= playerData.activeBoosts[i].boost_DarkElixirPerSecond;
            }

            if (lowestTimeBoost != null)
            {
                if (timeToProcess > lowestTimeBoost.boost_TimeRemaining)
                {
                    iridiumToAdd += boostedIPS * lowestTimeBoost.boost_TimeRemaining;
                    darkelixerToAdd += boostedDEPS * lowestTimeBoost.boost_TimeRemaining;

                    Debug.Log($"{lowestTimeBoost.boost_Name} lasted for {lowestTimeBoost.boost_TimeRemaining} seconds");
                    timeToProcess -= (int)lowestTimeBoost.boost_TimeRemaining;

                    for (int i = 0; i < playerData.activeBoosts.Count; i++)
                    {
                        playerData.activeBoosts[i].boost_TimeRemaining -= lowestTimeBoost.boost_TimeRemaining;
                    }

                    playerData.activeBoosts.Remove(lowestTimeBoost);

                    lowestTimeBoost = null;
                }
                else
                {
                    for (int i = 0; i < playerData.activeBoosts.Count; i++)
                    {
                        playerData.activeBoosts[i].boost_TimeRemaining -= timeToProcess;
                        Debug.Log($"Used up {timeToProcess} seconds of boost: {playerData.activeBoosts[i].boost_Name}");
                    }

                    iridiumToAdd += boostedIPS * timeToProcess;
                    darkelixerToAdd += boostedDEPS * timeToProcess;

                    timeToProcess = 0;
                }
            }
            else
            {
                Debug.Log("No boosts active!");
                iridiumToAdd += boostedIPS * timeToProcess;
                darkelixerToAdd += boostedDEPS * timeToProcess;
                timeToProcess = 0;
            }
        }

        Debug.Log($"{playerData.profileName} was idle for {timeElapsed} seconds");

        Debug.Log($"Iridium Added: {iridiumToAdd}");

        playerData.iridium_Current += iridiumToAdd;
        playerData.iridium_Total += iridiumToAdd;

        Debug.Log($"Dark Elixir Added: {darkelixerToAdd}");
        playerData.darkElixir_Total += darkelixerToAdd;

        return playerData;
    }
}
