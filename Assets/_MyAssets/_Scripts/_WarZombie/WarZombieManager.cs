using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class WarZombieManager : NetworkBehaviour, IFSMManager<WarZombieFSM>, IDamageable
{
    [SerializeField] private Transform _target;
    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private ColliderList _rightHand;
    [SerializeField] private Slider _healthbarSlider;

    public WarZombieFSM CurrentState { get; set; }
    public Transform Target { get => _target; set => _target = value; }
    public Animator Animator { get => _animator; set => _animator = value; }
    public NavMeshAgent Agent { get => _agent; set => _agent = value; }
    public ColliderList RightHand => _rightHand;
    public bool IsDeath { get; set; } = false;
    public NetworkVariable<float> netHP = new NetworkVariable<float>(100);
    public float HP => netHP.Value;

    public WarZombie.IdleState IdleState { get; private set; }
    public WarZombie.RunState RunState { get; private set; }
    public WarZombie.AttackState AttackState { get; private set; }
    public WarZombie.DeathState DeathState { get; private set; }

    public readonly int RunParam = Animator.StringToHash("run");
    public readonly int AttackParam = Animator.StringToHash("attack");
    public readonly int DeathParam = Animator.StringToHash("death");

    // void Awake()
    // {
    //     if (!NetworkManager.Singleton.IsServer)
    //     {
    //         return;
    //     }

    //     IdleState = new(this);
    //     RunState = new(this);
    //     AttackState = new(this);
    //     DeathState = new(this);
    // }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        UpdateHealthbar();
        netHP.OnValueChanged += (oldVal, val) => {
            UpdateHealthbar();
        };

        if (!IsServer)
        {
            return;
        }

        IdleState = new(this);
        RunState = new(this);
        AttackState = new(this);
        DeathState = new(this);

        RightHand.SetEnabled(false);
        Agent.destination = Target.position;

        CurrentState = IdleState;
        CurrentState.EnterState();
    }

    // void Start()
    // {
    //     if (!NetworkManager.Singleton.IsServer)
    //     {
    //         return;
    //     }

    //     UpdateHealthbar();

    //     RightHand.SetEnabled(false);
    //     Agent.destination = Target.position;

    //     CurrentState = IdleState;
    //     CurrentState.EnterState();

    //     // Invoke("DeathTest", 4);
    // }

    // void DeathTest()
    // {
    //     HP = 0;
    // }

    void Update()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        CurrentState.UpdateState();
    }


    public void UpdateHealthbar()
    {
        _healthbarSlider.value = HP / 100f;
    }

    public void ServerGetHit(float damage)
    {
        netHP.Value = Mathf.Clamp(HP - damage, 0, 100);
        UpdateHealthbar();
    }

    void ServerChangeTarget()
    {

    }
}