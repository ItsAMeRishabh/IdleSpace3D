[System.Serializable]
public class Troop
{
    public string structureName = "Name";
    public int structureLevel = 1;
    public int structureOwned = 0;
    public double structureBaseCost = 0;
    public double structureCurrentCost = 0;
    public double structureBaseIridiumPerSecond = 0;
    public double structureIridiumMultiplier = 1; //TODO: Make this dependant on the level of the building
    public double structureCostMultiplier = 1.25f;
    public double structureCostMultiplierMultiplier = 1;

    public Troop(string name, int baseCost, int baseIridiumPerSecond, double iridiumMultiplier, double costMultiplier, double costMultiplierMultiplier)
    {
        structureName = name;
        structureOwned = 0;
        structureBaseCost = baseCost;
        structureBaseIridiumPerSecond = baseIridiumPerSecond;
        structureIridiumMultiplier = iridiumMultiplier;
        structureCostMultiplier = costMultiplier;
        structureCostMultiplierMultiplier = costMultiplierMultiplier;
    }

    public Troop(TroopSO so)
    {
        structureName = so.structureName;
        structureOwned = so.structureOwned;
        structureBaseCost = so.structureBaseCost;
        structureBaseIridiumPerSecond = so.structureBaseIridiumPerSecond;
        structureIridiumMultiplier = so.structureIridiumMultiplier;
        structureCostMultiplier = so.structureCostMultiplier;
        structureCostMultiplierMultiplier = so.structureCostMultiplierMultiplier;
    }

    public double GetIridiumPerTick()
    {
        double x = structureBaseIridiumPerSecond * structureIridiumMultiplier * (1.0f / GameManager.ticksPerSecond);
        return x;
    }
}
