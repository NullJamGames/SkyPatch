using UnityEngine;
using UnityEngine.AI;

namespace NJG.Runtime.Entity
{
    public class EnemyAttackState : EnemyBaseState
    {
        private readonly NavMeshAgent _agent;
        private readonly Transform _target;

        public EnemyAttackState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform target) : base(enemy,
            animator
        )
        {
            _agent = agent;
            _target = target;
        }

        public override void OnEnter()
        {
            _animator.CrossFade(_attackHash, _crossFadeDuration);
        }

        public override void Update()
        {
            _agent.SetDestination(_target.position);
            _enemy.Attack();
        }
    }
}