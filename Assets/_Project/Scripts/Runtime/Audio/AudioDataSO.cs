using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Audio
{
    [CreateAssetMenu(fileName = "AudioData", menuName = "NJG/Audio/AudioData")]
    public class AudioDataSO : ScriptableObject
    {
        #region Music

        [field: FoldoutGroup("Music"), SerializeField]
        public EventReference Music { get; private set; }

        #endregion

        #region Player SFX

        [field: FoldoutGroup("Player SFX"), SerializeField]
        public EventReference PlayerFootstep { get; private set; }

        [field: FoldoutGroup("Player SFX"), SerializeField]
        public EventReference PlayerLand { get; private set; }

        [field: FoldoutGroup("Player SFX"), SerializeField]
        public EventReference PlayerBounce { get; private set; }
        #endregion

        #region Interact SFX

        [field: FoldoutGroup("Interact SFX"), SerializeField]
        public EventReference FillTheBucket { get; private set; }

        [field: FoldoutGroup("Interact SFX"), SerializeField]
        public EventReference PickupPlant { get; private set; }

        [field: FoldoutGroup("Interact SFX"), SerializeField]
        public EventReference WaterPlant { get; private set; }

        [field: FoldoutGroup("Interact SFX"), SerializeField]
        public EventReference RechargingAlarm { get; private set; }

        [field: FoldoutGroup("Interact SFX"), SerializeField]
        public EventReference SolarPanelStatic { get; private set; }

        [field: FoldoutGroup("Interact SFX"), SerializeField]
        public EventReference WaterTree { get; private set; }

        [field: FoldoutGroup("Interact SFX"), SerializeField]
        public EventReference CompostBin { get; private set; }

        [field: FoldoutGroup("Interact SFX"), SerializeField]
        public EventReference Plant { get; private set; }

        #endregion

        #region Ambience

        [field: FoldoutGroup("Ambience"), SerializeField]
        public EventReference Birds { get; private set; }

        [field: FoldoutGroup("Ambience"), SerializeField]
        public EventReference HeavyRain { get; private set; }

        [field: FoldoutGroup("Ambience"), SerializeField]
        public EventReference LightRain { get; private set; }

        [field: FoldoutGroup("Ambience"), SerializeField]
        public EventReference LightWind { get; private set; }

        [field: FoldoutGroup("Ambience"), SerializeField]
        public EventReference WaterfallHeavy { get; private set; }

        [field: FoldoutGroup("Ambience"), SerializeField]
        public EventReference WaterfallMedium { get; private set; }

        #endregion

        #region UI SFX

        [field: FoldoutGroup("UI SFX"), SerializeField]
        public EventReference UIPositive { get; private set; }

        [field: FoldoutGroup("UI SFX"), SerializeField]
        public EventReference UINegative { get; private set; }

        [field: FoldoutGroup("UI SFX"), SerializeField]
        public EventReference UIJobDone { get; private set; }

        #endregion
    }
}