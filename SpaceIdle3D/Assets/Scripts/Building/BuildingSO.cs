using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "SpaceIdle3D/Building", fileName = "Building")]
public class BuildingSO : ScriptableObject
{
    public string building_Name = "Name";
    public int building_Level = 1;
    public double building_IridiumBoostPerLevel = 1.2;
    public double building_BaseCost = 0;
    public double building_CostMultiplier = 1.25f;
    [NonSerialized] public double building_CurrentCost = 0;
    public List<double> building_UpgradeCosts;
    [NonSerialized] public double building_CurrentUpgradeCost = 0;
    public List<GameObject> buildingPrefabs;
    public List<LevelUpUnlocks> levelUpUnlocks;
}
