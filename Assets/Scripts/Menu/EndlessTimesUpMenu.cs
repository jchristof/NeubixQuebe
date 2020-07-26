using TMPro;
using UnityEngine;

public class EndlessTimesUpMenu : MonoBehaviour {
    public GameObject Menu => gameObject;
    public TextMeshProUGUI scoreText;

    public void SetScore(int score) {
        scoreText.text = $"Score: {score}";
    }

}