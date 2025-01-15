using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestButtonScript : MonoBehaviour
{
    public WorldManagerScript worldMgr { private get; set; }

    /// <summary>
    /// Player rests in shelter.
    /// </summary>
    public void Rest()
    {
        worldMgr.AttemptRest();
    }
}
