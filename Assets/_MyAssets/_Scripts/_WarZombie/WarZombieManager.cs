using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class WarZombieManager : MonoBehaviour, IFSMManager<WarZombieFSM>
{
    [SerializeField] private Transform _target;
    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _agent;

    public WarZombieFSM CurrentState { get; set; }
    public Transform Target { get => _target; set => _target = value; }
    public Animator Animator { get => _animator; set => _animator = value; }
    public NavMeshAgent Agent { get => _agent; set => _agent = value; }
    public bool IsDeath { get; set; } = false;
    public float HP { get; } = 100;

    public WarZombie.IdleState IdleState { get; private set; }
    public WarZombie.RunState RunState { get; private set; }
    public WarZombie.AttackState AttackState { get; private set; }
    public WarZombie.DeathState DeathState { get; private set; }

    public readonly int RunParam = Animator.StringToHash("run");
    public readonly int AttackParam = Animator.StringToHash("attack");
    public readonly int DeathParam = Animator.StringToHash("death");

    void Awake()
    {
        // if (!NetworkManager.Singleton.IsServer)
        // {
        //     return;
        // }

        IdleState = new(this);
        RunState = new(this);
        AttackState = new(this);
        DeathState = new(this);
    }

    void Start()
    {
        // if (!NetworkManager.Singleton.IsServer)
        // {
        //     return;
        // }

        Agent.destination = Target.position;

        CurrentState = IdleState;
        CurrentState.EnterState();
    }

    void Update()
    {
        // if (!NetworkManager.Singleton.IsServer)
        // {
        //     return;
        // }

        CurrentState.UpdateState();
    }
}