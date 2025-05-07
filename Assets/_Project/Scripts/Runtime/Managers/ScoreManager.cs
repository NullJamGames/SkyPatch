using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Manager
{
    public class ScoreManager : MonoBehaviour
    {
        [FoldoutGroup("Score"), SerializeField, Tooltip("Max score if player compleates the level under min time")]
        private int _startScore = 1000;
        [FoldoutGroup("Score"), SerializeField, Tooltip("Start losing Score after this time")] 
        private int _lossStartTime = 20;
        [FoldoutGroup("Score"),SerializeField, Tooltip("Score loss per second")] 
        private float _lossSpeed = 1;
        
        private float _currTime = 0f;

        void Update()
        {
            _currTime += Time.deltaTime;
        }

        public int GetScore()
        {
            int score = _startScore;
            
            float _negativeTime = _currTime - _lossStartTime;

            if (_negativeTime > 0)
                score -= (int)(_negativeTime * _lossSpeed);
            
            return score;
        }
        
    }
}
