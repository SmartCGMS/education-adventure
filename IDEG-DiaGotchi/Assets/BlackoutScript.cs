using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackoutScript : MonoBehaviour
{
    private float BlackoutTimer = 0;
    private bool IsInProgress = false;
    private bool BlackoutIn = false;

    private Action OnFullBlackout;
    private Action OnEnd;

    void Update()
    {
        if (IsInProgress)
        {
            BlackoutTimer -= Time.deltaTime;

            if (BlackoutTimer < 0)
            {
                if (BlackoutIn)
                {
                    BlackoutIn = false;
                    BlackoutTimer = 1.0f;

                    OnFullBlackout();
                }
                else
                {
                    IsInProgress = false;
                    BlackoutTimer = 0.0f;

                    OnEnd();
                }
            }

            GetComponent<CanvasGroup>().alpha = (BlackoutIn ? (1.0f - BlackoutTimer) : BlackoutTimer);
        }
    }

    public void Blackout(float durationSecs, Action onFullBlackout, Action onEnd)
    {
        if (!IsInProgress)
        {
            BlackoutTimer = 1.0f;
            IsInProgress = true;
            BlackoutIn = true;

            OnFullBlackout = onFullBlackout;
            OnEnd = onEnd;
        }
    }
}
