using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PumpController : MonoBehaviour
{
    private float CurrentSelectedBolus = 0;
    private float CurrentSelectedBasal = 0;

    public Text CurrentBolusText;
    public Text CurrentBasalText;

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
        if (CurrentSelectedBolus > 5)
            CurrentSelectedBolus = 5;
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


    private void UpdateBasalText()
    {
        CurrentBasalText.text = string.Format("{0:0.0} U/hr", CurrentSelectedBasal);
    }

    public void BasalAddVal()
    {
        CurrentSelectedBasal += 0.2f;
        if (CurrentSelectedBasal > 3)
            CurrentSelectedBasal = 3;
        UpdateBasalText();
    }

    public void BasalSubVal()
    {
        CurrentSelectedBasal = Mathf.Max(0.0f, CurrentSelectedBasal - 0.2f);
        UpdateBasalText();
    }

    public void BasalDose()
    {
        PlayerStatsScript.Current.DoseBasal(CurrentSelectedBasal);
        UpdateBasalText();
    }
}
