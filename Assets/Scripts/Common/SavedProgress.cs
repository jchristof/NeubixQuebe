using System;
using System.Collections.Generic;

namespace DefaultNamespace {
    [Serializable]
    public class SavedProgress {
        public int version;
        public ChallengeModeProgress challengeProgress = new ChallengeModeProgress();
        public int currentChallenge;
        public EndlessHighScore endlessHighScore = new EndlessHighScore();
    }

    [Serializable]
    public class SingleChallengeProgress {
        public bool complete;
        public int completeTime;
    }
    
    [Serializable]
    public class ChallengeModeProgress {
        public List<SingleChallengeProgress> challenges = new List<SingleChallengeProgress>();
    }

    [Serializable]
    public class EndlessHighScore {

        public int highScore;

    }
}