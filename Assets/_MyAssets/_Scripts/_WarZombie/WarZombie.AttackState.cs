using UnityEngine;

namespace WarZombie
{
    public class AttackState : WarZombieFSM
    {
        public AttackState(WarZombieManager manager) : base(manager)
        { }

        public override void EnterState()
        {
            base.EnterState();

            Manager.Animator.SetBool(Manager.AttackParam, true);
            Manager.RightHand.SetEnabled();
        }

        public override void UpdateState()
        {
            base.UpdateState();

            var isAttacking = Manager.Animator.GetBool(Manager.AttackParam);
            if (!isAttacking)
            {
                ChangeState(Manager.IdleState);
            }
        }

        public override void ExitState()
        {
            base.ExitState();

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
            damageable.ServerGetHit(Manager.AttackDamage);
        }
    }
}