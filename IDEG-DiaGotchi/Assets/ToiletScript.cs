using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToiletScript : MonoBehaviour, InteractiveObject
{
    private bool ToiletInProgress = false;

    public BlackoutScript BlackoutPanel;
    public PlayerStatsScript StatsControllerScript;

    public void Interact()
    {
        if (ToiletInProgress)
            return;

        if (StatsControllerScript.ToiletValue < 0.1f)
        {
            // TODO: talk ("I don't need to go right now")
            return;
        }

        ToiletInProgress = true;

        BlackoutPanel.Blackout(2.0f, () => {
            StatsControllerScript.UseToilet();
        }, () => {
            ToiletInProgress = false;
        });
    }
}
