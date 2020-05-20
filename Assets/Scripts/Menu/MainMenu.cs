using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public GameController gameController;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChallengesClicked() {
       gameController.ChallengesClicked();
    }

    public void EndlessClicked() {
        gameController.EndlessClicked();
    }

    public void RelaxClicked() {
        gameController.RelaxClicked();
    }
}
