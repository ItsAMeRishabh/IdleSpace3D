using System;
using UnityEngine;

[RequireComponent(typeof(TroopManager))]
public class Building : MonoBehaviour
{
    public BuildingSO buildingSO;
    public BuildingData buildingData;

    private GameManager gameManager;
    private TroopManager troopManager;

    public TroopManager TroopManager => troopManager;

    private bool startedTouch = false;
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void OnMouseDown()
    {
        if (!DetectClickOnUI.IsPointerOverUIElement())
        {
            startedTouch = true;
            gameManager.ClickedOnBuilding(this);
        }
    }

    private void OnMouseUp()
    {
        /*if(startedTouch && !DetectClickOnUI.IsPointerOverUIElement())
        {
            startedTouch = false;
            gameManager.ClickedOnBuilding(this);
        }
        else
        {
            startedTouch = false;
        }*/
    }

    public double GetIridiumPerTick()
    {
        double x = 0;
        foreach (Troop troop in buildingData.building_OwnedTroops)
        {
            x += troop.GetIridiumPerTick() * Math.Pow(buildingSO.building_IridiumBoostPerLevel, buildingData.building_Level - 1);
        }
        return x;
    }
}
