using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildingLocation
{
    public BuildingSO buildingSO;
    public List<Transform> buildingLocations = new List<Transform>();
}
