using System.Collections.Generic;

[System.Serializable]
public class BuildingSaveData
{
    public string buildingName;
    public int buildingLevel;
    public List<LevelUpUnlocks> levelUpUnlocks;
    public List<Troop> availableTroops;
}
