using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class OldLadder : MonoBehaviour
    {
        [FoldoutGroup("Pos"), SerializeField]
        private float _topHeight = 3;

        [FoldoutGroup("Pos"), SerializeField]
        private Vector3 _bottomPos;

        [FoldoutGroup("Pos"), SerializeField]
        private Vector3 _topExitPos = new(0, 4, 1);

        public Quaternion ClimbRotation { get; private set; }

        private void OnEnable()
        {
            ClimbRotation = transform.rotation * Quaternion.Euler(0, 180, 0);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.TransformPoint(_bottomPos),
                transform.TransformPoint(_bottomPos + new Vector3(0, _topHeight, 0)));
            Gizmos.DrawSphere(transform.TransformPoint(_bottomPos + new Vector3(0, _topHeight, 0)), 0.1f);
            Gizmos.DrawSphere(transform.TransformPoint(_bottomPos), 0.1f);
            Gizmos.DrawSphere(transform.TransformPoint(_topExitPos), 0.1f);
        }
#endif

        public Vector3 GetClimbPosition() => transform.TransformPoint(_bottomPos);

        public Vector3 GetTopExitPos() => transform.TransformPoint(_topExitPos);

        public float GetBottomHeight() => transform.TransformPoint(_bottomPos).y;

        public float GetTopHeight() => transform.TransformPoint(_bottomPos + new Vector3(0, _topHeight, 0)).y;

        public bool IsCloserToTopPoint(float playerY)
        {
            float distanceToTop = Mathf.Abs(playerY - GetTopHeight());
            float distanceToBottom = Mathf.Abs(playerY - GetBottomHeight());

            return distanceToTop < distanceToBottom;
        }
    }
}