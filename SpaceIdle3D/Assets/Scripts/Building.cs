using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public string building_Name = "Name";
    public int building_Level = 1;
    public double building_IridiumBoostPerLevel = 1.2;
    public GameObject[] buildingPrefabs;
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

    public double GetIridiumPerTick()
    {
        double x = 0;
        foreach (Troop troop in ownedTroops)
        {
            x += troop.GetIridiumPerTick() * troop.troops_Owned * Mathf.Pow((float)building_IridiumBoostPerLevel, building_Level - 1);
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
                    if (t.troop_Name == so.troop_Name)
                    {
                        continue;
                    }
                }
                ownedTroops.Add(new Troop(so));
            }
        }
    }
}
