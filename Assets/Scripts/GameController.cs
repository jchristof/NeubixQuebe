using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour {
    public GameObject mainMenu;
    public GameObject challengeMenu;
    public GameObject relaxMenu;
    public GameObject cubeCollection;
    public GameObject successTiers;
    public GameObject inGameMenu;
    private InGameMenu inGameMenuScript;

    public void Start() {
        mainMenu.SetActive(true);
        challengeMenu.SetActive(false);
        relaxMenu.SetActive(false);
        cubeCollection.SetActive(false);
        successTiers.SetActive(false);
        inGameMenu.SetActive(false);
        inGameMenuScript = inGameMenu.GetComponent<InGameMenu>();
    }

    public void ChallengesClicked() {
        mainMenu.SetActive(false);
        challengeMenu.SetActive(true);
    }

    public void EndlessClicked() {
        
    }

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
        cubeCollection.SetActive(false);
        successTiers.SetActive(true);
        inGameMenu.SetActive(false);
        StartCoroutine(RunSuccessTiers());
    }
    
    IEnumerator RunSuccessTiers() {
        successTiers.GetComponent<SuccessTiers>().RunSuccessAnimation();
        yield return new WaitForSeconds(3f);
        successTiers.SetActive(false);
        mainMenu.SetActive(true);
    }
}
