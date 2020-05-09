using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour {
    public GameObject mainMenu;
    public GameObject challengeMenu;
    public GameObject relaxMenu;
    public GameObject cubeCollection;
    public GameObject successTiers;

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
    public void ChallengeMenuItemSelected() {
        challengeMenu.SetActive(false);
        cubeCollection.SetActive(true);
    }

    public void GameModeWon() {
        cubeCollection.SetActive(false);
        successTiers.SetActive(true);
        StartCoroutine(RunSuccessTiers());
    }
    
    IEnumerator RunSuccessTiers() {
        successTiers.GetComponent<SuccessTiers>().RunSuccessAnimation();
        yield return new WaitForSeconds(3f);
        successTiers.SetActive(false);
        mainMenu.SetActive(true);
    }
}
