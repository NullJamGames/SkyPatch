using UnityEngine;

namespace NJG.Runtime.SpawnSystem
{
    public interface ISpawnPointStrategy
    {
        public Transform NextSpawnPoint();
    }
}