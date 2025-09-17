using SpiritGarden.Character;
using UnityEngine;

namespace SpiritGarden.Creature.Behaviour
{
    public class CBS_FollowCharacter : CBStateBase
    {
        readonly float duration;

        private Transform target;
        private Vector3 offset;

        private Vector3 pos;
        private float timerDuration = 0f;
        private bool isWithinDuration = false;

        private float previousStoppingDistance;

        public CBS_FollowCharacter (CreatureReference creatureReference, float duration) : base(creatureReference)
        {
            this.duration = duration;
        }

        public override void OnEnter()
        {
            target = creatureReference.RefGame.RefCharacter.GetComponent<CharacterReference>().ModelTransform;
            offset = Vector3.forward + new Vector3(2, 0, 2);

            previousStoppingDistance = creatureReference.Agent.stoppingDistance;
            creatureReference.Agent.stoppingDistance = 3f;

            timerDuration = 0f;
            isWithinDuration = true;
        }

        public override void Update()
        {
            pos = target.position + target.TransformDirection(offset);
            creatureReference.Agent.SetDestination(pos);

            timerDuration += Time.deltaTime;
            if (timerDuration >= duration)
            {
                timerDuration = 0f;
                isWithinDuration = false;
            }
        }

        public override void OnExit()
        {
            creatureReference.Agent.stoppingDistance = previousStoppingDistance;
            creatureReference.Agent.SetDestination(creatureReference.MainTransform.position);

            timerDuration = 0f;
            isWithinDuration = true;
        }

        public bool IsWithinDuration()
        {
            return isWithinDuration;
        }
    }
}