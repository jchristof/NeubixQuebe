﻿using DefaultNamespace;
using TMPro;
using UnityEngine;

public class InGamePauseMenu : MonoBehaviour {

    public GameObject Menu => gameObject;
    public GameController gameController;
    public TextMeshProUGUI audioButtonText;

    private void OnEnable() =>
        audioButtonText.text = gameController.AudioPlaying.toOnOffText();

    public void ToggleAudio() {
        gameController.ToggleAudio();
        audioButtonText.text = gameController.AudioPlaying.toOnOffText();
    }

}