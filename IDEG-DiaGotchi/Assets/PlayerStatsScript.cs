using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsScript : MonoBehaviour
{
    private float UpdateTimer = 2.0f;
    private float HungerIncrement = 0.005f;
    private float SleepinessIncrement = 0.0006f;

    public float HungerValue = 0.1f;
    public float SleepinessValue = 0.1f;
    public GameObject HungerIndicatorPanel;
    public GameObject SleepinessIndicatorPanel;

    private scgms.SCGMS_Game game;

    private void UpdateIndicators()
    {
        HungerIndicatorPanel.transform.localScale = new Vector3(HungerValue, 1.0f, 1.0f);
        SleepinessIndicatorPanel.transform.localScale = new Vector3(SleepinessValue, 1.0f, 1.0f);
    }

    void Start()
    {
        UpdateIndicators();

        game = new scgms.SCGMS_Game(1, 1, 5 * 60 * 1000, "log.csv");
    }

    void Update()
    {
        UpdateTimer -= Time.deltaTime;
        if (UpdateTimer <= 0.0f)
        {
            HungerValue = Mathf.Min(1.0f, HungerValue + HungerIncrement);
            SleepinessValue = Mathf.Min(1.0f, SleepinessValue + SleepinessIncrement);

            UpdateIndicators();

            UpdateTimer = 2.0f;

            game.Step();

            Debug.Log(game.InterstitialGlucose);
        }
    }
}
