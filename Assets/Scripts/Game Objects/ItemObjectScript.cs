using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemObjectScript : MonoBehaviour
{
    [SerializeField] private TMP_Text itemText;
    [SerializeField] private Button useItemBtn;

    private int itemIndex;
    private WorldManagerScript worldMgr;

    #region Methods
    /// <summary>
    /// Display item's description and options.
    /// </summary>
    /// <param name="_itemIndex">The index of the item.</param>
    /// <param name="itemQuantity">The amount of the item currently stored in the player's inventory.</param>
    public void DisplayItem(int _itemIndex, int itemQuantity, WorldManagerScript mgr)
    {
        worldMgr = mgr;
        itemIndex = _itemIndex;
        if (WorldItems.itemData[itemIndex] is Food)
        {
            // Display food quantity in grams
            Food f = (Food)WorldItems.itemData[itemIndex];
            itemText.text = $"{f.name} ({itemQuantity * WorldItems.FOOD_GRAMS_MULTIPLIER}g)";
            if (worldMgr.IsPlayerHungry())
            {
                // Change "Use item" button to "Eat" or "Drink"
                if (f.ingestType == Ingest.Eat)
                {
                    useItemBtn.GetComponentInChildren<TMP_Text>().text = "Eat";
                }
                else { useItemBtn.GetComponentInChildren<TMP_Text>().text = "Drink"; }
            } // Player not hungry, cannot eat/drink
            else { Destroy(useItemBtn.gameObject); }
        }
        else // Display item quantity as normal
        {
            itemText.text = $"{WorldItems.itemData[itemIndex].name} (x{itemQuantity})";
            if (WorldItems.itemData[itemIndex] is Clothing)
            {
                // Change "Use item" button to "Equip" or "Unequip"
                Clothing c = (Clothing)WorldItems.itemData[itemIndex];
                if (worldMgr.IsClothingEquipped(c))
                {
                    useItemBtn.GetComponentInChildren<TMP_Text>().text = "Unequip";
                }
                else { useItemBtn.GetComponentInChildren<TMP_Text>().text = "Equip"; }
            } // Cannot use item, so destroy "Use item" button
            else { Destroy(useItemBtn.gameObject); }            
        }
    }

    /// <summary>
    /// Use this item.
    /// </summary>
    public void UseItem()
    {
        // Check if item is Food (eat/drink item) or Clothing (equip/unequip item)
        if (WorldItems.itemData[itemIndex] is Food)
        {
            worldMgr.IngestFood(itemIndex);
        }
        else if (WorldItems.itemData[itemIndex] is Clothing) worldMgr.UnEquipClothing(itemIndex);
    }

    /// <summary>
    /// Drop this item.
    /// </summary>
    public void DropItem()
    {
        worldMgr.DropItem(itemIndex);
    }
    #endregion
}
