using UnityEngine;

public class TileScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private ChallengeMenu _challengeMenu;
    public void SetParentComponent(ChallengeMenu challengeMenu) {
        _challengeMenu = challengeMenu;
    }
    
    

    void OnMouseDown() {
        _challengeMenu.ChildMouseDown(gameObject);
    }

    private void OnMouseUp() {
        _challengeMenu.ChildMouseUp();
    }

    void OnMouseDrag() {
        _challengeMenu.ChildMouseDrag();
    }
}

static class VectorExtensions {
    public static Vector3 OffsetX(this Vector3 pos, float distance) {
        return new Vector3(pos.x + distance, pos.y, pos.z);
    }
}