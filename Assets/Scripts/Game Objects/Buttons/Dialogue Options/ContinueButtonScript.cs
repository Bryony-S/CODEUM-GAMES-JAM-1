using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueButtonScript : MonoBehaviour
{
    public WorldManagerScript worldMgr { private get; set; }
    public SceneStatus newScene {  private get; set; }

    /// <summary>
    /// Continues to next dialogue scene.
    /// </summary>
    public void ContinueToNextScene()
    {
        worldMgr.ContinueToNextScene(newScene);
    }
}
