public class SkeletonFSM : BaseFSM
{
    public SkeletonManager Manager { get; private set; }
    const float ATTACK_RANGE = 3;
    // const float FOLLOW_RANGE = 100;

    public SkeletonFSM(SkeletonManager manager)
    {
        Manager = manager;
    }

    public void ChangeState(SkeletonFSM newState)
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
        if (Manager.Agent.remainingDistance > 1.3f)
        {
            ChangeState(Manager.Skill1State);
        }
        else
        {
            switch (UnityEngine.Random.Range(0, 2))
            {
                case 0:
                    ChangeState(Manager.KickState);
                    break;

                case 1:
                    ChangeState(Manager.Skill2State);
                    break;
            }
        }
    }

    // protected bool CanPatrol()
    // {
    //     return false;
    // }

    // protected bool CanFollow()
    // {
    //     return ATTACK_RANGE < Manager.Agent.remainingDistance;
    // }

    protected bool CanAttack()
    {
        return Manager.Agent.remainingDistance <= ATTACK_RANGE;
    }
}