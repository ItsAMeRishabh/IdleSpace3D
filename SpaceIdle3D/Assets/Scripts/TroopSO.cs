using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SpaceIdle3D/Troop", fileName = "Troop")]
public class TroopSO : ScriptableObject
{
    public string troop_Name = "Name";
    public int troop_Level = 1;
    public int troops_Owned = 0;
    public double troop_BaseCost = 0;
    public double troop_BaseIridiumPerSecond = 0;
    public double troop_IridiumBoostPerLevel = 1.2;
    public double troop_CostMultiplier = 1.25f;
}
