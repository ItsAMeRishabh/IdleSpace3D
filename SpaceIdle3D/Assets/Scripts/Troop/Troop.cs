using System;
using UnityEngine;

[Serializable]
public class Troop
{
    public string troop_Name = "Name";
    public int troop_Level = 1;
    public int troops_Owned = 0;

    [NonSerialized] public Sprite troop_UnlockedSprite;
    [NonSerialized] public Sprite troop_LockedSprite;
    [NonSerialized] public double troop_BaseCost = 0;
    [NonSerialized] public double troop_CurrentCost = 0;
    [NonSerialized] public double troop_CostMultiplier = 1.25;
    [NonSerialized] public double troop_BaseUpgradeCost = 0;
    [NonSerialized] public double troop_CurrentUpgradeCost = 0;
    [NonSerialized] public double troop_UpgradeCostMultiplier = 1.25f;
    [NonSerialized] public double troop_BaseIridiumPerSecond = 0;
    [NonSerialized] public double troop_IridiumBoostPerLevel = 1.2;
    [NonSerialized] public double troop_IridiumMultiplier = 1;
    [NonSerialized] public GameObject troop_Prefab;
    [NonSerialized] public string troop_AfterReachAnimation;
    [NonSerialized] public GameObject troop_SpawnParticleSystem;

    public Troop(TroopSO so)
    {
        troop_Name = so.troop_Name;
        troop_LockedSprite = so.troop_LockedSprite;
        troop_UnlockedSprite = so.troop_UnlockedSprite;
        troop_AfterReachAnimation = so.troop_AfterReachAnimation;
        troop_Level = so.troop_Level;
        troops_Owned = so.troops_Owned;

        troop_BaseCost = so.troop_BaseCost;
        troop_CostMultiplier = so.troop_CostMultiplier;

        troop_BaseUpgradeCost = so.troop_BaseUpgradeCost;
        troop_UpgradeCostMultiplier = so.troop_UpgradeCostMultiplier;

        troop_BaseIridiumPerSecond = so.troop_BaseIridiumPerSecond;
        troop_IridiumBoostPerLevel = so.troop_IridiumBoostPerLevel;
        troop_IridiumMultiplier = Mathf.Pow((float)troop_IridiumBoostPerLevel, troop_Level - 1);
        troop_Prefab = so.troop_Prefab;
    }

    public Troop(TroopSO so, Troop t)
    {
        troop_Name = t.troop_Name;
        troop_Level = t.troop_Level;
        troops_Owned = t.troops_Owned;

        troop_LockedSprite = so.troop_LockedSprite;
        troop_UnlockedSprite = so.troop_UnlockedSprite;
        troop_AfterReachAnimation = so.troop_AfterReachAnimation;
        troop_BaseCost = so.troop_BaseCost;
        troop_CostMultiplier = so.troop_CostMultiplier;
        troop_CurrentCost = troop_BaseCost * Math.Pow(troop_CostMultiplier, troops_Owned);

        troop_BaseUpgradeCost = so.troop_BaseUpgradeCost;
        troop_UpgradeCostMultiplier = so.troop_UpgradeCostMultiplier;
        troop_CurrentUpgradeCost = troop_BaseUpgradeCost * Math.Pow(troop_UpgradeCostMultiplier, troop_Level - 1);

        troop_BaseIridiumPerSecond = so.troop_BaseIridiumPerSecond;
        troop_IridiumBoostPerLevel = so.troop_IridiumBoostPerLevel;
        troop_IridiumMultiplier = Mathf.Pow((float)troop_IridiumBoostPerLevel, troop_Level - 1);
        troop_Prefab = so.troop_Prefab;
    }

    public double GetIridiumPerTick()
    {
        double x = troop_BaseIridiumPerSecond * troops_Owned * Mathf.Pow((float)troop_IridiumBoostPerLevel, troop_Level - 1) * (1.0f / GameManager.ticksPerSecond);
        return x;
    }

    public double GetIridiumPerTickPerTroop()
    {
        double x = troop_BaseIridiumPerSecond * Mathf.Pow((float)troop_IridiumBoostPerLevel, troop_Level - 1) * (1.0f / GameManager.ticksPerSecond);
        return x;
    }
}
