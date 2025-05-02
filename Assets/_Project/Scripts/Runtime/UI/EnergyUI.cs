using System;
using System.Collections.Generic;
using MEC;
using NJG.Runtime.Managers;
using TMPro;
using UnityEngine;
using Zenject;

namespace NJG.Runtime.UI
{
    public class EnergyUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _energyText;

        private GameManager _gameManager;

        [Inject]
        private void Construct(GameManager gameManager)
        {
            _gameManager = gameManager;
        }
        
        private void Start()
        {
            UpdateEnergy();
        }

        public void UpdateEnergy()
        {
            //Timing.RunCoroutine(UpdateEnergyNextFrameRoutine());
        }

        // private IEnumerator<float> UpdateEnergyNextFrameRoutine()
        // {
        //     yield return Timing.WaitForOneFrame;
        //     
        //     //int energy = _gameManager.Energy;
        //     //_energyText.text = $"Energy: {energy}";
        // }
    }
}