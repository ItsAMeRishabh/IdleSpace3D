using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public string buildingName = "Name";
    public int buildingLevel = 1;
    public List<LevelUpUnlocks> levelUpUnlocks;
    public List<Troop> ownedTroops;
    public List<TroopSO> troopsSO;
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        SOCheck();
    }
    void OnMouseDown()
    {
        gameManager.ClickedOnBuilding(this);
    }

    public BuildingSaveData GetBuildingSaveData()
    {
        BuildingSaveData data = new BuildingSaveData();
        data.buildingName = buildingName;
        data.buildingLevel = buildingLevel;
        data.levelUpUnlocks = levelUpUnlocks;
        data.availableTroops = ownedTroops;
        return data;
    }

    public double GetIridiumPerTick()
    {
        double x = 0;
        foreach (Troop troop in ownedTroops)
        {
            x += troop.GetIridiumPerTick() * troop.structureOwned;
        }
        return x;
    }

    void SOCheck()
    {
        if (troopsSO.Count > 0)
        {
            foreach (TroopSO so in troopsSO)
            {
                foreach (Troop t in ownedTroops)
                {
                    if (t.structureName == so.structureName)
                    {
                        continue;
                    }
                }
                ownedTroops.Add(new Troop(so));
            }
        }
    }
}
