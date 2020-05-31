using System;
using DefaultNamespace;
using InGame;
using UnityEngine;

public class GameController : MonoBehaviour {
    public GameObject mainMenu;
    public GameObject challengeMenu;
    public GameObject relaxMenu;
    public GameObject cubeCollection;
    public GameObject successTiers;
    public GameObject inGameMenu;
    public GameObject successMenu;
    public GameObject retryMenu;
    public GameObject inGamePauseMenu;
    private InGameMenu inGameMenuScript;

    private int prefsVersion = 1;
    public SavedProgress savedProgress;

    private void Awake() {
        string progress = PlayerPrefs.GetString("GameProgress", "");
        savedProgress = JsonUtility.FromJson<SavedProgress>(progress);
        if (savedProgress?.version != prefsVersion) {
            savedProgress = null;
            PlayerPrefs.DeleteKey("GameProgress");
        }

        if (savedProgress == null) {
            savedProgress = new SavedProgress();
            savedProgress.version = prefsVersion;
            for(int i = 0; i < 18; i++)
                savedProgress.challengeProgress.challenges.Add(new SingleChallengeProgress());
        }
        
    }

    public void Start() {
        mainMenu.SetActive(true);
        challengeMenu.SetActive(false);
        relaxMenu.SetActive(false);
        cubeCollection.SetActive(false);
        successTiers.SetActive(false);
        successMenu.SetActive(false);
        retryMenu.SetActive(false);
        inGameMenu.SetActive(false);
        inGameMenuScript = inGameMenu.GetComponent<InGameMenu>();
    }

    private void Update() {
        if (Input.GetKeyUp(KeyCode.Alpha3)) {
            savedProgress = new SavedProgress();
            savedProgress.version = prefsVersion;
            for(int i = 0; i < 18; i++)
                savedProgress.challengeProgress.challenges.Add(new SingleChallengeProgress());
            
            var savedData = JsonUtility.ToJson(savedProgress);
            PlayerPrefs.SetString("GameProgress",savedData);
            PlayerPrefs.Save();
        }
    }

    private int GetGameMode(int challenge) {
        if (challenge < 6)
            return 0;
        else if(challenge < 15) {
            return 1;
        }
        else {
            return 2;
        }
    }

    private float GetLevelTime(int challenge) {
        if (challenge < 3)
            return 1f;
        else if (challenge < 6)
            return .75f;
        else if(challenge < 9)
            return 1f;
        else if (challenge < 12)
            return .75f;
        else if (challenge < 15)
            return .5f;
        
        else if (challenge == 15)
            return 2.5f;
        else if (challenge == 16)
            return 2f;
        else return 1f;
    }
    public void SuccessMenuNextChallenge() {
        successMenu.SetActive(false);
        retryMenu.SetActive(false);
        challengeMenu.SetActive(false);
        cubeCollection.SetActive(true);
        
        var nextChallenge = savedProgress.currentChallenge + 1;

        cubeCollection.GetComponent<GameModeTwoColor>().Init(GetGameMode(nextChallenge), GetLevelTime(nextChallenge));
        inGameMenu.SetActive(true);
        savedProgress.currentChallenge = nextChallenge;
        inGameMenuScript.SetChallengeNumber((savedProgress.currentChallenge + 1).ToString());
    }

    public void SuccessAnimDone() {
        successMenu.SetActive(true);
        cubeCollection.SetActive(false);
    }
    
    public void FailedAnimDone() {
        retryMenu.SetActive(true);
        cubeCollection.SetActive(false);
    }

    public void Retry() {
        retryMenu.SetActive(false);
        relaxMenu.SetActive(false);
        challengeMenu.SetActive(false);
        cubeCollection.SetActive(true);
        
        var currentChallenge = savedProgress.currentChallenge;

        cubeCollection.GetComponent<GameModeTwoColor>().Init(GetGameMode(currentChallenge), GetLevelTime(currentChallenge));
        inGameMenu.SetActive(true);
        savedProgress.currentChallenge = currentChallenge;
        inGameMenuScript.SetChallengeNumber((savedProgress.currentChallenge + 1).ToString());
    }

    public void SuccessMenuMainMenu() {
        retryMenu.SetActive(false);
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

    public void ChallengeMenuItemSelected(int challenge) {
        challengeMenu.SetActive(false);
        cubeCollection.SetActive(true);
        cubeCollection.GetComponent<GameModeTwoColor>().Init(GetGameMode(challenge), GetLevelTime(challenge));
        inGameMenu.SetActive(true);
        inGameMenuScript.SetChallengeNumber((challenge + 1).ToString());
        savedProgress.currentChallenge = challenge;
    }

    public void ChallengeMenuBack() {
        challengeMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void GameModeWon() {
        inGameMenu.SetActive(false);
        cubeCollection.SetActive(false);
        var currentLevel = savedProgress.currentChallenge;
        var challenge = savedProgress.challengeProgress.challenges[currentLevel];
        challenge.complete = true;
        var savedData = JsonUtility.ToJson(savedProgress);
        PlayerPrefs.SetString("GameProgress",savedData);
        PlayerPrefs.Save();
    }

    public void InGamePause() {
        inGamePauseMenu.SetActive(true);
    }

    public void InGameContinue() {
        inGamePauseMenu.SetActive(false);
        cubeCollection.GetComponent<GameModeTwoColor>().Unpause();
    }
    
    public void InGamePauseMenuMainMenu() {
        inGamePauseMenu.SetActive(false);
        retryMenu.SetActive(false);
        successMenu.SetActive(false);
        mainMenu.SetActive(true);
        cubeCollection.SetActive(false);
        inGameMenu.SetActive(false);
    }
}