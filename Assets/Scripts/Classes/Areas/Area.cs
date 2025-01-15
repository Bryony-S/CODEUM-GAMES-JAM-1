using System.Collections;
using System.Collections.Generic;

public abstract class Area
{
    public string name { get; protected set; }
    protected int spawnRateMod;
    public List<Area> connections;

    public abstract List<Area> GetConnections();
}
