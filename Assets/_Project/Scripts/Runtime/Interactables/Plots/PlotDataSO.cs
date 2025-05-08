using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    [CreateAssetMenu(fileName = "NewPlotData", menuName = "NJG/Data/NewPlotData")]
    public class PlotDataSO : ScriptableObject
    {
        [field: BoxGroup("General"), SerializeField]
        public string PlantName { get; private set; }

        [field: BoxGroup("Prefabs"), SerializeField]
        public GameObject SeedPrefab { get; private set; }
        [field: BoxGroup("Prefabs"), SerializeField]
        public GameObject FullyGrownPrefab { get; private set; }
        [field: BoxGroup("Prefabs"), SerializeField]
        public bool IsHarvestable { get; private set; } = true;
        [field: BoxGroup("Prefabs"), SerializeField, ShowIf(nameof(IsHarvestable))]
        public HarvestedPlant HarvestablePlantPrefab { get; private set; }
    }
}