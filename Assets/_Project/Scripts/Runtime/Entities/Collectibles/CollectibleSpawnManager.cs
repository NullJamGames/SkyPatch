﻿using NJG.Runtime.SpawnSystem;
using NJG.Utilities.ImprovedTimers;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    public class CollectibleSpawnManager : EntitySpawnManager
    {
        [SerializeField]
        private CollectibleData[] _collectibleData;
        [SerializeField]
        private float _spawnInterval = 1f;
        private int _counter;

        private EntitySpawner<Collectible> _spawner;

        private CountdownTimer _spawnTimer;

        protected override void Awake()
        {
            base.Awake();

            _spawner = new EntitySpawner<Collectible>(new EntityFactory<Collectible>(_collectibleData),
                _spawnPointStrategy);

            _spawnTimer = new CountdownTimer(_spawnInterval);
            _spawnTimer.OnTimerStop += () =>
            {
                if (_counter++ >= _spawnPoints.Length)
                {
                    _spawnTimer.Stop();
                    return;
                }

                Spawn();
                _spawnTimer.Start();
            };
        }

        private void Start() => _spawnTimer.Start();

        private void Update() => _spawnTimer.Tick(Time.deltaTime);

        public override void Spawn() => _spawner.Spawn();
    }
}