using System;
using UnityEngine;

[Serializable]
public class Troop
{
    public string troop_Name = "Name";
    public int troop_Level = 1;
    public int troops_Owned = 0;
    [NonSerialized] public double troop_BaseCost = 0;
    [NonSerialized] public double troop_CurrentCost = 0;
    [NonSerialized] public double troop_BaseIridiumPerSecond = 0;
    [NonSerialized] public double troop_IridiumBoostPerLevel = 1.2;
    [NonSerialized] public double troop_IridiumMultiplier = 1;
    [NonSerialized] public double troop_CostMultiplier = 1.25;
    [NonSerialized] public GameObject troop_Prefab;

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
        troop_Prefab = so.troop_Prefab;
    }

    public double GetIridiumPerTick()
    {
        double x = troop_BaseIridiumPerSecond * troops_Owned * troop_IridiumMultiplier * (1.0f / GameManager.ticksPerSecond);
        return x;
    }

    public double GetIridiumPerTickPerTroop()
    {
        double x = troop_BaseIridiumPerSecond * troop_IridiumMultiplier * (1.0f / GameManager.ticksPerSecond);
        return x;
    }
}
