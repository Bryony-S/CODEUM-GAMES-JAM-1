using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public abstract class Shelter : Area
{
    public int searchSize { get; protected set; }
    private int searchTime;
    protected List<int> spawnItems;

    public Shelter()
    {
        searchTime = 0;
        connections = new List<Area>();
        spawnItems = GenerateSpawnList();
    }

    protected abstract List<int> GenerateSpawnList(); // Generates list of items that can spawn in this shelter

    /// <summary>
    /// Gets how much of the area is unsearched.
    /// </summary>
    /// <returns>Returns size of unsearched area in number of hours it would take to search.</returns>
    public int GetUnsearchedArea()
    {
        return searchSize - searchTime;
    }

    /// <summary>
    /// Player searches the area for items.
    /// </summary>
    /// <param name="searchAmount">The number of hours the player wants to spend searching.</param>
    public List<int> Search(int searchAmount)
    {
        searchTime += searchAmount;
        List<int> foundItems = new List<int>();
        // Generate found items for each hour spent searching
        for (var t = 0; t < searchAmount; t++)
        {
            foreach (int i in spawnItems)
            {
                List<int> spawnRateMods = new List<int>() {
                spawnRateMod, WorldItems.itemData[i].spawnRate};
                if (Helper.ChanceEvent(spawnRateMods)) foundItems.Add(i);
            }
        }
        return foundItems;
    }
}
