using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public string profileName;
    public double totalIridium;
    public double iridiumPerClickPercent;
    public double upgradeClick_BaseCost;
    public List<Troop> ownedStructures = new List<Troop>();
}
