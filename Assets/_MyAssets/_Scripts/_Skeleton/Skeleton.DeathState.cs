using Unity.Netcode;
using UnityEngine;

namespace Skeleton
{
    public class DeathState : SkeletonFSM
    {
        private float _timer;

        public DeathState(SkeletonManager manager) : base(manager)
        {}

        public override void EnterState()
        {
            base.EnterState();

            if (Manager.ReviveCount > 0)
            {
                Manager.ReviveCount--;
                ChangeState(Manager.StunState);
                return;
            }

            Manager.RightHand.SetEnabled(false);
            Manager.BodyCollider.enabled = false;

            Manager.Animator.SetBool(Manager.DeathParam, true);
            _timer = 0;
        }

        public override void UpdateState()
        {
            base.UpdateState();

            _timer += Time.deltaTime;
            if (_timer >= 10)
            {
                // Object.Destroy(Manager.gameObject);
                Manager.GetComponent<NetworkObject>().Despawn();
            }
        }

        public override void ExitState()
        {
            base.ExitState();
        }
    }
}