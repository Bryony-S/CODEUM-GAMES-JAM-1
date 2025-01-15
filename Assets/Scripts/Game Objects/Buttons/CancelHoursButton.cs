using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelHoursButton : MonoBehaviour
{
    [SerializeField] private GameObject popupCanvas;

    /// <summary>
    /// Player cancels current action.
    /// </summary>
    public void CancelPopup()
    {
        popupCanvas.SetActive(false);
    }
}
