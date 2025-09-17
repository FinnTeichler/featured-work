using SpiritGarden.Character;
using UnityEngine;

namespace SpiritGarden.Creature.Behaviour
{
    public class CBS_OrbitCharacter : CBStateBase
    {
        readonly float timerChangePosMax;
        readonly float radiusMax;
        readonly float duration;

        private Transform target;
        private float timerChangePos = 0f;

        private float timerDuration = 0f;
        private bool isWithinDuration = false;

        public CBS_OrbitCharacter(CreatureReference creatureReference, float timerChangePosMax, float radiusMax, float duration) : base(creatureReference)
        {
            this.timerChangePosMax = timerChangePosMax;
            this.radiusMax = radiusMax;
            this.duration = duration;
        }

        public override void OnEnter()
        {
            isWithinDuration = true;

            target = creatureReference.RefGame.RefCharacter.GetComponent<CharacterReference>().ModelTransform;

            creatureReference.Agent.SetDestination(RandomPointInCircle(target.position, radiusMax));
        }

        public override void Update()
        {
            timerChangePos += Time.deltaTime;
            if (timerChangePos >= timerChangePosMax)
            {
                timerChangePos = 0f;

                creatureReference.Agent.SetDestination(RandomPointInCircle(target.position, radiusMax));
            }

            timerDuration += Time.deltaTime;
            if (timerDuration >= duration)
            {
                timerDuration = 0f;
                isWithinDuration = false;
            }
        }

        public override void OnExit()
        {
            creatureReference.Agent.SetDestination(creatureReference.MainTransform.position);
        }

        private Vector3 RandomPointInCircle(Vector3 origin, float maxRadius)
        {
            Vector3 point = origin + Random.insideUnitSphere * maxRadius;
            point.y = Terrain.activeTerrain.SampleHeight(point) + Terrain.activeTerrain.transform.position.y + 1;

            return point;
        }

        public bool IsWithinDuration()
        {
            return isWithinDuration;
        }
    }
}