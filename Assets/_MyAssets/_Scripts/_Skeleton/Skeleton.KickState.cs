using UnityEngine;

namespace Skeleton
{
    public class KickState : SkeletonFSM
    {
        public KickState(SkeletonManager manager) : base(manager)
        { }

        public override void EnterState()
        {
            base.EnterState();

            Manager.Animator.SetBool(Manager.KickParam, true);
            Manager.BodyCollider.enabled = false;
            Manager.LeftFoot.SetEnabled();
            Manager.RightFoot.SetEnabled();
        }

        public override void UpdateState()
        {
            base.UpdateState();

            var isRunning = Manager.Animator.GetBool(Manager.KickParam);
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
            Manager.LeftFoot.SetEnabled(false);
            Manager.RightFoot.SetEnabled(false);
        }

        public override void OnTriggerEnter(Collider collider)
        {
            base.OnTriggerEnter(collider);

            if (!collider.gameObject.CompareTag("NetworkPlayer"))
            {
                return;
            }

            var damageable = collider.transform.GetComponent<IDamageable>();
            damageable?.ServerGetHit(Manager.KickDamage);
        }
    }
}