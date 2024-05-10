using UnityEngine;

namespace Skeleton
{
    public class IdleState : SkeletonFSM
    {
        private float _timer;

        public IdleState(SkeletonManager manager) : base(manager)
        {}

        public override void EnterState()
        {
            base.EnterState();

            Manager.Agent.isStopped = true;
            _timer = 0;
        }

        public override void UpdateState()
        {
            base.UpdateState();

            _timer += Time.deltaTime;
            if (_timer < 2f)
            {
                return;
            }

            if (CanAttack())
            {
                PerformAttack();
            }
            else
            {
                ChangeState(Manager.WalkState);
            }
        }

        public override void ExitState()
        {
            base.ExitState();
        }
    }
}