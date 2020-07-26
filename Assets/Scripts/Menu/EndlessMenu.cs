using TMPro;
using UnityEngine;

public class EndlessMenu : MonoBehaviour {
    public GameObject Menu => gameObject;
    public TextMeshProUGUI highScore;

    public void SetHighScore(int score) {
        highScore.text = score.ToString();
    }
}