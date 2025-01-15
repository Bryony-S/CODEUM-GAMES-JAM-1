using System.Collections;
using System.Collections.Generic;

public class AbandonedCar : Shelter
{
    public AbandonedCar(Area parent)
    {
        name = "Abandoned Car";
        spawnRateMod = 5;
        searchSize = 1;
        connections = new List<Area> { parent };
    }

    /// <summary>
    /// Returns all Areas connected to this area.
    /// </summary>
    /// <returns>A list of Areas connected to this area.</returns>
    public override List<Area> GetConnections()
    {
        return connections;
    }

    /// <summary>
    /// Create list of items that can spawn here.
    /// Abandoned cars can spawn Water, Boots and Coats.
    /// </summary>
    /// <returns>List of item indices that can spawn.</returns>
    protected override List<int> GenerateSpawnList()
    {
        List<int> spawnList = new List<int>();
        spawnList.Add(WorldItems.GetItemIndex("Water"));
        spawnList.Add(WorldItems.GetItemIndex("Boots"));
        spawnList.Add(WorldItems.GetItemIndex("Coat"));
        return spawnList;
    }
}
