using System.Collections;
using System.Collections.Generic;

public class Food : Item
{
    public int satiation { get; private set; }
    public Ingest ingestType { get; private set; }

    public Food(string _name, int _weight, int _spawnRate,int _satiation, Ingest _ingestType) : base(_name, _weight, _spawnRate)
    {
        satiation = _satiation;
        ingestType = _ingestType;
    }
}
