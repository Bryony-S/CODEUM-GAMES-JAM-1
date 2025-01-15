using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.UIElements;

public class District : Area
{
    public Settlement parent;

    public District(string _name, List<Area> _shelters)
    {
        name = _name;
        spawnRateMod = 100;
        connections = _shelters;
        foreach (Area a in connections) a.connections.Add(this);
    }

    #region Methods
    /// <summary>
    /// Returns all Areas connected to this area.
    /// </summary>
    /// <returns>A list of Areas connected to this area.</returns>
    public override List<Area> GetConnections()
    {
        // Check if connecting road exists, if not then generate road
        if (!ConnectingRoadExists())
        {
            Road r = new Road();
            r.connections.Add(this);
            connections.Add(r);
        }
        List<Area> c = new List<Area>(connections);
        // Get other districts in this settlement
        foreach (District d in parent.districts) if (d != this) c.Add(d);
        return c;
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
    /// Returns all shelters in this district.
    /// </summary>
    /// <returns>A list of shelters in this district.</returns>
    public List<Shelter> GetShelters()
    {
        List<Shelter> shelters = new List<Shelter>();
        foreach (Area area in connections)
        {
            if (area is Shelter) shelters.Add((Shelter)area);
        }
        return shelters;
    }

    /// <summary>
    /// Returns all other districts connected to this district.
    /// </summary>
    /// <returns>A list of districts connected to this district.</returns>
    public List<District> GetOtherDistricts()
    {
        List<District> otherDistricts = new List<District>();
        foreach (Area area in GetConnections())
        {
            if (area is District) otherDistricts.Add((District)area);
        }
        return otherDistricts;
    }
    #endregion
}
