using System.Collections.Generic;
using UnityEngine;

public class ChallengeMenu : MonoBehaviour {
    public GameObject controller;
    private List<GameObject> cubes = new List<GameObject>();
    private readonly RaycastHit[] raycastHits = new RaycastHit[5];

    private GameController gameController;

    void Start() {
        gameController = controller.GetComponent<GameController>();
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)){
            gameController.ChallengeMenuBack();
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
            var tile = hit.transform.GetComponent<TileScript>();
            if (tile == null) continue;

            gameController.ChallengeMenuItemSelected(tile.name);
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
