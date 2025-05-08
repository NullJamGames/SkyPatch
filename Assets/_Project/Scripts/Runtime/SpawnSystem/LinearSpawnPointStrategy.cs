using UnityEngine;

namespace NJG.Runtime.SpawnSystem
{
    public class LinearSpawnPointStrategy : ISpawnPointStrategy
    {
        private readonly Transform[] _spawnPoints;
        private int _index;

        public LinearSpawnPointStrategy(Transform[] spawnPoints) => _spawnPoints = spawnPoints;

        public Transform NextSpawnPoint()
        {
            Transform result = _spawnPoints[_index];
            _index = (_index + 1) % _spawnPoints.Length;
            return result;
        }
    }
}