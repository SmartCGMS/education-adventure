using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodRecordScript : MonoBehaviour
{
    public float CarbContent = 0.0f;
    public float HungerValue = 0.1f;

    public FoodUIScript FoodUI;

    public void EatButtonClicked()
    {
        PlayerStatsScript.Current.EatMeal(new PlayerStatsScript.MealParam { carbs = CarbContent, hungerDec = HungerValue });

        FoodUI.CloseFoodUI();
    }
}
