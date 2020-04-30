using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace DefaultNamespace {
    public class ChallengeMenuTiles : MonoBehaviour, TileField { 
        public GameObject roundCornerCube;
        public List<Material> challengeRowColors;
    
        private List<GameObject> cubes = new List<GameObject>();
        void Start() {
            for (var i = 17; i >= 0; i--) {
                GameObject cube = Instantiate(roundCornerCube);
                cube.transform.position = new Vector3(2 - (i % 3), i / 3, 0);
                cube.GetComponent<Renderer>().material = challengeRowColors[i / 3]; //GetMaterial();
                cube.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
                cube.transform.localScale = Vector3.one * .9f;

                cube.GetComponentInChildren<Text>().text = (18 - i).ToString();
                cube.name = (18 - i).ToString();
                //cube.tag = "id here";
                cubes.Add(cube);
            }
        }

        public List<GameObject> GetTiles() {
            return cubes;
        }
    }
}