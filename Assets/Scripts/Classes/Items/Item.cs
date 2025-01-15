using System;
using System.Collections;
using System.Collections.Generic;

public class Item
{
    public string name { get; private set; }
    public int weight { get; private set; }
    public int spawnRate { get; private set; }

    public Item(string _name, int _weight, int _spawnRate)
    {
        name = _name;
        weight = _weight;
        spawnRate = _spawnRate;
    }
}
