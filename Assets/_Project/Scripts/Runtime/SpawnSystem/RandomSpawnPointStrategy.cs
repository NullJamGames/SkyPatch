﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NJG.Runtime.SpawnSystem
{
    public class RandomSpawnPointStrategy : ISpawnPointStrategy
    {
        private readonly Transform[] _spawnPoints;
        private List<Transform> _unusedSpawnPoints;

        public RandomSpawnPointStrategy(Transform[] spawnPoints)
        {
            _spawnPoints = spawnPoints;
            _unusedSpawnPoints = new List<Transform>(spawnPoints);
        }

        public Transform NextSpawnPoint()
        {
            if (!_unusedSpawnPoints.Any())
                _unusedSpawnPoints = new List<Transform>(_spawnPoints);

            int randomIndex = Random.Range(0, _unusedSpawnPoints.Count);
            Transform result = _unusedSpawnPoints[randomIndex];
            _unusedSpawnPoints.RemoveAt(randomIndex);
            return result;
        }
    }
}