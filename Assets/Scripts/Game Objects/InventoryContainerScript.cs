using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryContainerScript : MonoBehaviour
{
    [SerializeField] private GameObject itemObject;
    [SerializeField] private WorldManagerScript worldMgr;

    /// <summary>
    /// Refreshes inventory container to display current inventory.
    /// </summary>
    /// <param name="currentInventory">The player's current inventory.</param>
    public void Refresh(Dictionary<int, int> currentInventory)
    {
        Clear();
        foreach (int i in currentInventory.Keys)
        {
            // Check there is at least 1 of that item in inventory
            if (currentInventory[i] > 0)
            {
                // Create item info & options object
                GameObject o = Instantiate(itemObject, this.transform);
                o.name = WorldItems.itemData[i].name + " item";
                o.GetComponent<ItemObjectScript>().DisplayItem(i, currentInventory[i], worldMgr);
            }
        }
    }

    /// <summary>
    /// Clear old child objects in inventory container.
    /// </summary>
    private void Clear()
    {
        if (transform.childCount > 0)
        {
            // Source: https://stackoverflow.com/a/46359133
            GameObject[] allChildren = new GameObject[transform.childCount];
            int i = 0;
            foreach (Transform child in transform)
            {
                allChildren[i] = child.gameObject;
                i++;
            }
            foreach (GameObject child in allChildren) Destroy(child);
        }
    }
}
