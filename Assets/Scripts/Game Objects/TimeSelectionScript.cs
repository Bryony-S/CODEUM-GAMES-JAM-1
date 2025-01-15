using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeSelectionScript : MonoBehaviour
{
    [SerializeField] private TMP_Text hoursText;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private GameObject popupCanvas;
    [SerializeField] private WorldManagerScript worldMgr;

    private const int HOURS_MIN = 1;
    public int hoursMax = 24;
    private int currentHoursSelection = HOURS_MIN;
    private Action currentAction;

    /// <summary>
    /// Enable time selection popup.
    /// </summary>
    /// <param name="playerAction">The action the player is attempting to take.</param>
    /// <param name="maxHours">The maximum number of hours the player can perform the action.</param>
    public void SelectTime(Action playerAction, int maxHours)
    {
        hoursMax = maxHours;
        currentHoursSelection = HOURS_MIN;
        currentAction = playerAction;
        switch (currentAction)
        {
            case Action.Searching:
                titleText.text = "Search for how many hours?";
                break;
            case Action.Resting:
                titleText.text = "Rest for how many hours?";
                break;
            case Action.Sleeping:
                titleText.text = "Sleep for how many hours?";
                break;
        }
        UpdateText();
        popupCanvas.SetActive(true);
    }

    /// <summary>
    /// Refreshes hours text.
    /// </summary>
    private void UpdateText()
    {
        hoursText.text = $"{currentHoursSelection}";
    }

    /// <summary>
    /// Increments hour selection by 1.
    /// </summary>
    public void IncrementHours()
    {
        if (currentHoursSelection < hoursMax) currentHoursSelection++;
        UpdateText();
    }

    /// <summary>
    /// Decrements hour selection by 1.
    /// </summary>
    public void DecrementHours()
    {
        if (currentHoursSelection > HOURS_MIN) currentHoursSelection--;
        UpdateText();
    }

    /// <summary>
    /// Player confirms to perform action for selected amount of hours.
    /// </summary>
    public void ConfirmAction()
    {
        popupCanvas.SetActive(false);
        worldMgr.ConfirmTimeSelection(currentAction, currentHoursSelection);
    }
}
