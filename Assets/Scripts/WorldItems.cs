using System;
using System.Collections;
using System.Collections.Generic;

public static class WorldItems
{
    public static List<Item> itemData { get; private set; }
    public const int FOOD_GRAMS_MULTIPLIER = 100;
    public const int MATCHES_GRAMS_MULTIPLIER = 10;

    static WorldItems()
    {
        itemData = GenerateItemData();
    }

    #region METHODS
    /// <summary>
    /// Generates all the items in the game world.
    /// </summary>
    /// <returns>A dictionary of all the items, separated by item type.</returns>
    private static List<Item> GenerateItemData()
    {
        return new List<Item>(){
            // Food items
            new Food("Bread", 500, 50, 8, Ingest.Eat),
            new Food("Water", 200, 90, 2, Ingest.Drink),
            new Food("Chocolate", 100, 50, 4, Ingest.Eat),
            new Food("Fruit", 100, 40, 5, Ingest.Eat),
            new Food("Energy Drink", 100, 20, 10, Ingest.Drink),
            // Clothing items
            new Clothing("T-Shirt", 500, 0, 1, ClothingSlot.Shirt),
            new Clothing("Hoodie", 1000, 30, 2, ClothingSlot.Coat),
            new Clothing("Trousers", 1500, 20, 2, ClothingSlot.Trousers),
            new Clothing("Trainers", 2000, 0, 1, ClothingSlot.Shoes),
            new Clothing("Boots", 3000, 10, 2, ClothingSlot.Shoes),
            new Clothing("Coat", 1500, 5, 3, ClothingSlot.Coat),
            new Clothing("Cap", 200, 20, 1, ClothingSlot.Hat),
            new Clothing("Woolly Hat", 300, 5, 2, ClothingSlot.Hat),
            new Clothing("Shorts", 800, 0, 1, ClothingSlot.Trousers),
            // Misc. items
            new Item("Sleeping Bag", 5000, 5),
            new Item("Matches", 100, 50)
        };
    }

    /// <summary>
    /// Gets indices of all items with passed item type.
    /// </summary>
    /// <returns>List of indices of all items with passed item type.</returns>
    public static List<int> GetAllItemTypeIndices(Type itemType)
    {
        List<int> typeIndices = new List<int>();
        for (int i = 0; i < itemData.Count; i++)
        {
            if (itemData[i].GetType() == itemType) typeIndices.Add(i);
        }
        return typeIndices;
    }

    /// <summary>
    /// Gets item index by it's name.
    /// </summary>
    /// <param name="itemName">Name of the item being searched for.</param>
    /// <returns>Index of item.</returns>
    public static int GetItemIndex(string itemName)
    {
        for (var i = 0; i < itemData.Count; i++)
        {
            if (itemData[i].name == itemName) return i;
        }
        // Method will only return -1 if item withing matching name could not be found or there is an error
        return -1;
    }
    #endregion
}
