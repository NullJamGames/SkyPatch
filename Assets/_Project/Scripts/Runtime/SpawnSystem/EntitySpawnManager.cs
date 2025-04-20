using System;
using UnityEngine;

namespace NJG.Runtime.SpawnSystem
{
    public abstract class EntitySpawnManager : MonoBehaviour
    {
        protected enum SpawnPointStrategyType
        {
            Linear,
            Random
        }

        [SerializeField]
        protected SpawnPointStrategyType _spawnPointStrategyType = SpawnPointStrategyType.Linear;
        [SerializeField]
        protected Transform[] _spawnPoints;
        
        protected ISpawnPointStrategy _spawnPointStrategy;

        protected virtual void Awake()
        {
            _spawnPointStrategy = _spawnPointStrategyType switch
            {
                SpawnPointStrategyType.Linear => new LinearSpawnPointStrategy(_spawnPoints),
                SpawnPointStrategyType.Random => new RandomSpawnPointStrategy(_spawnPoints),
                _ => _spawnPointStrategy
            };
        }

        public abstract void Spawn();
    }
}