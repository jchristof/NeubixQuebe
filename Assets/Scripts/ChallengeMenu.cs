using System.Collections.Generic;
using UnityEngine;

public class ChallengeMenu : MonoBehaviour {
    public GameObject gameController;
    private List<GameObject> cubes = new List<GameObject>();
    private GameMode gameMode;
    
    void Start() {
        cubes = GetComponent<TileField>().GetTiles();
        gameMode = GetComponent<GameMode>();
    }

    private RaycastHit[] raycastHits = new RaycastHit[5];
    private GameObject dragChild;
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)){
          
        }
    }

    private void OnMouseUp() {
        Debug.Log("OnMouseUpEnter");
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Physics.RaycastNonAlloc(ray, raycastHits);

        foreach (var hit in raycastHits) {
            if (hit.transform.GetComponent<TileScript>() == null) continue;
            dragChild = hit.transform.gameObject;
            gameController.GetComponent<GameController>().ChallengeMenuItemSelected();
            break;
        }
        
        Debug.Log("OnMouseUpExit");
    }
}
