using System;
using TMPro;
using UnityEngine;

public class SuccessMenu : MonoBehaviour {
    public GameObject Menu => gameObject;
    public GameObject Time;

    public void SetTime(float time) {
        TimeSpan t = TimeSpan.FromMilliseconds(time);
        Time.GetComponent<TextMeshProUGUI>().text =
            "Time " + string.Format("{0:D1}:{1:D2}",
                t.Minutes,
                t.Seconds);
    }
}
