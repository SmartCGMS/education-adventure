using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiabetologistScript : MonoBehaviour, InteractiveObject, InteractiveObjectCondition, IScriptedActionListener
{
    private bool IsTalking = false;

    public BlackoutScript BlackoutPanel = null;

    private bool SensorReady = false;
    private bool PumpReady = false;

    //
    public void Interact()
    {
        if (IsTalking)
            return;

        IsTalking = true;
        SC_FPSController.Current.TalkAll(8);

        SC_FPSController.Current.SubscribeForScriptedAction(this, 10010);
        SC_FPSController.Current.SubscribeForScriptedAction(this, 10011);
        SC_FPSController.Current.SubscribeForScriptedAction(this, 10012);
        SC_FPSController.Current.SubscribeForScriptedAction(this, 10013);
        SC_FPSController.Current.SubscribeForScriptedAction(this, 10014);
        SC_FPSController.Current.SubscribeForScriptedAction(this, 10015);
        SC_FPSController.Current.SubscribeForScriptedAction(this, 10016);
    }

    public bool PreventInteract()
    {
        return IsTalking;
    }

    public void ScriptedActionPerformed(int actionId)
    {
        if (actionId == 10010)
        {
            BlackoutPanel.Blackout(3.0f, () => {
                SC_FPSController.Current.Freeze();
            }, () => {
                SC_FPSController.Current.Unfreeze();
                SC_FPSController.Current.TalkAll(9);
            });
        }
        else if (actionId == 10011)
        {
            BlackoutPanel.Blackout(3.0f, () => {
                SC_FPSController.Current.Freeze();
            }, () => {
                SC_FPSController.Current.Unfreeze();
                SC_FPSController.Current.TalkAll(10);
            });
        }
        else if (actionId == 10012)
        {
            SensorReady = true;
        }
        else if (actionId == 10013 && SensorReady)
        {
            SC_FPSController.Current.TalkAll(11);
            SC_FPSController.Current.PerformScriptedAction(6);
            SensorReady = false;
        }
        else if (actionId == 10014)
        {
            PumpReady = true;
        }
        else if (actionId == 10015 && PumpReady)
        {
            SC_FPSController.Current.TalkAll(12);
            SC_FPSController.Current.PerformScriptedAction(7);
            PumpReady = false;
        }
        else if (actionId == 10016)
        {
            // something?
            SC_FPSController.Current.PerformScriptedAction(5);
        }
    }
}
