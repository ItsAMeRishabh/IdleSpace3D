using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "SpaceIdle3D/DefaultValues", fileName = "DefaultValues")]
public class DefaultValues : ScriptableObject
{
    public double iridium_Total = 0;
    public double iridium_PerSecond = 1;
    public double iridium_PerClick = 10;
    public int iridium_PerClickLevel = 1;
    public List<Building> ownedBuildings = new List<Building>();
}
