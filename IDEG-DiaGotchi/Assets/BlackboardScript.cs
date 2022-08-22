using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackboardScript : MonoBehaviour, IScriptedActionListener
{
    public SC_FPSController scController = null;

    public void ScriptedActionPerformed(int actionId)
    {
        int stringId = 53 + (actionId - 10001);

        string completeText = "";
        for (int i = 53; i <= stringId; i++)
            completeText += Strings.Get(i) + "\r\n";

        var go = gameObject.transform.Find("Canvas/tabuleMainText");
        if (go != null)
        {
            var txt = go.GetComponent<Text>();
            if (txt != null)
                txt.text = completeText;
        }
    }

    void Start()
    {
        scController.SubscribeForScriptedActions(this, new List<int>{ 10001, 10002, 10003, 10004, 10005, 10006, 10007, 10008 });
    }

    void OnDestroy()
    {
        //scController.UnsubscribeFromAllScriptedActions(this);
    }
}
