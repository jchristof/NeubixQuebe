using System.Collections.Generic;
using UnityEngine;

namespace InGame {
    public class GameBehavior : MonoBehaviour {
        public List<Material> challengeRowColors;
        public Material cubeColor0;
        public Material cubeColor1;
        public Material numberedGameMaterial;

        public Material successMaterial;

        public CubePool cubePool;
        public InGameMenu inGameMenu;
        public GameController gameController;
        
        private InGameFsm inGameState;
        public Game game;
        public MoveScript moveScript;
        public int moveTotal;
        public void Init(GameType gameType, GameMode gameMode, float levelTime) {
            game = GameFactory.GetGame(gameMode, cubePool, challengeRowColors, numberedGameMaterial);
            inGameState = new InGameFsm(this, levelTime, gameType);
            paused = false;
        }

        private bool paused;

        public void Unpause() {
            paused = false;
        }

        public void Pause() {
            paused = true;
        }

        public void Update() {
            if (Input.GetKeyUp(KeyCode.Alpha2))
                inGameState?.GameCompleted();

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
            var distance = moveScript?.OnMouseUp() ?? 0;
            moveTotal += distance;
            inGameMenu.SetMoveCount(moveTotal);
            Debug.Log($"distance {distance}");
            if (moveScript != null)
                if (game.CheckedSolved())
                    inGameState.GameCompleted();
        }

        public void OnDisable() {
            moveTotal = 0;
            inGameMenu.SetMoveCount(0);
            inGameMenu.HidePause();
            inGameState = null;
            game = null;
            moveScript = null;
        }
    }
}