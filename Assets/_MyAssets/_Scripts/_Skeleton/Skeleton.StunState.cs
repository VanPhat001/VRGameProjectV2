using System.Collections;
using UnityEngine;

namespace Skeleton
{
    public class StunState : SkeletonFSM
    {
        private float _timer;

        public StunState(SkeletonManager manager) : base(manager)
        { }

        public override void EnterState()
        {
            base.EnterState();

            Manager.Animator.SetBool(Manager.StunParam, true);
            Manager.Animator.SetTrigger(Manager.StunTriggerParam);
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
                Manager.StartCoroutine(PerformSkillCoroutine());
            }

            var isStuning = Manager.Animator.GetBool(Manager.StunParam);
            if (!isStuning)
            {
                ChangeState(Manager.IdleState);
            }
        }

        IEnumerator PerformSkillCoroutine()
        {
            yield break;
        }

        public override void ExitState()
        {
            base.ExitState();

            Manager.ServerRevive(75);
            Manager.Animator.SetBool(Manager.StunParam, false);
            Manager.Animator.ResetTrigger(Manager.StunTriggerParam);
            Manager.IsDeath = false;

            Manager.ServerChangeTarget();
            Manager.BodyCollider.enabled = true;
        }
    }
}