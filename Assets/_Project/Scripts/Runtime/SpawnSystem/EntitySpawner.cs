namespace NJG.Runtime.SpawnSystem
{
    public class EntitySpawner<T> where T : Entity.Entity
    {
        private readonly IEntityFactory<T> _entityFactory;
        private readonly ISpawnPointStrategy _spawnPointStrategy;

        public EntitySpawner(IEntityFactory<T> entityFactory, ISpawnPointStrategy spawnPointStrategy)
        {
            _entityFactory = entityFactory;
            _spawnPointStrategy = spawnPointStrategy;
        }

        public T Spawn() => _entityFactory.Create(_spawnPointStrategy.NextSpawnPoint());
    }
}