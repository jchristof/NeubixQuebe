using System;
using System.Collections.Generic;

namespace DefaultNamespace {
    [Serializable]
    public class SavedProgress {
        ChallengeModeProgress challengeProgress = new ChallengeModeProgress();
    }

    public class SingleChallengeProgress {
        public bool complete;
        public int completeTime;
    }
    
    public class ChallengeModeProgress {
        public List<SingleChallengeProgress> challengeProgress = new List<SingleChallengeProgress>();
    }
}