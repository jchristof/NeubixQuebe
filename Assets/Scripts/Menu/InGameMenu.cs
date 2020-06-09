using System;
using TMPro;
using UnityEngine;

public class InGameMenu : MonoBehaviour {
    public GameObject time;
    public GameObject challengeNumber;

    private TextMeshProUGUI challengeNumberText;
    public TextMeshProUGUI totalMoveCounter;
    
    public GameController gameController;

    private void Awake() {
        challengeNumberText = challengeNumber.GetComponent<TextMeshProUGUI>();
    }

    public void SetChallengeNumber(string challenge) {
        challengeNumberText.text = challenge;
    }

    public void SetMoveCount(int moveTotal) {
        totalMoveCounter.text = moveTotal.ToString();
    }
}