using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class WaterLeak : MonoBehaviour
    {
        [FoldoutGroup("Settings"), SerializeField, PropertyTooltip("Leak per second")]
        private float _leakSpeed;

        private WaterContainer _waterContainer;

        private void Start()
        {
            _waterContainer = GetComponent<WaterContainer>();
        }

        private void Update()
        {
            _waterContainer.ReduceWater(_leakSpeed * Time.deltaTime);
        }
    }
}