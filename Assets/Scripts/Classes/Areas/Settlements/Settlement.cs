using System;
using System.Collections;
using System.Collections.Generic;

public abstract class Settlement : Area
{
    public List<District> districts { get; protected set; }

    public Settlement()
    {
        spawnRateMod = 100;
        connections = new List<Area>();
        // Create districts and set their parent to this
        districts = GenerateDistricts();
        foreach (District d in districts) d.parent = this;
    }

    protected abstract List<District> GenerateDistricts();
}
