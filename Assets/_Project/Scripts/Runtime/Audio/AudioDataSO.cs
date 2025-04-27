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
    public EventReference PlayerJump { get; private set; }
    [field: FoldoutGroup("Player SFX"), SerializeField]
    public EventReference PlayerLand { get; private set; }
    [field: FoldoutGroup("Player SFX"), SerializeField]
    public EventReference PlayerDash { get; private set; }
    [field: FoldoutGroup("Player SFX"), SerializeField]
    public EventReference PlayerFall { get; private set; }
    [field: FoldoutGroup("Player SFX"), SerializeField]
    public EventReference PlayerAction { get; private set; }
    
    #endregion

    #region Ambience

    [field: FoldoutGroup("Ambience"), SerializeField]
    public EventReference Ambience { get; private set; }
    [field: FoldoutGroup("Ambience"), SerializeField]
    public EventReference Birds { get; private set; }

    #endregion

    #region UI SFX

    [field: FoldoutGroup("UI SFX"), SerializeField]
    public EventReference UIButtonClick { get; private set; }

    #endregion
    }
}