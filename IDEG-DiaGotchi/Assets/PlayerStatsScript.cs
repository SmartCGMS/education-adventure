using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsScript : MonoBehaviour
{
    // perform step every X seconds (2 seconds is default)
    private static readonly float UpdateTimerRepeat = 2.0f;
    // X minutes per step (5 minutes is default)
    private static readonly float StepIncrement = 5.0f;

    // increment hunger by 0.5% every minute
    private static readonly float HungerIncrement = 0.002f;
    // increment sleepiness by 0.06% every minute (approx. maximum sleepiness after 16 hours)
    private static readonly float SleepinessIncrement = 0.00105f;
    // increment toilet by 0.1% every minute (approx. to force going to toilet at least every 2 hours)
    private static readonly float ToiletIncrement = 0.008f;

    // current timer value
    private float UpdateTimer = UpdateTimerRepeat;

    // current hunger value
    public float HungerValue = 0.1f;
    // current sleepiness value
    public float SleepinessValue = 0.1f;
    // current toilet value
    public float ToiletValue = 0.1f;


    public Text DateText;
    public Text TimeText;

    public TextMeshProUGUI CurrentIGText;
    public Text PumpTextIG;
    public Text PumpTextIOB;

    public GameObject HungerIndicatorPanel;
    public GameObject SleepinessIndicatorPanel;
    public GameObject ToiletIndicatorPanel;

    public Material DaySkybox;
    public Material NightSkybox;
    public Camera MainCamera;

    public DrawControllerGUIScript GUIDrawer;

    public GameObject sceneLight;

    private uint DayOfWeek = 0; // monday
    private uint MinuteOfDay = 6 * 60 + 0; // 6:00

    // SmartCGMS wrapper instance
    private scgms.SCGMS_Game game = new scgms.SCGMS_Game(1, 1, (uint)(StepIncrement * 60 * 1000), "log.csv");

    public static PlayerStatsScript Current;

    private static readonly int IGHistoryCnt = 4;
    private List<double> IGHistory = new List<double>();

    public Texture2D TrendLow3;
    public Texture2D TrendLow2;
    public Texture2D TrendLow1;
    public Texture2D TrendSteady;
    public Texture2D TrendUp1;
    public Texture2D TrendUp2;
    public Texture2D TrendUp3;

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
        UpdateIndicator(ToiletIndicatorPanel, ToiletValue);

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

        string dayPhase = "day";
        if (hour >= 22 || hour <= 5)
            dayPhase = "night";
        else if (hour <= 8)
            dayPhase = "morning";
        else if (hour <= 11)
            dayPhase = "forenoon";
        else if (hour == 12)
            dayPhase = "noon";
        else if (hour <= 16)
            dayPhase = "afternoon";
        else if (hour <= 18)
            dayPhase = "late afternoon";
        else
            dayPhase = "evening";

        TimeText.text = dayPhase;//string.Format("{0}:{1:00}", hour, minute);

        CurrentIGText.text = string.Format("{0:0.0}", game.InterstitialGlucose);
        PumpTextIG.text = string.Format("{0:0.0} mmol/l", game.InterstitialGlucose);
        PumpTextIOB.text = string.Format("{0:0.0} U", game.InsulinOnBoard);

        //Debug.Log("IOB = " + game.InsulinOnBoard);

        // -3, -2, -1, 0, 1, 2, 3
        int igtrend = 0; // flat
        if (IGHistory.Count == IGHistoryCnt)
        {
            double kTotal = 0;

            for (int i = 0; i < IGHistoryCnt - 1; i++)
                kTotal += IGHistory[i + 1] - IGHistory[i];

            if (kTotal > 2.5)
                igtrend = 3;
            else if (kTotal > 1.7)
                igtrend = 2;
            else if (kTotal > 0.8)
                igtrend = 1;
            else if (kTotal > -0.8)
                igtrend = 0;
            else if (kTotal > -1.7)
                igtrend = -1;
            else if (kTotal > -2.5)
                igtrend = -2;
            else
                igtrend = -3;
        }

        //TODO set trend image in pump according to igtrend value
    }

    void Start()
    {
        Current = this;

        UpdateIndicators();
    }

    void PerformGameStep()
    {
        game.Step();
        GUIDrawer.PushIG(game.InterstitialGlucose);

        IGHistory.Add(game.InterstitialGlucose);
        while (IGHistory.Count > IGHistoryCnt)
            IGHistory.RemoveAt(0);

        var beforeMinutes = MinuteOfDay;

        // NOTE: disabled continuous change of time
        /*
        MinuteOfDay += (uint)(StepIncrement);
        if (MinuteOfDay > 1440) // 24h
        {
            MinuteOfDay = 0;
            DayOfWeek = (DayOfWeek + 1) % 7;
        }
        */

        if (beforeMinutes < 360 && MinuteOfDay >= 360) // 6:00
            MainCamera.GetComponent<Skybox>().material = DaySkybox;
        else if (beforeMinutes < 1200 && MinuteOfDay >= 1200) // 20:00
            MainCamera.GetComponent<Skybox>().material = NightSkybox;

        HungerValue = Mathf.Min(1.0f, HungerValue + HungerIncrement * StepIncrement);
        SleepinessValue = Mathf.Min(1.0f, SleepinessValue + SleepinessIncrement * StepIncrement);
        ToiletValue = Mathf.Min(1.0f, ToiletValue + ToiletIncrement * StepIncrement);
    }

    void UpdateEnvironment()
    {
        // NOTE: disabled continuous change of time
        /*
        float interpolant = ((UpdateTimerRepeat - UpdateTimer) / UpdateTimerRepeat) * StepIncrement;
        // this causes the "sun" to be directly above the player on 12:00 and directly under at 0:00
        float xrot = -90.0f + (((float)MinuteOfDay + interpolant) / 1440.0f) * 360.0f;
        */
        float xrot = -90.0f + (((float)MinuteOfDay) / 1440.0f) * 360.0f;

        sceneLight.transform.localEulerAngles = new Vector3(xrot, -130.0f, 0.0f);
        

        var light = sceneLight.GetComponent<Light>();
        if (light != null)
        {
            // midnignt ( 0:00):               intensity = 0
            // morning  ( 6:00): 255, 194, 79, intensity = 1.6
            // noon     (12:00): 255, 213, 96, intensity = 1.4
            // evening  (18:00): 255, 149, 0,  intensity = 1.6
            // midnignt (24:00):               intensity = 0

            float r = 255.0f;
            float g = 0;
            float b = 0;
            float intensity = 1.4f;
            // 0:00 - 12:00
            if (MinuteOfDay <= 720)
            {
                g = 194.0f + (((float)MinuteOfDay) / 720.0f) * (213.0f - 194.0f);
                b = 79.0f + (((float)MinuteOfDay) / 720.0f) * (96.0f - 79.0f);

                if (MinuteOfDay < 360.0f) // 6:00
                {
                    intensity = ((float)MinuteOfDay / 360.0f) * 1.6f;

                    r *= intensity / 1.6f;
                    g *= intensity / 1.6f;
                    b *= intensity / 1.6f;
                }
                else
                    intensity = 1.6f - (((float)MinuteOfDay - 360.0f) / 360.0f) * (1.6f - 1.4f);
            }
            else // 12:00 - 24:00
            {
                g = 213.0f - (((float)MinuteOfDay - 720.0f) / 720.0f) * (213.0f - 149.0f);
                b = 96.0f - (((float)MinuteOfDay - 720.0f) / 720.0f) * 96.0f;

                if (MinuteOfDay < 1080.0f) // 18:00
                    intensity = 1.4f + ((float)(MinuteOfDay - 720.0f) / 360.0f) * (1.6f - 1.4f);
                else
                {
                    intensity = 1.6f - (((float)MinuteOfDay - 1080.0f) / 360.0f) * 1.6f;

                    r *= intensity / 1.6f;
                    g *= intensity / 1.6f;
                    b *= intensity / 1.6f;
                }
            }

            light.color = new Color(r / 255.0f, g / 255.0f, b / 255.0f);
            light.intensity = intensity / 1.6f;
        }
    }

    void Update()
    {
        UpdateTimer -= Time.deltaTime;
        if (UpdateTimer <= 0.0f)
        {
            UpdateTimer = UpdateTimerRepeat;

            PerformGameStep();

            UpdateIndicators();
        }

        UpdateEnvironment();
    }

    public void SetTime(uint hour, uint minute)
    {
        MinuteOfDay = hour * 60 + minute;
        Invoke("UpdateIndicators", 0);
    }

    public uint GetCurrentHour()
    {
        return MinuteOfDay / 60;
    }

    public uint GetCurrentMinute()
    {
        return MinuteOfDay % 60;
    }

    public void SleepFor(int minutes)
    {
        int stepIncrement = (int)StepIncrement;
        // round up to step increments
        if ((minutes % stepIncrement) != 0)
            minutes += stepIncrement - (minutes % stepIncrement);

        int stepCount = minutes / stepIncrement;

        float oldSleepiness = SleepinessValue;
        float sleepDecrement = Mathf.Min(oldSleepiness, Mathf.Min((float)minutes / (8.0f * 60.0f), 1.0f));

        UpdateTimer = UpdateTimerRepeat;

        for (int i = 0; i < stepCount; i++)
            PerformGameStep();

        SleepinessValue = oldSleepiness - sleepDecrement;

        UpdateIndicators();
        UpdateEnvironment();
    }

    public void UseToilet()
    {
        ToiletValue = 0.0f;

        UpdateIndicators();
        UpdateEnvironment();
    }

    public class MealParam
    {
        public float carbs { get; set; }
        public float hungerDec { get; set; }
    }

    public void EatMeal(MealParam meal)
    {
        game.ScheduleCarbohydratesIntake(meal.carbs, 0.0f);
        HungerValue = Mathf.Max(0.0f, HungerValue - meal.hungerDec);
    }

    public void DoseBolus(float val)
    {
        game.ScheduleInsulinBolus((double)val, 0.0f);
    }
}
