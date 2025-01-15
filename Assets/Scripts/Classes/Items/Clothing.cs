using System.Collections;
using System.Collections.Generic;

public class Clothing : Item
{
    public int warmth { get; private set; }
    public ClothingSlot clothingType { get; private set; }

    public Clothing(string _name, int _weight, int _spawnRate, int _warmth, ClothingSlot _clothingType) : base(_name, _weight, _spawnRate)
    {
        warmth = _warmth;
        clothingType = _clothingType;
    }
}
