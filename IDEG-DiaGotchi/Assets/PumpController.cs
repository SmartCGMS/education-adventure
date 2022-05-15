using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PumpController : MonoBehaviour
{
    private float CurrentSelectedBolus = 0;

    public Text CurrentBolusText;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void UpdateBolusText()
    {
        CurrentBolusText.text = string.Format("{0:0.0} U", CurrentSelectedBolus);
    }

    public void BolusAddVal()
    {
        CurrentSelectedBolus += 0.2f;
        UpdateBolusText();
    }

    public void BolusSubVal()
    {
        CurrentSelectedBolus = Mathf.Max(0.0f, CurrentSelectedBolus - 0.2f);
        UpdateBolusText();
    }

    public void BolusDose()
    {
        PlayerStatsScript.Current.DoseBolus(CurrentSelectedBolus);

        CurrentSelectedBolus = 0.0f;
        UpdateBolusText();
    }
}
