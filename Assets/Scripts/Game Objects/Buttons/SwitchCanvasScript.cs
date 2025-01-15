using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCanvasScript : MonoBehaviour
{
    [SerializeField] private GameObject oldCanvas;
    [SerializeField] private GameObject newCanvas;

    /// <summary>
    /// Disables old canvas and enables new canvas. Used to switch between UIs/menus.
    /// </summary>
    public void SwitchCanvas()
    {
        oldCanvas.SetActive(false);
        newCanvas.SetActive(true);
    }
}
