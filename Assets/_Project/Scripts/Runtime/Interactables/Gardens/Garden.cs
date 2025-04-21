using System;
using System.Collections.Generic;
using NJG.Runtime.EventChannel;
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
        private float _growTime = 2f;
        [FoldoutGroup("Settings"), SerializeField]
        private int _energyPerHarvest = 10;
        
        [SerializeField, ReadOnly]
        private List<Plot> _plots;
        
        private void OnEnable()
        {
            RegisterPlots();
        }

        private void OnDisable()
        {
            UnregisterPlots();
        }

        [Button(ButtonSizes.Large)]
        private void GeneratePlots()
        {
            _plots ??= new List<Plot>();

            foreach (Plot plot in _plots)
            {
                DestroyImmediate(plot.gameObject);
            }
            
            _plots.Clear();

            for (int x = 0; x < _plotSize.x; x++)
            {
                for (int y = 0; y < _plotSize.y; y++)
                {
                    Plot plot = Instantiate(_plotPrefab, transform);
                    plot.transform.localPosition = new Vector3(x, 0, y);
                    _plots.Add(plot);
                }
            }
        }
        
        private void RegisterPlots()
        {
            if (_plots == null)
                return;

            foreach (Plot plot in _plots)
            {
                plot.Initialize(_growTime);
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