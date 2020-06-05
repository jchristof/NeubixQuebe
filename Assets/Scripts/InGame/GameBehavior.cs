using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame {
    public class GameBehavior : MonoBehaviour {
        public List<Material> challengeRowColors;
        public Material cubeColor0;
        public Material cubeColor1;

        public Material successMaterial;

        public CubePool cubePool;
        public InGameMenu inGameMenu;
        public GameController gameController;

        private InGameFsm inGameState;
        public Game game;
        public MoveScript moveScript;

        public void Init(int gameType, float levelTime) {
            inGameState = new InGameFsm(this, levelTime);
            paused = false;

            game = GameFactory.GetGame(gameType, cubePool.cubeGrid, challengeRowColors);
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