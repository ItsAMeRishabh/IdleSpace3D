using System.Collections.Generic;
using System;

[System.Serializable]
public class BuildingData
{
    public string building_Name = "BuildingName";
    public int building_Level = 1;
    public double building_IridiumBoostPerLevel = 1.2;
    [NonSerialized] public double building_UpgradeCost = 0;
    public List<Troop> building_OwnedTroops;
}
