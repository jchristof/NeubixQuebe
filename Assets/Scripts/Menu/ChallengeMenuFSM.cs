using System;
using System.Collections;
using DefaultNamespace;
using UnityEngine;

namespace Menu {
    public class ChallengeMenuState {
        public class ChallengeMenuFSM : FSM<ChallengeMenuFSM> {
            public ChallengeMenuFSM(ChallengeMenuBehavior gameMode) {
                this.gameMode = gameMode;
                AllowSelection = true;
                SetState(typeof(InputIdle));
            }

            public readonly ChallengeMenuBehavior gameMode;

            public void ChallengeSelected(GameObject gameObject) {
                AllowSelection = false;
                SetState(typeof(ChallengeSelected), gameObject);
            }

            public bool AllowSelection;
        }

        class InputIdle : State<ChallengeMenuFSM> {
            public InputIdle(ChallengeMenuFSM fsm) : base(fsm) { }

            public override void Update() {
                base.Update();
                // if (Input.GetKeyDown(KeyCode.Escape)) {
                //     fsm.gameMode.gameController.ChallengeMenuBack();
                // }
            }
        }

        class ChallengeSelected : State<ChallengeMenuFSM> {
            public ChallengeSelected(ChallengeMenuFSM fsm) : base(fsm) { }
            private int selectedChallenge;
            private GameObject selectedCube;
            public override void Pre(object args) {
                base.Pre(args);
                selectedCube = (GameObject) args;
                selectedChallenge = int.Parse(selectedCube.name) - 1;
                fsm.gameMode.StartCoroutine(RunSelectionAnimation());
            }

            public override void Update() {
                base.Update();
            }

            IEnumerator RunSelectionAnimation() {
                float startTime = Time.time;
                Vector3 distancePosition = Vector3.forward * 50f;
                Vector3 centerPosition = new Vector3(1f, 2.5f, -3.5f);
                foreach (var cube in fsm.gameMode.cubes) {
                    cube.GetComponent<TileScript>().savedPosition = cube.transform.position;
                }

                while (true) {
                    foreach (var cube in fsm.gameMode.cubes) {
                        if (cube == selectedCube)
                            cube.transform.position = Vector3.Lerp(cube.GetComponent<TileScript>().savedPosition, centerPosition, (Time.time - startTime) / 1f);
                        else 
                            cube.transform.position = Vector3.Lerp(cube.GetComponent<TileScript>().savedPosition, distancePosition, (Time.time - startTime) / 1f);
                            
                    }

                    if (Time.time - startTime > 2f)
                        break;
                    
                    yield return new WaitForSeconds(.02f);
                }

                fsm.gameMode.gameController.ChallengeMenuItemSelected(selectedChallenge);
            }
        }
    }
}