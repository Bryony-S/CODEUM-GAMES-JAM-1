using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepButtonScript : MonoBehaviour
{
    public WorldManagerScript worldMgr { private get; set; }

    /// <summary>
    /// Player sleeps in shelter.
    /// </summary>
    public void Sleep()
    {
        worldMgr.AttemptSleep();
    }
}
