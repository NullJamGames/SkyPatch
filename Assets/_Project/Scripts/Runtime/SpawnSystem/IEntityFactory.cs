using UnityEngine;

namespace NJG.Runtime.SpawnSystem
{
    public interface IEntityFactory<T> where T : Entity.Entity
    {
        public T Create(Transform spawnPoint);
    }
}