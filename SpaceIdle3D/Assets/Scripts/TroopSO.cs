using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Iridium Generator", menuName = "Iridium Generator")]
public class TroopSO : ScriptableObject
{
    public string structureName = "Name";
    public int structureLevel = 1;
    public int structureOwned = 0;
    public double structureBaseCost = 0;
    public double structureBaseIridiumPerSecond = 0;
    public double structureIridiumMultiplier = 1;
    public double structureCostMultiplier = 1.25f;
    public double structureCostMultiplierMultiplier = 1;
}
