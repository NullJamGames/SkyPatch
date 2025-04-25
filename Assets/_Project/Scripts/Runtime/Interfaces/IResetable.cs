using UnityEngine;

namespace NJG.Runtime.Interfaces
{
    public interface IResetable
    {
        public Vector3 StartPosition { get; }
        public void ResetState();
    }
}