namespace CopZombie
{
    public class IdleState : CopZombieFSM
    {
        public IdleState(CopZombieManager manager) : base(manager)
        {}

        public override void EnterState()
        {
            base.EnterState();

            Manager.Agent.isStopped = true;
        }

        public override void UpdateState()
        {
            base.UpdateState();

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