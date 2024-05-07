namespace CopZombie
{
    public class WalkState : CopZombieFSM
    {
        public WalkState(CopZombieManager manager) : base(manager)
        { }

        public override void EnterState()
        {
            base.EnterState();

            Manager.Animator.SetBool(Manager.WalkParam, true);
            Manager.Agent.isStopped = false;
            Manager.Agent.speed = 2.4f;
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

            Manager.Animator.SetBool(Manager.WalkParam, false);
            Manager.Agent.isStopped = true;
        }
    }
}