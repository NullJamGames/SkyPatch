using UnityEngine;

namespace NJG.Runtime.Pickupables
{
    public class Bucket : PickupableItem
    {
        [SerializeField]
        private GameObject _waterVisual;
        
        public bool HasWater { get; private set; }
        
        public bool TryFillWater()
        {
            if (HasWater)
                return false;

            HasWater = true;
            _waterVisual.SetActive(true);
            return true;
        }

        public bool TryEmptyWater()
        {
            if (!HasWater)
                return false;

            HasWater = false;
            _waterVisual.SetActive(false);
            return true;
        }
    }
}
