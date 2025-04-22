using System;
using System.Collections.Generic;
using DistantLands.Cozy;
using NJG.Runtime.Events;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class Garden : MonoBehaviour
    {
        [FoldoutGroup("References"), SerializeField]
        private Plot _plotPrefab;
        [FoldoutGroup("References"), SerializeField]
        private IntEventChannel _energyEventChannel;
        
        [FoldoutGroup("Settings"), SerializeField]
        private Vector2 _plotSize;
        [FoldoutGroup("Settings"), SerializeField]
        private int _energyPerHarvest = 10;
        
        private Plot[] _plots;
        
        private void OnEnable()
        {
            _plots = GetComponentsInChildren<Plot>();
            
            RegisterPlots();
        }

        private void OnDisable()
        {
            UnregisterPlots();
        }

        public void OnDaytime()
        {
            foreach (Plot plot in _plots)
            {
                plot.SetDayTime(true);
            }
        }
        
        public void OnNighttime()
        {
            foreach (Plot plot in _plots)
            {
                plot.SetDayTime(false);
            }
        }

        [Button(ButtonSizes.Large)]
        private void GeneratePlots()
        {
            if (_plotSize.x > 50 || _plotSize.y > 50)
            {
                Debug.LogError("Plot size is too large, stop trying to break things...");
                return;
            }
            
            Plot[] plots = GetComponentsInChildren<Plot>();
            foreach (Plot plot in plots)
            {
                DestroyImmediate(plot.gameObject);
            }

            for (int x = 0; x < _plotSize.x; x++)
            {
                for (int y = 0; y < _plotSize.y; y++)
                {
                    Plot plot = Instantiate(_plotPrefab, transform);
                    plot.transform.localPosition = new Vector3(x, 0, y);
                }
            }
        }
        
        private void RegisterPlots()
        {
            if (_plots == null)
                return;

            foreach (Plot plot in _plots)
            {
                plot.Initialize();
                plot.OnHarvested += OnPlotHarvested;
            }
        }
        
        private void UnregisterPlots()
        {
            if (_plots == null)
                return;

            foreach (Plot plot in _plots)
            {
                plot.OnHarvested -= OnPlotHarvested;
            }
        }

        private void OnPlotHarvested()
        {
            _energyEventChannel.Invoke(_energyPerHarvest);
        }
    }
}