using NJG.Runtime.Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class Ladder : MonoBehaviour
    {
        [FoldoutGroup("Pos"), SerializeField] 
        private float _topHeight = 3;

        [FoldoutGroup("Pos"), SerializeField] 
        private Vector3 _bottomPos;
        
        [FoldoutGroup("Pos"), SerializeField] 
        private Vector3 _topExitPos = new Vector3(0, 4, 1);
        
        public Vector3 GetClimbPosition(Vector3 playerPosition)
        {
            return transform.TransformPoint(_bottomPos);
        }

        public Quaternion GetClimbRotation()
        {
            return transform.rotation * Quaternion.Euler(0, 180, 0);
        }

        public Vector3 GetTopExitPos()
        {
            return transform.TransformPoint(_topExitPos);
        }
        public float GetBottomHeight()
        {
            return transform.TransformPoint(_bottomPos).y;
        }

        public float GetTopHeight()
        {
            return transform.TransformPoint(_bottomPos + new Vector3(0, _topHeight, 0)).y;
        }

        public bool IsCloserToTopPoint(float playerY)
        {
            float topY = _bottomPos.y + _topHeight;
            float botY = _bottomPos.y;
            
            float distanceToTop = Mathf.Abs(playerY - topY); 
            float distanceToBottom = Mathf.Abs(playerY - botY); 

            
            return distanceToTop < distanceToBottom;
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
    }
}
