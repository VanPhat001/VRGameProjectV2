public class WarZombieFSM : BaseFSM
{
    public WarZombieManager Manager { get; private set; }
    const float ATTACK_RANGE = 1.3f;
    // const float FOLLOW_RANGE = 100;

    public WarZombieFSM(WarZombieManager manager)
    {
        Manager = manager;
    }

    public void ChangeState(WarZombieFSM newState)
    {
        Manager.CurrentState?.ExitState();
        Manager.CurrentState = newState;
        Manager.CurrentState?.EnterState();
    }

    public override void EnterState() { }

    public override void ExitState() { }

    public override void UpdateState()
    {
        if (Manager.IsDeath)
        {
            return;
        }

        if (Manager.HP <= 0)
        {
            Manager.IsDeath = true;
            ChangeState(Manager.DeathState);
            return;
        }

        Manager.Agent.destination = Manager.Target.position;
    }

    protected void PerformAttack()
    {
        ChangeState(Manager.AttackState);
    }

    protected bool CanAttack()
    {
        return Manager.Agent.remainingDistance <= ATTACK_RANGE;
    }
}