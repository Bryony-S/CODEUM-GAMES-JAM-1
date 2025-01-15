using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class Road : Area
{
    public int length {  get; private set; }

    private const int UNIT_SIZE = 5; // 1 unit = 5 miles = 1 hour of travel time
    private const int MIN_LENGTH = 2;
    private const int MAX_LENGTH = 4; // Maximum possible length of a road in units: 4 units = 20 miles = 4 hours of travel time
    private const int MIN_CONNECTING_ROADS = 2; // The minimum number of connecting roads to generate
    private const int MAX_CONNECTING_ROADS = 4; // The maximum number of connecting roads to generate
    private const int TOWN_GEN_CHANCE = 30; // Percentage chance of a Settlement being generated as a Town
    
    public Road()
    {
        name = "Road";
        spawnRateMod = 100;
        length = Helper.random.Next(MIN_LENGTH, MAX_LENGTH + 1); // Generate random road length between 1-4 units
        connections = new List<Area> { new AbandonedCar(this) };
    }

    #region Methods
    /// <summary>
    /// Gets the length of the road in miles.
    /// </summary>
    /// <returns>The length of the road in miles.</returns>
    public int GetLengthInMiles()
    {
        return length * UNIT_SIZE;
    }

    #region Connection methods
    /// <summary>
    /// Returns all Areas connected to this area.
    /// </summary>
    /// <returns>A list of Areas connected to this area.</returns>
    public override List<Area> GetConnections()
    {
        // Check if connecting roads exist, if not then generate them
        if (!ConnectingRoadExists())
        {
            // Generate random number of new roads
            List<Area> newRoads = new List<Area>();
            for (int i = 0; i < Helper.random.Next(MIN_CONNECTING_ROADS, MAX_CONNECTING_ROADS + 1); i++)
            {
                Road r = new Road();
                // Connect new road to this road
                r.connections.Add(this);
                connections.Add(r);
                newRoads.Add(r);
            }
            // Connect new roads to each other
            foreach (Area road in newRoads)
            {
                foreach (Area connection in newRoads) if (road != connection) road.connections.Add(connection);
            }
        }
        // Check if connected to a District, if not then generate a Settlement
        if (!ConnectingDistrictExists())
        {
            if (Helper.ChanceEvent(new List<int>() { TOWN_GEN_CHANCE }))
            {
                // Generate Town
                Town t = new Town();
                // Connect this road to a random district in Town
                District d = t.districts[Helper.random.Next(t.districts.Count)];
                connections.Add(d);
                d.connections.Add(this);
            }
            else
            {
                // Generate Village
                Village v = new Village();
                // Connect this road to a random district in Village
                District d = v.districts[Helper.random.Next(v.districts.Count)];
                connections.Add(d);
                d.connections.Add(this);
            }
        }
        return connections;
    }

    /// <summary>
    /// Gets roads connected to this road.
    /// </summary>
    /// <returns>A list of connecting roads.</returns>
    public List<Road> GetConnectingRoads()
    {
        List<Road> connectingRoads = new List<Road>();
        foreach (Area area in GetConnections())
        {
            if (area is Road) connectingRoads.Add((Road)area);
        }
        return connectingRoads;
    }

    /// <summary>
    /// Checks if there is at least 1 Road in connected areas.
    /// </summary>
    /// <returns>True if a Road is found in connections.</returns>
    private bool ConnectingRoadExists()
    {
        foreach (Area area in connections) if (area is Road) return true;
        return false;
    }

    /// <summary>
    /// Checks if there is at least 1 District in connected areas.
    /// </summary>
    /// <returns>True if a District is found in connections.</returns>
    private bool ConnectingDistrictExists()
    {
        foreach (Area area in connections) if (area is District) return true;
        return false;
    }
    #endregion
    #endregion
}
