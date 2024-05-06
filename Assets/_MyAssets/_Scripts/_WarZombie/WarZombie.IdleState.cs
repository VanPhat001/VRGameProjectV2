namespace WarZombie
{
    public class IdleState : WarZombieFSM
    {
        public IdleState(WarZombieManager manager) : base(manager)
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
                ChangeState(Manager.RunState);
            }
        }

        public override void ExitState()
        {
            base.ExitState();
        }
    }
}