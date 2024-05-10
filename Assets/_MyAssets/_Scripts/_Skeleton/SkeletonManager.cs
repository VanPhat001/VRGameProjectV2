using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SkeletonManager : NetworkBehaviour, IFSMManager<SkeletonFSM>, IDamageable
{
    [SerializeField] private Transform _target;
    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Collider _bodyCollider;
    [SerializeField] private ColliderList _leftHand;
    [SerializeField] private ColliderList _rightHand;
    [SerializeField] private ColliderList _leftFoot;
    [SerializeField] private ColliderList _rightFoot;
    [SerializeField] private Slider _healthbarSlider;
    [SerializeField, Range(0, 100)] private float _damageReductionRate = 0;
    [SerializeField] private float _kickDamage;
    [SerializeField] private float _skill1Damage;
    [SerializeField] private float _skill2Damage;
    [SerializeField] private GameObject _explosionEffectPrefab;
    private NetworkVariable<float> _netHP = new NetworkVariable<float>(100);

    public SkeletonFSM CurrentState { get; set; }
    public Transform Target { get => _target; set => _target = value; }
    public Animator Animator { get => _animator; set => _animator = value; }
    public NavMeshAgent Agent { get => _agent; set => _agent = value; }
    public Collider BodyCollider => _bodyCollider;
    public ColliderList LeftHand => _leftHand;
    public ColliderList RightHand => _rightHand;
    public ColliderList LeftFoot => _leftFoot;
    public ColliderList RightFoot => _rightFoot;
    public bool IsDeath { get; set; } = false;
    public int ReviveCount { get; set; } = 1;

    public float HP => _netHP.Value;
    public float KickDamage => _kickDamage;
    public float Skill1Damage => _skill1Damage;
    public float Skill2Damage => _skill2Damage;
    public GameObject ExplosionEffectPrefab => _explosionEffectPrefab;

    public Skeleton.IdleState IdleState { get; private set; }
    public Skeleton.WalkState WalkState { get; private set; }
    public Skeleton.RunState RunState { get; private set; }
    public Skeleton.KickState KickState { get; private set; }
    public Skeleton.Skill1State Skill1State { get; private set; }
    public Skeleton.Skill2State Skill2State { get; private set; }
    public Skeleton.StunState StunState { get; private set; }
    public Skeleton.DeathState DeathState { get; private set; }

    public readonly int WalkParam = Animator.StringToHash("walk");
    public readonly int RunParam = Animator.StringToHash("run");
    public readonly int DeathParam = Animator.StringToHash("death");
    public readonly int KickParam = Animator.StringToHash("kick");
    public readonly int Skill1Param = Animator.StringToHash("skill1");
    public readonly int Skill2Param = Animator.StringToHash("skill2");
    public readonly int StunParam = Animator.StringToHash("stun");
    public readonly int StunTriggerParam = Animator.StringToHash("stunTrigger");

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
        RunState = new(this);
        KickState = new(this);
        Skill1State = new(this);
        Skill2State = new(this);
        StunState = new(this);
        DeathState = new(this);

        LeftHand.SetEnabled(false);
        RightHand.SetEnabled(false);
        LeftFoot.SetEnabled(false);
        RightFoot.SetEnabled(false);
        Agent.destination = Target.position;

        CurrentState = IdleState;
        CurrentState.EnterState();
    }

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
        _netHP.Value = Mathf.Clamp(HP - damage * (1 - _damageReductionRate / 100f), 0, 100);
        UpdateHealthbar();
    }

    public void ServerRevive(float hp)
    {
        _netHP.Value = hp;
    }

    public void ServerChangeTarget()
    {
        var connectedClients = Multiplayer.Singleton.ConnectedClientDict;
        var keys = new List<ulong>(connectedClients.Keys);
        var random = new System.Random();
        keys = keys.OrderBy(x => random.Next()).ToList();

        foreach (var key in keys)
        {
            if (connectedClients[key].TryGetComponent<NetworkPlayer>(out var netPlayer))
            {
                if (netPlayer.HP > 0)
                {
                    Target = connectedClients[key].transform.GetChild(0).GetChild(0);
                    return;
                }
            }
        }
    }

    public void Skill1Explosion(Vector3 origin, Vector3 direction, int count)
    {
        Skill1ExplosionClientRpc(origin, direction, count);
        PerformSkill1(origin, direction, count);
    }

    [ClientRpc]
    public void Skill1ExplosionClientRpc(Vector3 origin, Vector3 direction, int count)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            return;
        }

        PerformSkill1(origin, direction, count);
    }

    public void PerformSkill1(Vector3 origin, Vector3 direction, int count)
    {
        var pos = origin + direction * 0;
        for (int i = 0; i < count; i++)
        {
            var effect = CFX_SpawnSystem.GetNextObject(_explosionEffectPrefab);
            effect.transform.position = pos;
            effect.GetComponent<ExplosiveSmoke>()?.Explosion();

            pos += direction * 3;
        }
    }
}