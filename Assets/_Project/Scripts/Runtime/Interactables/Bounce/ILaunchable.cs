using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public interface ILaunchable
    {
        public void AddForce(Vector3 force);
    }
}