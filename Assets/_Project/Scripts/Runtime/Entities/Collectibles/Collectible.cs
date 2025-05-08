using UnityEngine;

namespace NJG.Runtime.Entity
{
    public class Collectible : Entity
    {
        [SerializeField]
        private int _score = 10; // TODO: Set using Factory
        //[SerializeField]
        //private IntEventChannel _scoreChannel;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                //_scoreChannel.Invoke(_score);
                Destroy(gameObject);
        }
    }
}