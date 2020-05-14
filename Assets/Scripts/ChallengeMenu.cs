using System;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeMenu : MonoBehaviour {
    public GameObject gameController;
    private List<GameObject> cubes = new List<GameObject>();
    private GameMode gameMode;
    public Animation hoverAnimation;
    
    void Start() {
        gameMode = GetComponent<GameMode>();
    }

    private readonly RaycastHit[] raycastHits = new RaycastHit[5];
    private GameObject dragChild;
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)){
          
        }
    }

    private void OnMouseOver() {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Physics.RaycastNonAlloc(ray, raycastHits);

        GameObject hoverCube = null;
        foreach (var hit in raycastHits) {
            var component = hit.transform?.GetComponent(typeof(TileScript));
            if (component == null) continue;
            hoverCube = hit.transform.gameObject;
            
            break;
        }

        if (hoverCube == null)
            return;

        foreach (var cube in cubes) {
            cube.GetComponent<Animator>().SetBool("ToAnimate", cube == hoverCube);
        }
    }

    private void OnMouseExit() {
        foreach (var cube in cubes) {
            cube.GetComponent<Animator>().SetBool("ToAnimate", false);
        }
    }

    private void OnMouseUp() {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Physics.RaycastNonAlloc(ray, raycastHits);

        foreach (var hit in raycastHits) {
            if (hit.transform.GetComponent<TileScript>() == null) continue;
            dragChild = hit.transform.gameObject;
            gameController.GetComponent<GameController>().ChallengeMenuItemSelected();
            break;
        }
    }

    public void OnEnable() {
        cubes = GetComponent<TileField>().GetTiles();
    }

    public void OnDisable() {
        foreach (var cube in cubes) {
            cube.SetActive(false);
        }
    }
}
