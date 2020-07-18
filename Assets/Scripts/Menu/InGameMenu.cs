using TMPro;
using UnityEngine;

public class InGameMenu : MonoBehaviour {
    public GameObject time;

    public TextMeshProUGUI challengeNumberText;
    public TextMeshProUGUI totalMoveCounter;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI levelCounter;
    
    public GameObject pauseButton;
    public GameObject Menu => gameObject;

    public void HidePause() => pauseButton.SetActive(false);


    public void ShowPause() => pauseButton.SetActive(true);


    public void SetChallengeNumber(string challenge) => challengeNumberText.text = challenge;


    public void SetMoveCount(int moveTotal) => totalMoveCounter.text = moveTotal.ToString();


    public void ShowTimer() => timerText.gameObject.SetActive(true);

    public void HideTimer() => timerText.gameObject.SetActive(false);

    public void ShowMoveCounter() => totalMoveCounter.gameObject.SetActive(true);
    public void HideMoveCounter() => totalMoveCounter.gameObject.SetActive(false);

    public void ShowLevelCounter() => levelCounter.gameObject.SetActive(true);
    public void HideLevelCounter() => levelCounter.gameObject.SetActive(false);

    private void OnEnable() {
        HidePause();
        HideTimer();
        HideMoveCounter();
    }
}