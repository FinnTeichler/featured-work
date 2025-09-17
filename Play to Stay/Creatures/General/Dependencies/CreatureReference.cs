using Animancer;
using MoreMountains.Feedbacks;
using SpiritGarden.Creature.Behaviour;
using SpiritGarden.Creature.Interaction;
using SpiritGarden.Creature.Animation;
using UnityEngine;
using UnityEngine.AI;

namespace SpiritGarden.Creature
{
    public class CreatureReference : MonoBehaviour
    {
        public GameReference RefGame { get { return refGame; }}
        [SerializeField] private GameReference refGame;

        public GameObject MainGameObject { get { return mainGameObject; } }
        private GameObject mainGameObject;
        public Transform MainTransform { get { return mainTransform; } }
        private Transform mainTransform;

        public Transform TransformFace { get { return transformFace; } }
        [SerializeField] private Transform transformFace;

        public CapsuleCollider MainTrigger { get { return mainTrigger; } }
        [SerializeField] private CapsuleCollider mainTrigger;

        public NavMeshAgent Agent { get { return agent; } }
        [SerializeField] private NavMeshAgent agent;

        [SerializeField] private AnimancerComponent animancer;

        public MMF_Player MMFInteractionText { get { return mmfInteractionText; }}
        [SerializeField] private MMF_Player mmfInteractionText;

        public MMF_Player MMFLevelUp { get { return mmfLevelUp; }}
        [SerializeField] private MMF_Player mmfLevelUp;

        public CreatureBehaviourManager BehaviourManager { get { return behaviourManager; } }
        [SerializeField] private CreatureBehaviourManager behaviourManager;

        public CreatureFriendship Friendship { get { return friendship; } }
        [SerializeField] private CreatureFriendship friendship;

        public CreatureInfoHub InfoHub { get { return boolHub; } }
        [SerializeField] private CreatureInfoHub boolHub;

        public CreatureAnimationControl AnimationControl { get { return animationControl; } }
        [SerializeField] private CreatureAnimationControl animationControl;

        public SkinnedMeshRenderer FaceSkinnedMeshRenderer {  get { return faceSkinnedMeshRenderer; } }
        [SerializeField] private SkinnedMeshRenderer faceSkinnedMeshRenderer;

        private void Awake()
        {
            mainGameObject = gameObject;
            mainTransform = transform;

            IfNullLogError(mainTrigger);
            IfNullLogError(agent);
            IfNullLogError(animancer);
            IfNullLogError(behaviourManager);
            IfNullLogError(friendship);
            IfNullLogError(boolHub);
            IfNullLogError(animationControl);

            behaviourManager.creatureReference = this;
            friendship.creatureReference = this;
            boolHub.creatureReference = this;
            animationControl.creatureReference = this;
        }

        private void IfNullLogError(UnityEngine.Component component)
        {
            if (component == null)
            {
                Debug.LogError($"{this} is missing a {component} component.");
            }
        }
    }
}