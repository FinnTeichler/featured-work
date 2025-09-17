using UnityEngine;

namespace SpiritGarden.Creature.Behaviour
{
    public class CBS_RequestInteraction : CBStateBase
    {
        readonly int retryAmount;

        private float duration = 1f;
        private float durationTimer = 0f;
        private int retryCount = 0;
        private float stoppingDistance = 2f;

        private Transform target;

        private bool needsAnimation = false;

        public CBS_RequestInteraction(CreatureReference creatureReference, int retryAmount) : base(creatureReference)
        {
            this.retryAmount = retryAmount;
        }

        public override void OnEnter()
        {
            needsAnimation = true;
            retryCount = 0;
            durationTimer = 0f;

            target = creatureReference.RefGame.RefCharacter.ModelTransform;
        }

        public override void Update()
        {
            Vector3 pos = target.position + target.forward * stoppingDistance;
            creatureReference.Agent.SetDestination(pos);

            if (creatureReference.InfoHub.AgentReachedDestination() && creatureReference.Agent.velocity.magnitude < 1f)
            {
                if (needsAnimation)
                {
                    creatureReference.AnimationControl.PlayEmote(CreatureAnimationControl.Emote.JUMP);

                    retryCount++;
                    needsAnimation = false;
                }

                durationTimer += Time.deltaTime;
                float relativeTime = durationTimer / duration;

                Vector3 direction = creatureReference.RefGame.RefCharacter.ModelTransform.position - creatureReference.MainTransform.position;
                direction.y = 0;
                Quaternion rotation = Quaternion.LookRotation(direction);
                creatureReference.MainTransform.rotation = Quaternion.Slerp(creatureReference.MainTransform.rotation, rotation, relativeTime);

                if (durationTimer >= duration)
                {
                    durationTimer = 0f;
                }
            }
            else
            {
                needsAnimation = true;
            }
        }

        public override void OnExit()
        {
            needsAnimation = true;
            retryCount = 0;
            durationTimer = 0f;

            creatureReference.Agent.SetDestination(creatureReference.MainTransform.position);
        }

        public bool IsDone()
        {
            return retryCount > retryAmount;
        }
    }
}