using System.Collections;
using UnityEngine;

public class GameModeTransistion : MonoBehaviour {
    public CubePool cubePool;

    // Start is called before the first frame update
    void Start() {
        StartCoroutine(RunSuccessTiers());
    }

    IEnumerator RunSuccessTiers() {
        yield return new WaitForSeconds(3f);
    }

    // Update is called once per frame
    void Update() { }
}