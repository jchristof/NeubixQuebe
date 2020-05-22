using DefaultNamespace;
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
        inGameMenu.SetActive(false);
        inGameMenuScript = inGameMenu.GetComponent<InGameMenu>();
    }

    public void SuccessMenuNextChallenge() {
        successMenu.SetActive(false);
        challengeMenu.SetActive(false);
        cubeCollection.SetActive(true);
        inGameMenu.SetActive(true);
        savedProgress.currentChallenge++;
        inGameMenuScript.SetChallengeNumber((savedProgress.currentChallenge + 1).ToString());
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

    public void ChallengeMenuItemSelected(int challenge) {
        challengeMenu.SetActive(false);
        cubeCollection.SetActive(true);
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
        GetComponentInChildren<GameModeTransistion>().RunSuccessAnimation();
        var currentLevel = savedProgress.currentChallenge;
        var challenge = savedProgress.challengeProgress.challenges[currentLevel];
        challenge.complete = true;
        var savedData = JsonUtility.ToJson(savedProgress);
        PlayerPrefs.SetString("GameProgress",savedData);
        PlayerPrefs.Save();
    }
}