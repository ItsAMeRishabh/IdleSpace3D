using System;

[System.Serializable]
public class Boost
{
    public string boost_Name;
    [NonSerialized] public double boost_IridiumPerClick;
    [NonSerialized] public double boost_IridiumPerSecond;
    [NonSerialized] public double boost_DarkElixirPerSecond;
    public double boost_TimeRemaining;

    public Boost()
    {
        boost_Name = "Name";
        boost_IridiumPerClick = 0;
        boost_IridiumPerSecond = 0;
        boost_DarkElixirPerSecond = 0;
        boost_TimeRemaining = 0;
    }

    public Boost(BoostSO boostSO)
    {
        boost_Name = boostSO.boost_Name;
        boost_IridiumPerClick = boostSO.boost_IridiumPerClick;
        boost_IridiumPerSecond = boostSO.boost_IridiumPerSecond;
        boost_DarkElixirPerSecond = boostSO.boost_DarkElixirPerSecond;
        boost_TimeRemaining = boostSO.boost_Duration;
    }
}
