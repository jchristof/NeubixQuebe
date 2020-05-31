using System;
using TMPro;
using UnityEngine;

public class InGameMenu : MonoBehaviour {
    public GameObject time;
    public GameObject challengeNumber;

    private TextMeshProUGUI challengeNumberText;
    public GameController gameController;

    private void Awake() {
        challengeNumberText = challengeNumber.GetComponent<TextMeshProUGUI>();
    }

    public void SetChallengeNumber(string challenge) {
        challengeNumberText.text = challenge;
    }
}