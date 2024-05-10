using DemoObserver;
using Unity.Netcode;
using UnityEngine;

namespace CopZombie
{
    public class DeathState : CopZombieFSM
    {
        private float _timer;

        public DeathState(CopZombieManager manager) : base(manager)
        {}

        public override void EnterState()
        {
            base.EnterState();

            Manager.RightHand.SetEnabled(false);
            Manager.BodyCollider.enabled = false;

            Manager.Animator.SetBool(Manager.DeathParam, true);
            _timer = 0;

            Manager.PostEvent(EventID.OnZombieDeath);
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