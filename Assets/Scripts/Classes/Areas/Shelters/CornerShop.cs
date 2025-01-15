using System.Collections;
using System.Collections.Generic;

public class CornerShop : Shelter
{
    public CornerShop()
    {
        name = "Corner Shop";
        spawnRateMod = 70;
        searchSize = Helper.random.Next(1, 3);
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
    /// Corner shops can spawn all Food, Hoodie, Cap and Matches.
    /// </summary>
    /// <returns>List of item indices that can spawn.</returns>
    protected override List<int> GenerateSpawnList()
    {
        List<int> spawnList = new List<int>();
        spawnList.AddRange(WorldItems.GetAllItemTypeIndices(typeof(Food)));
        spawnList.Add(WorldItems.GetItemIndex("Hoodie"));
        spawnList.Add(WorldItems.GetItemIndex("Cap"));
        //spawnList.Add(WorldItems.GetItemIndex("Matches"));
        return spawnList;
    }
}
