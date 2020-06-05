using System.Collections.Generic;
using System.Linq;
using Menu;
using UnityEngine;
using UnityEngine.Rendering;

namespace DefaultNamespace {
    public class GameModeChallengeMenu : MonoBehaviour {
        public List<Material> challengeRowColors;
        public Sprite crown;
        public CubePool cubePool;
        public List<GameObject> cubes = new List<GameObject>();
        public GameController gameController;
        private readonly RaycastHit[] raycastHits = new RaycastHit[5];
        private ChallengeMenuState.ChallengeMenuFSM fsm;
        void Update() {
            fsm?.Update();
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
            if (!fsm.AllowSelection)
                return;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Physics.RaycastNonAlloc(ray, raycastHits);

            foreach (var hit in raycastHits) {
                var tile = hit.transform.GetComponent<TileScript>();
                if (tile == null) continue;
                fsm.ChallengeSelected(tile.gameObject);
                
                break;
            }
        }

        public void OnEnable() {
            cubes = GetTiles();
            fsm = new ChallengeMenuState.ChallengeMenuFSM(this);
        }

        public void OnDisable() {
            cubes.Clear();
        }

        public List<GameObject> GetTiles() {
            if (cubes.Any())
                return cubes;

            ConfigureCubes();
            return cubes;
        }

        void ConfigureCubes() {
            var savedProgress = gameController.savedProgress.challengeProgress.challenges;
            for (var i = 17; i >= 0; i--) {
                GameObject cube = cubePool.cubeGrid[i];
                cube.SetActive(true);
                cube.transform.position = new Vector3(2 - (i % 3), i / 3, 0);
                var material = challengeRowColors[i / 3];
                if (savedProgress[17 - i].complete) {
                    cube.GetComponent<Renderer>().material = material;
                }
                else {
                    var newMaterial = new Material(material);
                    var color = newMaterial.color;
                    newMaterial.color = new Color(color.r * .3f, color.g * .3f, color.b * .3f, color.a);
                    cube.GetComponent<Renderer>().material = newMaterial;
                }

                cube.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
                cube.GetComponent<TileScript>().text.text = (18 - i).ToString();
                cube.name = (18 - i).ToString();
                cube.GetComponent<TileScript>().image.enabled = false;
                cube.GetComponent<TileScript>().text.enabled = true;
                cubes.Add(cube);
            }

            cubes.Last().GetComponent<TileScript>().image.enabled = true;
            cubes.Last().GetComponent<TileScript>().image.sprite = crown;
            cubes.Last().GetComponent<TileScript>().text.enabled = false;
        }

        public bool CheckSolved(int distance) {
            return false;
        }
    }
}