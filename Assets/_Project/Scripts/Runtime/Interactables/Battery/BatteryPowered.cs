using KBCore.Refs;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public abstract class BatteryPowered : ValidatedMonoBehaviour, IActivatable
    {
        public abstract bool IsActive { get; }
        public abstract void Activate();
        public abstract void Deactivate();
    }
}