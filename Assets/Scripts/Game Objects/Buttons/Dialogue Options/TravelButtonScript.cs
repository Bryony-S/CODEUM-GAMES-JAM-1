using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelButtonScript : MonoBehaviour
{
    public WorldManagerScript worldMgr { private get; set; }
    public Area destination { private get; set; }

    /// <summary>
    /// Player travels to destination.
    /// </summary>
    public void Travel()
    {
        worldMgr.Travel(destination);
    }
}
