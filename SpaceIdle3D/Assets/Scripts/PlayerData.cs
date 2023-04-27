using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public string profileName;

    public JsonDateTime profileCreateTime;
    public JsonDateTime lastSaveTime;

    public double iridium_Total;
    public double iridium_Current;
    [NonSerialized] public double iridium_PerSecond; //Recalculated on load
    [NonSerialized] public double iridium_PerSecondBoosted;

    public double darkElixir_Total;
    public double darkElixir_PerSecond;
    [NonSerialized] public double darkElixir_PerSecondBoosted;

    public double iridium_PerClickRate;
    public int iridium_PerClickLevel;
    [NonSerialized] public double iridium_PerClick; //Recalculated on load
    [NonSerialized] public double iridium_PerClickBoosted;

    public List<BuildingData> ownedBuildings = new List<BuildingData>();
    public List<Boost> activeBoosts = new List<Boost>();
}
