using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Building : MonoBehaviour
{
    public BuildingSO buildingSO;
    public BuildingData buildingData;

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void OnMouseDown()
    {
        if (!DetectClickOnUI.IsPointerOverUIElement())
        {
            gameManager.ClickedOnBuilding(this);
        }
    }

    public double GetIridiumPerTick()
    {
        double x = 0;
        foreach (Troop troop in buildingData.building_OwnedTroops)
        {
            x += troop.GetIridiumPerTick() * troop.troops_Owned * Mathf.Pow((float)buildingData.building_IridiumBoostPerLevel, buildingData.building_Level - 1);
        }
        return x;
    }
}
