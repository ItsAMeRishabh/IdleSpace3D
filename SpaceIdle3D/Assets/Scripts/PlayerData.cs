using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string profileName;
    public double iridium_Total;
    public double iridium_PerSecond; //Recalculated on load
    public double iridium_PerClick; //Recalculated on load
    public int iridium_PerClickLevel;
    public List<BuildingData> ownedBuildings = new List<BuildingData>();
    public List<Boost> boosts = new List<Boost>();
}
