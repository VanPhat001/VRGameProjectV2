using System;
using UnityEngine;

namespace Skeleton
{
    public class Skill1State : SkeletonFSM
    {
        private float _timer;

        public Skill1State(SkeletonManager manager) : base(manager)
        { }

        public override void EnterState()
        {
            base.EnterState();

            Manager.Animator.SetBool(Manager.Skill1Param, true);
            Manager.BodyCollider.enabled = false;
            _timer = 0;
        }

        public override void UpdateState()
        {
            base.UpdateState();

            _timer += Time.deltaTime;
            if (_timer >= 3)
            {
                _timer = float.MinValue;
                Explosion();
            }

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

        private void Explosion()
        {
            // that code excute in server
            // then we have call clientrpc to sync server and client
            var origin = Manager.transform.position;
            var direction = Manager.transform.forward.normalized;
            var n = 4;

            Manager.Skill1Explosion(origin, direction, n);
        }
    }
}