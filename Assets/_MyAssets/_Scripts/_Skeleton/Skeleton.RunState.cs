namespace Skeleton
{
    public class RunState : SkeletonFSM
    {
        public RunState(SkeletonManager manager) : base(manager)
        { }

        public override void EnterState()
        {
            base.EnterState();

            Manager.Animator.SetBool(Manager.RunParam, true);
            Manager.Agent.isStopped = false;
            Manager.Agent.speed = 7f;
        }

        public override void UpdateState()
        {
            base.UpdateState();

            if (CanAttack())
            {
                PerformAttack();
            }
        }

        public override void ExitState()
        {
            base.ExitState();

            Manager.Animator.SetBool(Manager.RunParam, false);
            Manager.Agent.isStopped = true;
        }
    }
}