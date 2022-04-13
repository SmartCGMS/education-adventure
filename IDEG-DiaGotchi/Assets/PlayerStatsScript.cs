using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsScript : MonoBehaviour
{
    // perform step every X seconds (2 seconds is default)
    private float UpdateTimer = 2.0f;
    // X minutes per step (5 minutes is default)
    private static readonly float StepIncrement = 5.0f;


    // increment hunger by 0.5% every 5 minutes
    private float HungerIncrement = 0.005f;
    // increment sleepiness by 0.06% every 5 minutes
    private float SleepinessIncrement = 0.0006f;


    // current hunger value
    public float HungerValue = 0.1f;
    // current sleepiness value
    public float SleepinessValue = 0.1f;


    public Text DateText;
    public Text TimeText;

    public GameObject HungerIndicatorPanel;
    public GameObject SleepinessIndicatorPanel;

    private uint DayOfWeek = 6; // sunday
    private uint MinuteOfDay = 12 * 60 + 0; // 12:00

    // SmartCGMS wrapper instance
    private scgms.SCGMS_Game game = new scgms.SCGMS_Game(1, 1, (uint)(StepIncrement * 60 * 1000), "log.csv");

    private Color GetColorForScale(float value)
    {
        if (value < 0.5f)
            return Color.green;
        if (value < 0.85f)
            return Color.yellow;
        return Color.red;
    }

    private void UpdateIndicator(GameObject obj, float value)
    {
        obj.transform.localScale = new Vector3(value, 1.0f, 1.0f);
        obj.GetComponent<Image>().color = GetColorForScale(value);
    }

    private void UpdateIndicators()
    {
        UpdateIndicator(HungerIndicatorPanel, HungerValue);
        UpdateIndicator(SleepinessIndicatorPanel, SleepinessValue);

        switch (DayOfWeek)
        {
            case 0: DateText.text = "Monday"; break;
            case 1: DateText.text = "Tuesday"; break;
            case 2: DateText.text = "Wednesday"; break;
            case 3: DateText.text = "Thursday"; break;
            case 4: DateText.text = "Friday"; break;
            case 5: DateText.text = "Saturday"; break;
            case 6: DateText.text = "Sunday"; break;
        }

        uint hour = MinuteOfDay / 60;
        uint minute = MinuteOfDay % 60;

        TimeText.text = string.Format("{0}:{1:00}", hour, minute);
    }

    void Start()
    {
        UpdateIndicators();
    }

    void Update()
    {
        UpdateTimer -= Time.deltaTime;
        if (UpdateTimer <= 0.0f)
        {
            UpdateTimer = 2.0f;

            game.Step();

            MinuteOfDay += (uint)(StepIncrement);
            if (MinuteOfDay > 1440) // 24h
            {
                MinuteOfDay = 0;
                DayOfWeek = (DayOfWeek + 1) % 7;
            }

            HungerValue = Mathf.Min(1.0f, HungerValue + HungerIncrement);
            SleepinessValue = Mathf.Min(1.0f, SleepinessValue + SleepinessIncrement);

            UpdateIndicators();
        }
    }
}
