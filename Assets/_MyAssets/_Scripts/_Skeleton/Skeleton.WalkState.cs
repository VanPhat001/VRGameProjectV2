using UnityEngine;

namespace Skeleton
{
    public class WalkState : SkeletonFSM
    {
        private float _timer;

        public WalkState(SkeletonManager manager) : base(manager)
        { }

        public override void EnterState()
        {
            base.EnterState();

            Manager.Animator.SetBool(Manager.WalkParam, true);
            Manager.Agent.isStopped = false;
            Manager.Agent.speed = 2f;
            _timer = 0;
        }

        public override void UpdateState()
        {
            base.UpdateState();

            _timer += Time.deltaTime;
            if (CanAttack())
            {
                PerformAttack();
            }
            else if (_timer >= 2.5f)
            {
                ChangeState(Manager.RunState);
            }
        }

        public override void ExitState()
        {
            base.ExitState();

            Manager.Animator.SetBool(Manager.WalkParam, false);
            Manager.Agent.isStopped = true;
        }
    }
}