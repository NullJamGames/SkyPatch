using UnityEngine;
using UnityEngine.AI;

namespace NJG.Runtime.Entity
{
    public class EnemyWanderState : EnemyBaseState
    {
        protected readonly NavMeshAgent _agent;
        protected readonly Vector3 _startPoint;
        protected readonly float _wanderRadius;
        
        public EnemyWanderState(Enemy enemy, Animator animator, NavMeshAgent agent, float wanderRadius) : base(enemy,
            animator
        )
        {
            _agent = agent;
            _startPoint = enemy.transform.position;
            _wanderRadius = wanderRadius;
        }

        public override void OnEnter()
        {
            _animator.CrossFade(_walkHash, _crossFadeDuration);
        }

        public override void Update()
        {
            if (HasReachedDestination())
            {
                Vector3 randomDirection = Random.insideUnitSphere * _wanderRadius;
                randomDirection += _startPoint;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomDirection, out hit, _wanderRadius, 1);
                Vector3 finalPosition = hit.position;
                
                _agent.SetDestination(finalPosition);
            }
        }

        private bool HasReachedDestination()
        {
            return !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance
                && (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f);
        }
    }
}