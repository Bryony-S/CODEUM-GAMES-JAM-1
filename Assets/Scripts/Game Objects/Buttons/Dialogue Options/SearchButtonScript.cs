using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchButtonScript : MonoBehaviour
{
    public WorldManagerScript worldMgr { private get; set; }

    /// <summary>
    /// Search current shelter for items.
    /// </summary>
    public void SearchShelter()
    {
        worldMgr.AttemptSearch();
    }
}
