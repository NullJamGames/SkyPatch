using System;
using System.Collections.Generic;
using MEC;
using NJG.Runtime.Managers;
using TMPro;
using UnityEngine;
using Zenject;

namespace NJG.Runtime.UI
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _scoreText;

        private GameManager _gameManager;

        [Inject]
        private void Construct(GameManager gameManager)
        {
            _gameManager = gameManager;
        }
        
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
            
            int score = _gameManager.Energy;
            _scoreText.text = $"Energy: {score}";
        }
    }
}