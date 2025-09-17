using FIMSpace.FLook;
using FinnTeichler.CMFExtension;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.UltimateInventorySystem.Equipping;
using SensorToolkit;
using UnityEngine;

namespace SpiritGarden.Character
{
    public class CharacterReference : MonoBehaviour
    {
        [HideInInspector] public GameReference refGame;

        public GameObject MainGameObject { get { return mainGameObject; } }
        private GameObject mainGameObject;

        public Transform MainTransform { get { return mainTransform; } }
        private Transform mainTransform;

        public Transform TransformFace { get { return transformFace; } }
        private Transform transformFace;

        // Transforms
        public Transform ModelTransform { get { return modelTransform; } }
        [Space(10)][Header("Transforms")][SerializeField] private Transform modelTransform;

        // Logic
        public CharacterStateController CharacterStateController { get { return characterStateController; } }
        [Space(10)][Header("Logic")][SerializeField] private CharacterStateController characterStateController;



        // Physics and Movement related components
        public CapsuleCollider MovementCollider { get { return movementCollider; } }
        [Space(10)][Header("Movement")][SerializeField] private CapsuleCollider movementCollider;

        public Rigidbody MovementRigidbody { get { return movementRigidbody; } }
        [SerializeField] private Rigidbody movementRigidbody;

        public CharacterControls CharacterControls { get { return characterControls; } }
        [SerializeField] private CharacterControls characterControls;

        public CMF.Mover CMFMover { get { return cmfMover; } }
        [SerializeField] private CMF.Mover cmfMover;

        public CMF.AdvancedWalkerController CMFAdvWalkerController { get { return cmfAdvWalkerController; } }
        [SerializeField] private CMF.AdvancedWalkerController cmfAdvWalkerController;


        // Camera related components
        public Camera ThirdPersonCamera { get { return thirdPersonCamera; } }
        [Space(10)][Header("Camera")][SerializeField] private Camera thirdPersonCamera;

        public CMF.CameraController CMFCameraController { get { return cmfCameraController; } }
        [SerializeField] private CMF.CameraController cmfCameraController;

        public CameraControls CameraControls { get { return cameraControls; } }
        [SerializeField] private CameraControls cameraControls;

        public Transform CameraTarget { get { return cameraTarget; } }
        [SerializeField] private Transform cameraTarget;

        // Detection related components
        public TriggerSensor CreatureSensorCamera { get { return creatureSensorCamera; } }
        [Space(10)][Header("Detection")][SerializeField] private TriggerSensor creatureSensorCamera;

        public TriggerSensor CreatureSensorModel { get { return creatureSensorModel; } }
        [SerializeField] private TriggerSensor creatureSensorModel;

        public TriggerSensor ItemSensor { get { return itemSensor; } }
        [SerializeField] private TriggerSensor itemSensor;


        // Animation
        public CharacterAnimationControl AnimationControl { get { return animationControl; } }
        [Space(10)][Header("Animation")][SerializeField] private CharacterAnimationControl animationControl;

        public Animator AnimatorComponent { get { return animatorComponent; } }
        [SerializeField] private Animator animatorComponent;

        public FLookAnimator LookAnimatorHand { get { return lookAnimatorHand; } }
        [SerializeField] private FLookAnimator lookAnimatorHand;

        public Transform LookAnimatorHandTarget { get { return lookAnimatorHandTarget; } }
        [SerializeField] private Transform lookAnimatorHandTarget;

        public FLookAnimator LookAnimatorHead { get { return lookAnimatorHead; } }
        [SerializeField] private FLookAnimator lookAnimatorHead;



        public Transform EquippedItemTransform { get { return equippedItemPosition; } }
        [SerializeField] private Transform equippedItemPosition;

        public float HandTargetSpeed { get { return handTargetSpeed; } }
        [SerializeField] private float handTargetSpeed = 1f;
        public AnimationCurve HandTargetSpeedInputCurve { get { return handTargetSpeedInputCurve; } }
        [SerializeField] private AnimationCurve handTargetSpeedInputCurve;

        public Vector2 HandTargetCapX { get { return handTargetCapX; } }
        [SerializeField] private Vector2 handTargetCapX = new Vector2(8, -10);
        public Vector2 HandTargetCapY { get { return handTargetCapY; } }
        [SerializeField] private Vector2 handTargetCapY = new Vector2(10, -8);


        // Audio
        public CharacterAudioControl AudioControl { get { return audioControl; } }
        [Space(10)][Header("Audio")][SerializeField] private CharacterAudioControl audioControl;


        // Inventory
        public Inventory Inventory { get { return inventory; } }
        [Space(10)][Header("Inventory")][SerializeField] private Inventory inventory;

        public Equipper Equipper { get { return equipper; } }
        [SerializeField] private Equipper equipper;


        void Awake()
        {
            mainGameObject = gameObject;
            mainTransform = transform;

            IfNullLogError(characterStateController);

            IfNullLogError(movementCollider);
            IfNullLogError(movementRigidbody);
            IfNullLogError(characterControls);
            IfNullLogError(cmfMover);
            IfNullLogError(cmfAdvWalkerController);

            IfNullLogError(thirdPersonCamera);
            IfNullLogError(cmfCameraController);
            IfNullLogError(cameraControls);
            IfNullLogError(cameraTarget);

            IfNullLogError(creatureSensorCamera);
            IfNullLogError(animationControl);

            IfNullLogError(lookAnimatorHand);
            IfNullLogError(lookAnimatorHandTarget);
            IfNullLogError(equippedItemPosition);

            IfNullLogError(audioControl);

            IfNullLogError(inventory);
            IfNullLogError(equipper);

            characterStateController.characterReference = this;
            animationControl.characterReference = this;
            audioControl.characterReference = this;
            characterControls.refCharacter = this;
        }

        private void IfNullLogError(Component component)
        {
            if (component == null)
            {
                Debug.LogError($"{this} is missing a {component} component.");
            }
        }
    }
}