using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public string profileName;

    public JsonDateTime profileCreateTime;
    public JsonDateTime lastSaveTime;
    public double maxIdleTime;

    public double iridium_Total;
    public double iridium_Current;
    [NonSerialized] public double iridium_PerSecond;
    [NonSerialized] public double iridium_PerSecondBoost;

    public double iridium_PerClickRate;
    public int iridium_PerClickLevel;
    [NonSerialized] public double iridium_PerClick;
    [NonSerialized] public double iridium_PerClickBoost;

    public double darkElixir_Total;
    public double darkElixir_Current;
    public double darkElixir_PerSecond;
    [NonSerialized] public double darkElixir_PerSecondBoost;

    public double cosmium_Total;
    public double cosmium_Current;

    public List<BuildingData> ownedBuildings = new List<BuildingData>();
    public List<Boost> activeBoosts = new List<Boost>();
    public List<Stock> ownedStocks = new List<Stock>();
}
