using DefaultNamespace;
using InGame;
using UnityEngine;
using UnityEngine.Advertisements;

public class GameController : MonoBehaviour {
    string gameId = "229234ab-51d3-4d34-9eb0-3ac48485a9b8";
    
#if UNITY_ANDROID
    private string storeId = "3741621";
#else //any other build including ios/iphone
    private string storeId = "3741620";
#endif
    
#if DEBUG
    bool testMode = true;
#else
    bool testMode = false;
#endif
    //ads
    //https://www.youtube.com/watch?v=OElh7wda4Qc
    //unit dashboard
    //https://dashboard.unity3d.com/organizations/187602/projects/9fb52e65-e49a-4558-a1ee-f5c27bbc07dc/monetization/monetization-settings

    public SplashMenu splashMenu;
    public MainMenu mainMenu;
    public ChallengeMenu challengeMenu;
    public RelaxMenu relaxMenu;
    public SuccessMenu successMenu;
    public RetryMenu retryMenu;
    public InGamePauseMenu inGamePauseMenu;
    public InGameMenu inGameMenu;
    public EndlessMenu endlessMenu;
    public EndlessTimesUpMenu endlessTimesUpMenu;
    public AudioSource audioSource;

    public GameObject cubeCollection;
    public CubePool cubePool;

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
            for (int i = 0; i < 18; i++)
                savedProgress.challengeProgress.challenges.Add(new SingleChallengeProgress());
        }
    }

    private void AllMenusOff() {
        splashMenu.Menu.SetActive(false);
        mainMenu.Menu.SetActive(false);
        challengeMenu.Menu.SetActive(false);
        relaxMenu.Menu.SetActive(false);
        successMenu.Menu.SetActive(false);
        retryMenu.Menu.SetActive(false);
        inGamePauseMenu.Menu.SetActive(false);
        inGameMenu.Menu.SetActive(false);
        endlessMenu.Menu.SetActive(false);
        endlessTimesUpMenu.Menu.SetActive(false);
    }

    public void Start() {
        AllMenusOff();
        cubeCollection.SetActive(false);
        splashMenu.Menu.SetActive(true);
        splashMenu.audioOnOffText.text = audioSource.mute ? "OFF" : "ON";
        
        Advertisement.Initialize (storeId, testMode);
    }

    public void ToggleAudio() {
        audioSource.mute = !audioSource.mute;
        splashMenu.audioOnOffText.text = audioSource.mute ? "OFF" : "ON";
        mainMenu.audioOnOffText.text = audioSource.mute ? "OFF" : "ON";
    }

    public void SplashStart() {
        splashMenu.Menu.SetActive(false);
        mainMenu.Menu.SetActive(true);
    }

    private void Update() {
        if (Input.GetKeyUp(KeyCode.Alpha3)) {
            savedProgress = new SavedProgress();
            savedProgress.version = prefsVersion;
            for (int i = 0; i < 18; i++)
                savedProgress.challengeProgress.challenges.Add(new SingleChallengeProgress());

            var savedData = JsonUtility.ToJson(savedProgress);
            PlayerPrefs.SetString("GameProgress", savedData);
            PlayerPrefs.Save();
        }
    }

    private GameMode GetGameMode(int challenge) {
        if (challenge < 6)
            return GameMode.TwoColor;

        if (challenge < 15) {
            return GameMode.ThreeColor;
        }

        return GameMode.Numbered;
    }

    private float GetLevelTime(int challenge) {
        if (challenge < 3)
            return 1f;
        if (challenge < 6)
            return .75f;
        if (challenge < 9)
            return 1f;
        if (challenge < 12)
            return .75f;
        if (challenge < 15)
            return .5f;

        if (challenge == 15)
            return 2.5f;
        if (challenge == 16)
            return 2f;
        return 1f;
    }

    public void SuccessMenuNextChallenge() {
        AllMenusOff();
        cubeCollection.SetActive(true);
        inGameMenu.Menu.SetActive(true);

        var nextChallenge = savedProgress.currentChallenge + 1;
        cubeCollection.GetComponent<GameBehavior>()
            .Init(GameType.Challenge, GetGameMode(nextChallenge), GetLevelTime(nextChallenge));
        savedProgress.currentChallenge = nextChallenge;
        inGameMenu.SetChallengeNumber((savedProgress.currentChallenge + 1).ToString());
    }

    public void SuccessAnimDone() {
        AllMenusOff();
        cubeCollection.SetActive(false);
        successMenu.Menu.SetActive(true);
    }

    public void FailedAnimDone() {
        AllMenusOff();
        cubeCollection.SetActive(false);
        retryMenu.Menu.SetActive(true);
    }

    public void Retry() {
        AllMenusOff();
        cubeCollection.SetActive(true);

        var currentChallenge = savedProgress.currentChallenge;

        cubeCollection.GetComponent<GameBehavior>().Init(GameType.Challenge, GetGameMode(currentChallenge),
            GetLevelTime(currentChallenge));
        inGameMenu.Menu.SetActive(true);
        savedProgress.currentChallenge = currentChallenge;
        inGameMenu.SetChallengeNumber((savedProgress.currentChallenge + 1).ToString());
    }

    public void SuccessMenuMainMenu() {
        AllMenusOff();
        mainMenu.Menu.SetActive(true);
        cubeCollection.SetActive(false);
    }

    public void ChallengesClicked() {
        AllMenusOff();
        challengeMenu.Menu.SetActive(true);
    }

    public void EndlessGameClicked() {
        AllMenusOff();
        cubePool.HideAll();
        endlessMenu.Menu.SetActive(true);
        endlessMenu.SetHighScore(savedProgress.endlessHighScore.highScore);
    }
    public void EndlessClicked() {
        AllMenusOff();
        cubePool.HideAll();
        cubeCollection.SetActive(true);
        inGameMenu.Menu.SetActive(true);
        cubeCollection.GetComponent<GameBehavior>().Init(GameType.Endless, GetGameMode(0), 2f);
    }

    public void EndlessTimesUp(int score) {
        AllMenusOff();
        cubeCollection.SetActive(false);
        var currentHighScore = savedProgress.endlessHighScore.highScore;
        endlessTimesUpMenu.Menu.SetActive(true);
        endlessTimesUpMenu.SetScore(score);
        if (currentHighScore < score) {
            savedProgress.endlessHighScore.highScore = score;
            var savedData = JsonUtility.ToJson(savedProgress);
            PlayerPrefs.SetString("GameProgress", savedData);
            PlayerPrefs.Save();
            endlessTimesUpMenu.PlayNewHighScoreAnimation();
        }
    }

    public void RelaxStartClicked() {
        AllMenusOff();
        cubePool.HideAll();
        cubeCollection.SetActive(true);
        inGameMenu.Menu.SetActive(true);
        cubeCollection.GetComponent<GameBehavior>().Init(GameType.Relax, GetGameMode(0), 0);
    }

    public void RelaxClicked() {
        AllMenusOff();
        relaxMenu.Menu.SetActive(true);
    }

    public void RelaxBack() {
        AllMenusOff();
        cubeCollection.SetActive(false);
        mainMenu.Menu.SetActive(true);
    }
    
    public void ChallengeBack() {
        AllMenusOff();
        cubeCollection.SetActive(false);
        mainMenu.Menu.SetActive(true);
    }

    public void ChallengeMenuItemSelected(int challenge) {
        AllMenusOff();

        cubeCollection.SetActive(true);
        cubeCollection.GetComponent<GameBehavior>()
            .Init(GameType.Challenge, GetGameMode(challenge), GetLevelTime(challenge));
        inGameMenu.Menu.SetActive(true);
        inGameMenu.SetChallengeNumber((challenge + 1).ToString());
        savedProgress.currentChallenge = challenge;
    }

    public void ChallengeMenuBack() {
        AllMenusOff();
        mainMenu.Menu.SetActive(true);
    }

    public void GameModeWon(float levelCompletionTime) {
        AllMenusOff();

        cubeCollection.SetActive(false);
        successMenu.Menu.SetActive(true);
        successMenu.SetTime(levelCompletionTime);
        var currentLevel = savedProgress.currentChallenge;
        var challenge = savedProgress.challengeProgress.challenges[currentLevel];
        challenge.complete = true;
        var savedData = JsonUtility.ToJson(savedProgress);
        PlayerPrefs.SetString("GameProgress", savedData);
        PlayerPrefs.Save();

        ShowInterstitialAd();
    }
    
    public void ShowInterstitialAd() {
        // Check if UnityAds ready before calling Show method:
        if (Advertisement.IsReady()) {
            Advertisement.Show();
        } 
        else {
            Debug.Log("Interstitial ad not ready at the moment! Please try again later!");
        }
    }

    public void ToggleInGamePause() {
        var paused = inGamePauseMenu.Menu.activeInHierarchy;
        if (paused)
            cubeCollection.GetComponent<GameBehavior>().Unpause();
        else {
            cubeCollection.GetComponent<GameBehavior>().Pause();
        }

        inGamePauseMenu.Menu.SetActive(!paused);
    }

    public void InGamePause() {
        inGamePauseMenu.Menu.SetActive(true);
    }

    public void InGameContinue() {
        inGamePauseMenu.Menu.SetActive(false);
        cubeCollection.GetComponent<GameBehavior>().Unpause();
    }

    public void InGamePauseMenuMainMenu() {
        AllMenusOff();
        cubeCollection.SetActive(false);
        mainMenu.Menu.SetActive(true);
    }
}