using System;
using System.Collections.Generic;
using MEC;
using NJG.Runtime.Managers;
using TMPro;
using UnityEngine;

namespace NJG.Runtime.UI
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _scoreText;

        private void Start()
        {
            UpdateScore();
        }

        public void UpdateScore()
        {
            Timing.RunCoroutine(UpdateScoreNextFrameRoutine());
        }

        private IEnumerator<float> UpdateScoreNextFrameRoutine()
        {
            yield return Timing.WaitForOneFrame;
            
            int score = GameManager.Instance.Score;
            _scoreText.text = $"Energy: {score}";
        }
    }
}