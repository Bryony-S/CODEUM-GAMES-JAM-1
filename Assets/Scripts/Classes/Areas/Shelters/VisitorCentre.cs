using System.Collections;
using System.Collections.Generic;

public class VisitorCentre : Shelter
{
    public VisitorCentre()
    {
        name = "Visitor Information Centre";
        spawnRateMod = 5;
        searchSize = 1;
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
    /// Visitor centres can spawn Water and Sleeping Bags.
    /// </summary>
    /// <returns>List of item indices that can spawn.</returns>
    protected override List<int> GenerateSpawnList()
    {
        List<int> spawnList = new List<int>();
        spawnList.Add(WorldItems.GetItemIndex("Water"));
        spawnList.Add(WorldItems.GetItemIndex("Sleeping Bag"));
        return spawnList;
    }
}
