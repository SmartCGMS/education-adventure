using UnityEngine;
using System.Collections;

public class Clock : MonoBehaviour {

    public uint minutes = 0;
    public uint hour = 0;
    
    public float clockSpeed = 1.0f;

    int seconds;
    float msecs;
    GameObject pointerSeconds;
    GameObject pointerMinutes;
    GameObject pointerHours;

    void Start() 
    {
        pointerSeconds = transform.Find("rotation_axis_pointer_seconds").gameObject;
        pointerMinutes = transform.Find("rotation_axis_pointer_minutes").gameObject;
        pointerHours   = transform.Find("rotation_axis_pointer_hour").gameObject;

        msecs = 0.0f;
        seconds = 0;
    }

    void Update() 
    {
        msecs += Time.deltaTime * clockSpeed;
        if (msecs >= 1.0f)
        {
            msecs -= 1.0f;
            seconds++;
            if (seconds >= 60)
                seconds = 0;
        }

        if (PlayerStatsScript.Current == null)
            return;

        minutes = PlayerStatsScript.Current.GetCurrentMinute();
        hour = PlayerStatsScript.Current.GetCurrentHour();

        float rotationSeconds = (360.0f / 60.0f)  * seconds;
        float rotationMinutes = (360.0f / 60.0f)  * minutes;
        float rotationHours   = ((360.0f / 12.0f) * hour) + ((360.0f / (60.0f * 12.0f)) * minutes);

        pointerSeconds.transform.localEulerAngles = new Vector3(0.0f, 0.0f, rotationSeconds);
        pointerMinutes.transform.localEulerAngles = new Vector3(0.0f, 0.0f, rotationMinutes);
        pointerHours.transform.localEulerAngles   = new Vector3(0.0f, 0.0f, rotationHours);
    }
}
