using UnityEngine;
using UnityEngine.AI;

namespace NJG.Runtime.Entity
{
    public class EnemyChaseState : EnemyBaseState
    {
        private readonly NavMeshAgent _agent;
        private readonly Transform _target;
        
        public EnemyChaseState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform target) : base(enemy,
            animator
        )
        {
            _agent = agent;
            _target = target;
        }
        
        public override void OnEnter()
        {
            _animator.CrossFade(_runHash, _crossFadeDuration);
        }

        public override void Update()
        {
            _agent.SetDestination(_target.position);
        }
    }
}