using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CopZombieManager : NetworkBehaviour, IFSMManager<CopZombieFSM>, IDamageable
{
    [SerializeField] private Transform _target;
    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Collider _bodyCollider;
    [SerializeField] private ColliderList _rightHand;
    [SerializeField] private Slider _healthbarSlider;
    [SerializeField] private float _attackDamage;
    private NetworkVariable<float> _netHP = new NetworkVariable<float>(100);

    public CopZombieFSM CurrentState { get; set; }
    public Transform Target { get => _target; set => _target = value; }
    public Animator Animator { get => _animator; set => _animator = value; }
    public NavMeshAgent Agent { get => _agent; set => _agent = value; }
    public Collider BodyCollider => _bodyCollider;
    public ColliderList RightHand => _rightHand;
    public bool IsDeath { get; set; } = false;
    public float HP => _netHP.Value;
    public float AttackDamage => _attackDamage;

    public CopZombie.IdleState IdleState { get; private set; }
    public CopZombie.WalkState WalkState { get; private set; }
    public CopZombie.AttackState AttackState { get; private set; }
    public CopZombie.DeathState DeathState { get; private set; }

    public readonly int WalkParam = Animator.StringToHash("walk");
    public readonly int AttackParam = Animator.StringToHash("attack");
    public readonly int DeathParam = Animator.StringToHash("death");

    // void Awake()
    // {
    //     if (!NetworkManager.Singleton.IsServer)
    //     {
    //         return;
    //     }

    //     IdleState = new(this);
    //     WalkState = new(this);
    //     AttackState = new(this);
    //     DeathState = new(this);
    // }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        UpdateHealthbar();
        _netHP.OnValueChanged += (oldVal, val) =>
        {
            UpdateHealthbar();
        };

        if (!IsServer)
        {
            return;
        }

        IdleState = new(this);
        WalkState = new(this);
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

    private void OnTriggerEnter(Collider other)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        CurrentState.OnTriggerEnter(other);
    }


    public void UpdateHealthbar()
    {
        _healthbarSlider.value = HP / 100f;
    }

    public void ServerGetHit(float damage)
    {
        _netHP.Value = Mathf.Clamp(HP - damage, 0, 100);
        UpdateHealthbar();
    }

    public void ServerChangeTarget()
    {
        var connectedClients = Multiplayer.Singleton.ConnectedClientDict;

        // var index = UnityEngine.Random.Range(0, connectedClients.Keys.Count);
        // var netPlayer = connectedClients.ElementAt(index).Value;
        // Target = netPlayer.transform.GetChild(0).GetChild(0);

        var keys = new List<ulong>(connectedClients.Keys);
        var random = new System.Random();
        keys = keys.OrderBy(x => random.Next()).ToList();
        Target = connectedClients[keys[0]].transform.GetChild(0).GetChild(0);

        // NetPlayer <netPlayer>
        // |--> Head Container <child 0>
        // |-->--> Head <child 0>
    }
}