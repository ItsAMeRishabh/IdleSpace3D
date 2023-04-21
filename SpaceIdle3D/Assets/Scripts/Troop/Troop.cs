using UnityEngine;

[System.Serializable]
public class Troop
{
    public string troop_Name = "Name";
    public int troop_Level = 1;
    public int troops_Owned = 0;
    public double troop_BaseCost = 0;
    public double troop_CurrentCost = 0;
    public double troop_BaseIridiumPerSecond = 0;
    public double troop_IridiumBoostPerLevel = 1.2;
    public double troop_IridiumMultiplier = 1;
    public double troop_CostMultiplier = 1.25;

    public Troop(string name, int baseCost, int baseIridiumPerSecond, double iridiumBoostPerLevel, double costMultiplier)
    {
        troop_Name = name;
        troop_BaseCost = baseCost;
        troop_BaseIridiumPerSecond = baseIridiumPerSecond;
        troop_IridiumBoostPerLevel = iridiumBoostPerLevel;
        troop_IridiumMultiplier = Mathf.Pow((float)troop_IridiumBoostPerLevel, troop_Level - 1);
        troop_CostMultiplier = costMultiplier;
    }

    public Troop(TroopSO so)
    {
        troop_Name = so.troop_Name;
        troop_Level = so.troop_Level;
        troops_Owned = so.troops_Owned;
        troop_BaseCost = so.troop_BaseCost;
        troop_BaseIridiumPerSecond = so.troop_BaseIridiumPerSecond;
        troop_IridiumBoostPerLevel = so.troop_IridiumBoostPerLevel;
        troop_IridiumMultiplier = Mathf.Pow((float)troop_IridiumBoostPerLevel, troop_Level - 1);
        troop_CostMultiplier = so.troop_CostMultiplier;
    }

    public double GetIridiumPerTick()
    {
        double x = troop_BaseIridiumPerSecond * troop_IridiumMultiplier * (1.0f / GameManager.ticksPerSecond);
        return x;
    }
}
