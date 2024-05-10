using DemoObserver;
using Unity.Netcode;
using UnityEngine;

namespace WarZombie
{
    public class DeathState : WarZombieFSM
    {
        private float _timer;

        public DeathState(WarZombieManager manager) : base(manager)
        {}

        public override void EnterState()
        {
            base.EnterState();

            Manager.RightHand.SetEnabled(false);
            Manager.BodyCollider.enabled = false;
            // Manager.gameObject.GetComponent<Collider>().enabled = false;

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