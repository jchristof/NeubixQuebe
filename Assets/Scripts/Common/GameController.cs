using System;
using DefaultNamespace;
using InGame;
using UnityEngine;

public class GameController : MonoBehaviour {
    public SplashMenu splashMenu;
    public MainMenu mainMenu;
    public GameObject challengeMenu;
    public GameObject relaxMenu;
    public GameObject cubeCollection;
    public GameObject successTiers;
    public GameObject successMenu;
    public GameObject retryMenu;
    public GameObject inGamePauseMenu;
    private InGameMenu inGameMenu;
    public AudioSource audioSource;

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
        splashMenu.menu.SetActive(true);
        challengeMenu.SetActive(false);
        relaxMenu.SetActive(false);
        cubeCollection.SetActive(false);
        successTiers.SetActive(false);
        successMenu.SetActive(false);
        retryMenu.SetActive(false);
        inGameMenu.menu.SetActive(false);

        splashMenu.audioOnOffText.text = audioSource.mute ? "OFF" : "ON";
    }

    public void ToggleAudio() {
        audioSource.mute = !audioSource.mute;
        splashMenu.audioOnOffText.text = audioSource.mute ? "OFF" : "ON";
        mainMenu.audioOnOffText.text = audioSource.mute ? "OFF" : "ON";
    }
    
    public void SplashStart() {
        splashMenu.menu.SetActive(false);
        mainMenu.menu.SetActive(true);
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

    private GameMode GetGameMode(int challenge) {
        if (challenge < 6)
            return GameMode.TwoColor;
        
       if(challenge < 15) {
            return GameMode.ThreeColor;
        }

        return GameMode.Numbered;
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

        cubeCollection.GetComponent<GameBehavior>().Init(GameType.Challenge, GetGameMode(nextChallenge), GetLevelTime(nextChallenge));
        inGameMenu.menu.SetActive(true);
        savedProgress.currentChallenge = nextChallenge;
        inGameMenu.SetChallengeNumber((savedProgress.currentChallenge + 1).ToString());
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

        cubeCollection.GetComponent<GameBehavior>().Init(GameType.Challenge, GetGameMode(currentChallenge), GetLevelTime(currentChallenge));
        inGameMenu.menu.SetActive(true);
        savedProgress.currentChallenge = currentChallenge;
        inGameMenu.SetChallengeNumber((savedProgress.currentChallenge + 1).ToString());
    }

    public void SuccessMenuMainMenu() {
        retryMenu.SetActive(false);
        successMenu.SetActive(false);
        mainMenu.menu.SetActive(true);
        cubeCollection.SetActive(false);
        inGameMenu.menu.SetActive(false);
    }

    public void ChallengesClicked() {
        mainMenu.menu.SetActive(false);
        challengeMenu.SetActive(true);
    }

    public void EndlessClicked() {
        mainMenu.menu.SetActive(false);
        cubeCollection.SetActive(true);
        cubeCollection.GetComponent<GameBehavior>().Init(GameType.Endless, GetGameMode(0), 0);
    }
    
    public void RelaxStartClicked() {
        relaxMenu.SetActive(false);
        cubeCollection.SetActive(true);
        cubeCollection.GetComponent<GameBehavior>().Init(GameType.Endless, GetGameMode(0), 0);
    }

    public void RelaxClicked() {
        mainMenu.menu.SetActive(false);
        relaxMenu.SetActive(true);
    }

    public void RelaxBack() {
        mainMenu.menu.SetActive(true);
        relaxMenu.SetActive(false);
    }

    public void ChallengeMenuItemSelected(int challenge) {
        challengeMenu.SetActive(false);
        cubeCollection.SetActive(true);
        cubeCollection.GetComponent<GameBehavior>().Init(GameType.Challenge, GetGameMode(challenge), GetLevelTime(challenge));
        inGameMenu.menu.SetActive(true);
        inGameMenu.SetChallengeNumber((challenge + 1).ToString());
        savedProgress.currentChallenge = challenge;
    }

    public void ChallengeMenuBack() {
        challengeMenu.SetActive(false);
        mainMenu.menu.SetActive(true);
    }

    public void GameModeWon(float levelCompletionTime) {
        inGameMenu.menu.SetActive(false);
        cubeCollection.SetActive(false);
        successMenu.GetComponent<SuccessMenu>().SetTime(levelCompletionTime);
        var currentLevel = savedProgress.currentChallenge;
        var challenge = savedProgress.challengeProgress.challenges[currentLevel];
        challenge.complete = true;
        var savedData = JsonUtility.ToJson(savedProgress);
        PlayerPrefs.SetString("GameProgress",savedData);
        PlayerPrefs.Save();
    }

    public void ToggleInGamePause() {
        var paused = inGamePauseMenu.activeInHierarchy;
        if(paused)
            cubeCollection.GetComponent<GameBehavior>().Unpause();
        else {
            cubeCollection.GetComponent<GameBehavior>().Pause();
        }
        inGamePauseMenu.SetActive(!paused);
    }
    public void InGamePause() {
        inGamePauseMenu.SetActive(true);
    }

    public void InGameContinue() {
        inGamePauseMenu.SetActive(false);
        cubeCollection.GetComponent<GameBehavior>().Unpause();
    }
    
    public void InGamePauseMenuMainMenu() {
        inGamePauseMenu.SetActive(false);
        retryMenu.SetActive(false);
        successMenu.SetActive(false);
        mainMenu.menu.SetActive(true);
        cubeCollection.SetActive(false);
        inGameMenu.menu.SetActive(false);
    }
}