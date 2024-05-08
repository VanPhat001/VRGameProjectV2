using UnityEngine;

namespace Skeleton
{
    public class Skill1State : SkeletonFSM
    {
        public Skill1State(SkeletonManager manager) : base(manager)
        { }

        public override void EnterState()
        {
            base.EnterState();

            Manager.Animator.SetBool(Manager.Skill1Param, true);
            Manager.BodyCollider.enabled = false;
        }

        public override void UpdateState()
        {
            base.UpdateState();

            var isRunning = Manager.Animator.GetBool(Manager.Skill1Param);
            if (!isRunning)
            {
                ChangeState(Manager.IdleState);
            }
        }

        public override void ExitState()
        {
            base.ExitState();

            Manager.ServerChangeTarget();
            Manager.BodyCollider.enabled = true;
        }

        public override void OnTriggerEnter(Collider collider)
        {
            base.OnTriggerEnter(collider);

            if (!collider.gameObject.CompareTag("NetworkPlayer"))
            {
                return;
            }

            var damageable = collider.transform.GetComponent<IDamageable>();
            damageable?.ServerGetHit(Manager.Skill1Damage);
        }
    }
}