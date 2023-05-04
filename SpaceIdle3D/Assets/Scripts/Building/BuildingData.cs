using System.Collections.Generic;

[System.Serializable]
public class BuildingData
{
    public string building_Name = "BuildingName";
    public int building_Level = 1;
    public List<Troop> building_OwnedTroops;
}
