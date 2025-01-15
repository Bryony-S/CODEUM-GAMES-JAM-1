using System.Collections;
using System.Collections.Generic;

public class Superstore : Shelter
{
    public Superstore()
    {
        name = "Superstore";
        spawnRateMod = 60;
        searchSize = Helper.random.Next(3, 5);
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
    /// Superstores can spawn all Items.
    /// </summary>
    /// <returns>List of item indices that can spawn.</returns>
    protected override List<int> GenerateSpawnList()
    {
        List<int> spawnList = new List<int>();
        //for (int i = 0; i < WorldItems.itemData.Count; i++) spawnList.Add(i);
        spawnList.AddRange(WorldItems.GetAllItemTypeIndices(typeof(Food)));
        spawnList.AddRange(WorldItems.GetAllItemTypeIndices(typeof(Clothing)));
        //spawnList.Add(WorldItems.GetItemIndex("Sleeping Bag"));
        return spawnList;
    }
}
