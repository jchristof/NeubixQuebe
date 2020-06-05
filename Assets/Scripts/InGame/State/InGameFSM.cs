using InGame.State.ChallengeModeStates;

namespace InGame {
    public class InGameFsm : FSM<InGameFsm> {
        public InGameFsm(GameBehavior gameBehavior, float levelTime, GameType gameType) {
            this.gameBehavior = gameBehavior;
            this.gameType = gameType;
            
            if(gameType == GameType.Challenge)
                SetState(typeof(StartGameState), levelTime);
            else if(gameType == GameType.Endless)
                SetState(typeof(EndlessGameStart), levelTime);
        }

        public readonly GameBehavior gameBehavior;
        public float levelCompletionTime;
        private GameType gameType;

        public bool CanPause() {
            return IsInAny(typeof(GamePlay));
        }

        public void GameCompleted() {
            if(gameType == GameType.Challenge)
                SetState(typeof(SuccessTransition), false);
            else if(gameType == GameType.Endless)
                SetState(typeof(VisualTransitionOut));
            
            
        }
    }
}