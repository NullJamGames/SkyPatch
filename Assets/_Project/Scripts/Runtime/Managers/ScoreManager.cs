using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Manager
{
    public enum ERank{S,A,B,C,Loser};
    public class ScoreManager : MonoBehaviour
    {
        [FoldoutGroup("Score"), SerializeField, Tooltip("Max score if player compleates the level under min time")]
        private int _startScore = 1000;
        [FoldoutGroup("Score"), SerializeField, Tooltip("Start losing Score after this time")] 
        private int _lossStartTime = 20;
        [FoldoutGroup("Score"),SerializeField, Tooltip("Score loss per second")] 
        private float _lossSpeed = 1;

        [FoldoutGroup("Rank"), SerializeField, Tooltip("Max score to get S")]
        private int _sRankScore = 750;
        [FoldoutGroup("Rank"), SerializeField, Tooltip("Max score to get A")]
        private int _aRankScore = 650;
        [FoldoutGroup("Rank"), SerializeField, Tooltip("Max score to get B")]
        private int _bRankScore = 500;
        [FoldoutGroup("Rank"), SerializeField, Tooltip("Max score to get C")]
        private int _cRankScore = 300;
        
        private float _currTime = 0f;

        void Update()
        {
            _currTime += Time.deltaTime;
        }

        public SScoreRank GetScoreRank()
        {
            return new SScoreRank(GetScore(), GetRank());
        }

        public int GetScore()
        {
            int score = _startScore;
            
            float _negativeTime = _currTime - _lossStartTime;

            if (_negativeTime > 0)
                score -= (int)(_negativeTime * _lossSpeed);
            
            return score;
        }

        public ERank GetRank()
        {
            int score = GetScore();
            if(score > _sRankScore) return ERank.S;
            if(score > _aRankScore) return ERank.A;
            if(score > _bRankScore) return ERank.B;
            if(score > _cRankScore) return ERank.C;
            return ERank.Loser;
        }
        
    }
    
    public struct SScoreRank
    {
        public int Score { get; private set; }
        public ERank Rank { get; private set; }

        public SScoreRank(int score, ERank rank)
        {
            Score = score;
            Rank = rank;
        }
    }
}
