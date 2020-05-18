using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeTransistion : MonoBehaviour {
    public CubePool cubePool;
    public Material successMaterial;
    public GameObject gameController;
    private Color startColor = Color.green;
    private Color endColor = new Color(0f, 1f, 0f, 0f);

    private float duration = .5f;

    private bool start;

    void Start() {
        
    }

    public void RunSuccessAnimation() {
        //gameObject.GetComponent<MoveScript>().enabled = false;
        StartCoroutine(RunSuccessTiers());
        start = true;
    }

    private List<GameObject> cubes = new List<GameObject>();

    IEnumerator RunSuccessTiers() {
        int row = 5;

        while (row > -1) {
            foreach (var cube in cubePool.cubeGrid) {
                if (cube.transform.position.y == row) {
                    cube.GetComponent<Renderer>().material = new Material(successMaterial);
                    cube.GetComponent<TileScript>().fadeTime = 0f;
                    cubes.Add(cube);
                }
            }

            row--;
            yield return new WaitForSeconds(.25f);
        }
        
        yield return new WaitForSeconds(.25f);
        //gameObject.GetComponent<MoveScript>().enabled = true;
        gameController.GetComponent<GameController>().SuccessAnimDone();
    }

    // Update is called once per frame
    void Update() {
        if (!start)
            return;
        foreach (var cube in cubes) {
            var tilescript = cube.GetComponent<TileScript>();
            cube.GetComponent<Renderer>().material.color =
                Color.Lerp(startColor, endColor, tilescript.fadeTime);
            tilescript.fadeTime += Time.deltaTime / duration;
        }
    }
}