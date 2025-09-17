using SpiritGarden.Character;
using UnityEngine;

namespace SpiritGarden.Creature.Behaviour
{
    public class CBS_EvadeCharacter : CBStateBase
    {
        readonly float moveSpeed;
        private Transform target;
        private Vector3 offset;

        private Vector3 posTarget;
        private float moveSpeedNormal;


        public CBS_EvadeCharacter (CreatureReference creatureReference, float moveSpeed) : base(creatureReference)
        {
            this.moveSpeed = moveSpeed;
        }

        public override void OnEnter()
        {
            moveSpeedNormal = creatureReference.Agent.speed;         
            creatureReference.Agent.speed = moveSpeed;

            target = creatureReference.RefGame.RefCharacter.GetComponent<CharacterReference>().ModelTransform;
        }

        public override void Update()
        {
            posTarget = target.position + target.TransformDirection(offset);
            Vector3 direction = (posTarget - creatureReference.MainTransform.position);
            direction = direction * -1;
            creatureReference.Agent.SetDestination(creatureReference.MainTransform.position + direction * 2f);
        }

        public override void OnExit()
        {
            creatureReference.Agent.speed = moveSpeedNormal;
            creatureReference.Agent.SetDestination(creatureReference.MainTransform.position);
        }
    }
}