using KBCore.Refs;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public abstract class BatteryPowered : MonoBehaviour
    {
        public abstract bool IsActive { get; }
        public abstract void Activate();
        public abstract void Deactivate();
    }
}