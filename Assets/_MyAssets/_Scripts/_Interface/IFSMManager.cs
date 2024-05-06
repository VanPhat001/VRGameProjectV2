using UnityEngine;
using UnityEngine.AI;

public interface IFSMManager<T> where T : BaseFSM
{
    public T CurrentState { get; set; }
    public Transform Target { get; set; }
    public Animator Animator { get; set; }
    public NavMeshAgent Agent { get; set; }
    public bool IsDeath { get; set; }
}