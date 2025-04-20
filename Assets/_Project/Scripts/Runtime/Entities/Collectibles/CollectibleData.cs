using NJG.Runtime.SpawnSystem;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    [CreateAssetMenu(fileName = "CollectibleData", menuName = "NJG/CollectibleData")]
    public class CollectibleData : EntityData
    {
        public int Score;
    }
}