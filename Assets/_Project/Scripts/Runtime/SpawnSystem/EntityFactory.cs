﻿using UnityEngine;

namespace NJG.Runtime.SpawnSystem
{
    public class EntityFactory<T> : IEntityFactory<T> where T : Entity.Entity
    {
        private readonly EntityData[] _data;

        public EntityFactory(EntityData[] data) => _data = data;

        public T Create(Transform spawnPoint)
        {
            EntityData entityData = _data[Random.Range(0, _data.Length)];
            GameObject instance = GameObject.Instantiate(entityData.Prefab, spawnPoint.position, spawnPoint.rotation);
            return instance.GetComponent<T>();
        }
    }
}