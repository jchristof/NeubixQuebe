using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class InGameMenu : MonoBehaviour {
    public GameObject time;

    public TextMeshProUGUI challengeNumberText;
    public TextMeshProUGUI totalMoveCounter;
    public GameObject pauseButton;
    public GameObject Menu => gameObject;

    public void HidePause() {
        pauseButton.SetActive(false);
    }

    public void ShowPause() {
        pauseButton.SetActive(true);
    }
    
    public void SetChallengeNumber(string challenge) {
        challengeNumberText.text = challenge;
    }

    public void SetMoveCount(int moveTotal) {
        totalMoveCounter.text = moveTotal.ToString();
    }
}