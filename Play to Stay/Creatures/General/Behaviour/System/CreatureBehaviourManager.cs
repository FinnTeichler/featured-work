using UnityEngine;
using SpiritGarden.Creature.Interaction.Data;

namespace SpiritGarden.Creature.Behaviour
{
    public class CreatureBehaviourManager : MonoBehaviour
    {
        [HideInInspector] public CreatureReference creatureReference;

        [SerializeField] private bool debugMode;

        private CreatureBehaviour[] behaviours;
        public CreatureBehaviour BehaviourCurrent {  get { return behaviourCurrent; } }
        private CreatureBehaviour behaviourCurrent;


        private void OnEnable()
        {
            creatureReference.Friendship.OnLevelCurrentChanged += OnFriendshipLevelChanged;
        }

        private void OnDisable()
        {
            creatureReference.Friendship.OnLevelCurrentChanged -= OnFriendshipLevelChanged;
        }

        void Awake()
        {
            behaviours = GetComponents<CreatureBehaviour>();

            foreach (CreatureBehaviour behaviour in behaviours)
            {
                behaviour.creatureReference = creatureReference;
                behaviour.debugMode = debugMode;
            }

            ChooseBehaviour(creatureReference.Friendship.EvaluateLevelCurrent());
        }

        private void ChooseBehaviour(FriendshipLevel friendLevel)
        {
            //if (behaviourCurrent)
                //behaviourCurrent.Unsubscribe();

            bool foundMatchingBehaviour = false;
            foreach (CreatureBehaviour behaviour in behaviours)
            {
                behaviour.enabled = false;
                if (behaviour.BehaviourID.Equals(friendLevel.BehaviourID))
                {
                    behaviourCurrent = behaviour;
                    //behaviourCurrent.Subscribe();
                    behaviourCurrent.enabled = true;
                    
                    foundMatchingBehaviour = true;
                }                   
            }

            if (!foundMatchingBehaviour)
                Debug.LogError($"{gameObject.name} {this} did not find CreatureBehaviour with ID matching FriendLevel: {friendLevel.Name}");
        }

        public CreatureBehaviour GetBehaviourOfType(CreatureBehaviour checkedBehaviour)
        {
            foreach (CreatureBehaviour behaviour in behaviours)
            {
                if (behaviour.GetType() == checkedBehaviour.GetType())
                {
                    return behaviour;
                }                   
            }

            return null;
        }

        public void OnFriendshipLevelChanged(FriendshipLevel newLevel)
        {
            ChooseBehaviour(newLevel);
        }
    }
}