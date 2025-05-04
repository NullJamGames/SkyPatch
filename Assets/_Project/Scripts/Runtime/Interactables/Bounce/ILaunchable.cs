using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public interface ILaunchable
    {
        public void Launch(Vector3 force);
    }
}