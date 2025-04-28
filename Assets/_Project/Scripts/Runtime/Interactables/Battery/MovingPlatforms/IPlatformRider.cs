using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public interface IPlatformRider
    {
        public void AttachToPlatform(Transform platform);
        public void DetachFromPlatform();
    }
}