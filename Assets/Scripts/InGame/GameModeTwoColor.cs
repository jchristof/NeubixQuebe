using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace InGame {
    public class GameModeTwoColor : MonoBehaviour, TileField, GameMode {
        public List<Material> challengeRowColors;
        public Material cubeColor0;
        public Material cubeColor1;

        public Material successMaterial;

        public CubePool cubePool;
        public InGameMenu inGameMenu;
        public GameController gameController;

        private InGameState.InGameFSM inGameState;
        private Game game;

        public void Start() {
        }

        public void Init(int gameType) {
            inGameState = new InGameState.InGameFSM(this);
            
            if(gameType == 0)
                game = new TwoColorGame(cubePool.cubeGrid, new []{challengeRowColors[0], challengeRowColors[1]});
            else if (gameType == 1) {
                game = new ThreeColorGame(cubePool.cubeGrid, new []{challengeRowColors[0], challengeRowColors[1], challengeRowColors[2]});
            }
        }
        public void Update() {
            if (Input.GetKeyUp(KeyCode.Alpha2))
                inGameState?.RunSuccessAnimation();

            inGameState?.Update();
        }



        public void OnEnable() {
        }

        public void OnDisable() {
            inGameState = null;
            game = null;
        }

        public bool CheckSolved(int distance) {
            if (game.CheckedSolved()) {
                inGameState.RunSuccessAnimation();
                return true;
            }

            return false;
        }

        public List<GameObject> GetTiles() {
            return game?.GetGameTiles();
        }
    }
}