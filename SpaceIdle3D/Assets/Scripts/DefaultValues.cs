using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "SpaceIdle3D/DefaultValues", fileName = "DefaultValues")]
public class DefaultValues : ScriptableObject
{
    public double maxIdleTime = 3600;
    public double iridium_Total = 0;
    public double iridium_Current = 0;
    public double iridium_PerSecond = 0;
    public double darkElixir_Total = 0;
    public double darkElixir_Current = 0;
    public double darkElixir_PerSecond = 0.01f;
    public double iridium_PerClick = 1;
    public int iridium_PerClickLevel = 1;
    public double iridium_PerClickRate = 1f;
}
