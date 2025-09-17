using UnityEngine;
using UnityEngine.AI;

namespace SpiritGarden.Creature.Behaviour
{
    public class CBS_Idle : CBStateBase
    {
        private float wanderRadius = 20f;

        private float timerWander = 0f;
        private float timerWanderMax = 5f;

        public CBS_Idle(CreatureReference creatureReference) : base(creatureReference)
        {
            
        }

        public override void OnEnter()
        {
            StandAround();
        }

        public override void Update()
        {
            timerWander += Time.deltaTime;
            if (timerWander >= timerWanderMax)
            {
                timerWander = 0f;
                MoveRandomDirection();
            }

            if(creatureReference.InfoHub.AgentReachedDestination())
            {
                StandAround();
            }
        }

        public override void OnExit()
        {
            
        }

        private void StandAround()
        {
            creatureReference.Agent.SetDestination(creatureReference.MainTransform.position);
        }

        private void MoveRandomDirection()
        {
            Vector3 point;
            if (RandomPoint(creatureReference.MainTransform.position, wanderRadius, out point))
            {
                creatureReference.Agent.SetDestination(point);
            }
        }

        private bool RandomPoint(Vector3 center, float range, out Vector3 result)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector3 randomPoint = center + Random.insideUnitSphere * range;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }
            result = Vector3.zero;
            return false;
        }
    }
}