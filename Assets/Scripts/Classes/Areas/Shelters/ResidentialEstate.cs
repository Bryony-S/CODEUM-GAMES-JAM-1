using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class ResidentialEstate : Shelter
{
    public bool fireplaceLit { get; private set; }
    private int fireplaceFuel;

    private const int FIREPLACE_STARTING_FUEL = 8;
    private const int FIREPLACE_LIGHT_CHANCE = 90;

    public ResidentialEstate(bool big)
    {
        name = "Residential Estate";
        spawnRateMod = 40;
        if (big)
        {
            searchSize = Helper.random.Next(2, 5);
        }
        else {searchSize = Helper.random.Next(1, 3);}
        fireplaceLit = false;
        fireplaceFuel = FIREPLACE_STARTING_FUEL;
    }

    #region METHODS
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
    /// Residential estates can spawn all Food, all Clothing and Matches.
    /// </summary>
    /// <returns>List of item indices that can spawn.</returns>
    protected override List<int> GenerateSpawnList()
    {
        List<int> spawnList = new List<int>();
        spawnList.AddRange(WorldItems.GetAllItemTypeIndices(typeof(Food)));
        spawnList.AddRange(WorldItems.GetAllItemTypeIndices(typeof(Clothing)));
        //spawnList.Add(WorldItems.GetItemIndex("Matches"));
        return spawnList;
    }

    #region Fireplace methods
    /// <summary>
    /// Player attempts to light the fireplace.
    /// </summary>
    /// <returns>Whether the fireplace was successfully lit or not.</returns>
    public bool TryLightFireplace()
    {
        fireplaceLit = Helper.ChanceEvent(new List<int>(){FIREPLACE_LIGHT_CHANCE});
        return fireplaceLit;
    }

    /// <summary>
    /// Check if the fireplace can be lit.
    /// </summary>
    /// <returns>Whether the fireplace can be lit or not.</returns>
    public bool CanLightFireplace()
    {
        return !fireplaceLit && fireplaceFuel > 0;
    }

    /// <summary>
    /// Burn through 1 unit of fuel, then check if there is any fuel left for the fireplace to stay lit.
    /// </summary>
    /// <returns>Whether the fireplace is still lit after burning fuel.</returns>
    public bool BurnFireplaceFuel()
    {
        UnityEngine.Debug.Log("Burn fuel");
        fireplaceFuel--;
        fireplaceLit = fireplaceFuel > 0;
        return fireplaceLit;
    }
    #endregion
    #endregion
}
