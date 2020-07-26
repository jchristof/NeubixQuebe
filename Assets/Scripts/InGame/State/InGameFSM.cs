using InGame.State.ChallengeModeStates;
using InGame.State.ChallengeModeStates.Relax;
using VisualTransitionOut = InGame.State.ChallengeModeStates.VisualTransitionOut;

namespace InGame {
    public class InGameFsm : FSM<InGameFsm> {
        public InGameFsm(GameBehavior gameBehavior, float levelTime, GameType gameType) {
            this.gameBehavior = gameBehavior;
            this.gameType = gameType;
            
            if(gameType == GameType.Challenge)
                SetState(typeof(StartGameState), levelTime);
            else if(gameType == GameType.Endless)
                SetState(typeof(EndlessGameStart), levelTime);
            else if(gameType == GameType.Relax)
                SetState(typeof(RelaxGameState), 0);
        }

        public readonly GameBehavior gameBehavior;
        public float levelTime;
        public float levelCompletionTime;
        private GameType gameType;
        public int levelCounter;

        public bool CanPause() {
            return IsInAny(typeof(GamePlay));
        }

        public void GameCompleted() {
            if(gameType == GameType.Challenge)
                SetState(typeof(SuccessTransition), false);
            else if(gameType == GameType.Endless)
                SetState(typeof(VisualTransitionOut));
            else if(gameType == GameType.Relax)
                SetState(typeof(InGame.State.ChallengeModeStates.Relax.VisualTransitionOut));
        }
    }
}