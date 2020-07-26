using TMPro;
using UnityEngine;

public class EndlessTimesUpMenu : MonoBehaviour {

    public GameObject Menu => gameObject;
    public TextMeshProUGUI scoreText;
    public Animator newHighScoreAnim;

    public void Start() {
        newHighScoreAnim = gameObject.GetComponentInChildren<Animator>();
    }

    public void OnDisable() {
        int idle = Animator.StringToHash("IdleNewHighScore");
        newHighScoreAnim.Play(idle);
    }

    public void PlayNewHighScoreAnimation() {
        newHighScoreAnim.SetTrigger("NewHighScore");
    }

    public void SetScore(int score) {
        scoreText.text = $"Score: {score}";
    }

}