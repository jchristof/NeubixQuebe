using UnityEngine;

public class GameController : MonoBehaviour {
    public GameObject mainMenu;
    public GameObject challengeMenu;
    public GameObject cubeCollection;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //mainMenu.
    }

    public void MenuItemSelected() {
        mainMenu.SetActive(false);
        challengeMenu.SetActive(true);
    }

    public void ChallengeMenuItemSelected() {
        challengeMenu.SetActive(false);
        cubeCollection.SetActive(true);
    }

    public void GameModeWon() {
        cubeCollection.SetActive(false);
        mainMenu.SetActive(true);
    }
}
