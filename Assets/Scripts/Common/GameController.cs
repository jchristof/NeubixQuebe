﻿using DefaultNamespace;
using UnityEngine;

public class GameController : MonoBehaviour {
    public GameObject mainMenu;
    public GameObject challengeMenu;
    public GameObject relaxMenu;
    public GameObject cubeCollection;
    public GameObject successTiers;
    public GameObject inGameMenu;
    public GameObject successMenu;
    private InGameMenu inGameMenuScript;

    private SavedProgress _savedProgress;

    private void Awake() {
        string progress = PlayerPrefs.GetString("GameProgress", "");
        _savedProgress = JsonUtility.FromJson<SavedProgress>(progress) ?? new SavedProgress();
    }

    public void Start() {
        mainMenu.SetActive(true);
        challengeMenu.SetActive(false);
        relaxMenu.SetActive(false);
        cubeCollection.SetActive(false);
        successTiers.SetActive(false);
        inGameMenu.SetActive(false);
        inGameMenuScript = inGameMenu.GetComponent<InGameMenu>();
    }

    public void SuccessMenuNextChallenge() {
        successMenu.SetActive(false);
        challengeMenu.SetActive(false);
        cubeCollection.SetActive(true);
        inGameMenu.SetActive(true);
        inGameMenuScript.SetChallengeNumber("13");
    }

    public void SuccessAnimDone() {
        successMenu.SetActive(true);
        cubeCollection.SetActive(false);
    }

    public void SuccessMenuMainMenu() {
        successMenu.SetActive(false);
        mainMenu.SetActive(true);
        cubeCollection.SetActive(false);
        inGameMenu.SetActive(false);
    }

    public void ChallengesClicked() {
        mainMenu.SetActive(false);
        challengeMenu.SetActive(true);
    }

    public void EndlessClicked() { }

    public void RelaxClicked() {
        mainMenu.SetActive(false);
        relaxMenu.SetActive(true);
    }

    public void RelaxBack() {
        mainMenu.SetActive(true);
        relaxMenu.SetActive(false);
    }

    public void ChallengeMenuItemSelected(string challenge) {
        challengeMenu.SetActive(false);
        cubeCollection.SetActive(true);
        inGameMenu.SetActive(true);
        inGameMenuScript.SetChallengeNumber(challenge);
    }

    public void ChallengeMenuBack() {
        challengeMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void GameModeWon() {
        inGameMenu.SetActive(false);
        GetComponentInChildren<GameModeTransistion>().RunSuccessAnimation();
    }
}