namespace WarZombie
{
    public class DeathState : WarZombieFSM
    {
        public DeathState(WarZombieManager manager) : base(manager)
        {}

        public override void EnterState()
        {
            base.EnterState();

            Manager.Animator.SetBool(Manager.DeathParam, true);
        }

        public override void UpdateState()
        {
            base.UpdateState();
        }

        public override void ExitState()
        {
            base.ExitState();
        }
    }
}