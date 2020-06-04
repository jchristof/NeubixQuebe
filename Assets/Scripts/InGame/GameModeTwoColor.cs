using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame {
    public class GameModeTwoColor : MonoBehaviour {
        public List<Material> challengeRowColors;
        public Material cubeColor0;
        public Material cubeColor1;

        public Material successMaterial;

        public CubePool cubePool;
        public InGameMenu inGameMenu;
        public GameController gameController;

        private InGameState.InGameFSM inGameState;
        public Game game;
        public MoveScript moveScript;

        public void Init(int gameType, float levelTime) {
            inGameState = new InGameState.InGameFSM(this, levelTime);
            paused = false;

            if (gameType == 0)
                game = new TwoColorGame(cubePool.cubeGrid, new[] {challengeRowColors[0], challengeRowColors[1]});
            else if (gameType == 1)
                game = new ThreeColorGame(cubePool.cubeGrid,
                    new[] {challengeRowColors[0], challengeRowColors[1], challengeRowColors[2]});
            else if (gameType == 2)
                game = new NumberedGame(cubePool.cubeGrid,
                    new[] {challengeRowColors[0], challengeRowColors[1], challengeRowColors[2]});
        }

        private bool paused;

        public void Unpause() {
            paused = false;
        }

        public void Update() {
            if (Input.GetKeyUp(KeyCode.Alpha2))
                inGameState?.RunSuccessAnimation();

            if (inGameState?.CanPause() == true) {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    paused = !paused;
                    if (paused)
                        gameController.InGamePause();
                    else
                        gameController.InGameContinue();
                }
            }

            if (!paused) {
                moveScript?.Update();
                inGameState?.Update();
            }
        }

        void OnMouseDown() {
            moveScript?.OnMouseDown();
        }

        public void OnMouseUp() {
            moveScript?.OnMouseUp();
            if (moveScript != null)
                CheckSolved(1);
        }

        public void OnDisable() {
            inGameState = null;
            game = null;
            moveScript = null;
        }

        void CheckSolved(int distance) {
            if (game.CheckedSolved())
                inGameState.RunSuccessAnimation();
        }
    }
}