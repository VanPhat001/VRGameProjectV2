using UnityEngine;

namespace Skeleton
{
    public class Skill2State : SkeletonFSM
    {
        public Skill2State(SkeletonManager manager) : base(manager)
        { }

        public override void EnterState()
        {
            base.EnterState();

            Manager.Animator.SetBool(Manager.Skill2Param, true);
            Manager.BodyCollider.enabled = false;
            Manager.LeftHand.SetEnabled();
            Manager.RightHand.SetEnabled();
        }

        public override void UpdateState()
        {
            base.UpdateState();

            var isRunning = Manager.Animator.GetBool(Manager.Skill2Param);
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
            Manager.LeftHand.SetEnabled(false);
            Manager.RightHand.SetEnabled(false);
        }

        public override void OnTriggerEnter(Collider collider)
        {
            base.OnTriggerEnter(collider);

            if (!collider.gameObject.CompareTag("NetworkPlayer"))
            {
                return;
            }

            var damageable = collider.transform.GetComponent<IDamageable>();
            damageable?.ServerGetHit(Manager.Skill2Damage);
        }
    }
}