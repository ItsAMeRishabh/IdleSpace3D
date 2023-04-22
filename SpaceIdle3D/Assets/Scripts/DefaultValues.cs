using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "SpaceIdle3D/DefaultValues", fileName = "DefaultValues")]
public class DefaultValues : ScriptableObject
{
    public double iridium_Total = 0;
    public double iridium_PerSecond = 0;
    public double darkElixir_Total = 0;
    public double darkElixir_PerSecond = 0.01f;
    public double iridium_PerClick = 1;
    public int iridium_PerClickLevel = 1;
    public double iridium_PerClickRate = 1f;
    public List<BuildingData> ownedBuildings = new List<BuildingData>();
    public List<Boost> activeBoosts = new List<Boost>();
}
