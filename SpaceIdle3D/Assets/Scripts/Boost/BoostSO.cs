using UnityEngine;

[CreateAssetMenu(fileName = "Boost", menuName = "SpaceIdle3D/Boost")]
public class BoostSO : ScriptableObject
{
    public string boost_Name = "Name";
    public double boost_Duration = 10;
    public double boost_MaxDuration = 30;
    public double boost_IridiumPerClick = 1;
    public double boost_IridiumPerSecond = 1;
    public double boost_DarkElixirPerSecond = 1;
}
