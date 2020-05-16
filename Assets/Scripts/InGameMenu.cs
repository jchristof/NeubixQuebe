using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour {
    public GameObject time;
    public GameObject challengeNumber;

    private TextMeshProUGUI challengeNumberText;

    void Start() {
        challengeNumberText = challengeNumber.GetComponent<TextMeshProUGUI>();
    }

    public void SetChallengeNumber(string challenge) {
        challengeNumberText.text = challenge;
    }
}
