using UnityEngine;
using Animancer;
using CMF;

namespace SpiritGarden.Character
{
    [RequireComponent(typeof(AnimancerComponent))]
    public class CharacterAnimationControl : MonoBehaviour
    {
        private AnimancerComponent animancer;

        [SerializeField] private LinearMixerTransition mixerMoveCycle;
        [Space(10)]
        [SerializeField] private AnimationClip jumpStartFromIdle;
        [SerializeField] private AnimationClip jumpStartFromMoving;
        [SerializeField] private AnimationClip jumpIdle;
        [SerializeField] private AnimationClip jumpEnd;
        [Space(5)]
        [SerializeField] private AnimationClip engageCreatureEnter;
        [SerializeField] private AnimationClip engageCreatureIdle;
        [SerializeField] private AnimationClip engageCreatureExit;

        [HideInInspector] public CharacterReference characterReference;

        private Vector3 velocityHorizontalPrevious = Vector3.zero;
        private float smoothingFactor = 40f;

        private bool isJumping;

        private void Awake()
        {
            animancer = GetComponent<AnimancerComponent>();
        }

        void OnEnable()
        {
            characterReference.CMFAdvWalkerController.OnJump += OnJump;
            characterReference.CMFAdvWalkerController.OnLand += OnLand;

            characterReference.CharacterStateController.OnStateMovement += OnStateMovement;
            characterReference.CharacterStateController.OnStateEngageCreature += OnStateEngageCreature;
        }

        void OnDisable()
        {
            characterReference.CMFAdvWalkerController.OnJump -= OnJump;
            characterReference.CMFAdvWalkerController.OnLand -= OnLand;

            characterReference.CharacterStateController.OnStateMovement -= OnStateMovement;
            characterReference.CharacterStateController.OnStateEngageCreature -= OnStateEngageCreature;
        }


        void Start()
        {
            animancer.Play(mixerMoveCycle);
        }

        void Update()
        {
            if (isJumping == false)
            {
                mixerMoveCycle.State.Parameter = VelocityHorizontalRelativeToMax();
            }
        }

        private void OnStateMovement()
        {
            PlayMixerMoveCycle();
        }

        private void OnStateEngageCreature()
        {
            var state = animancer.Play(engageCreatureIdle, 0.2f, FadeMode.FixedSpeed);
            state.Time = 0;
        }

        private void OnJump(Vector3 _v)
        {
            if (VelocityHorizontalRelativeToMax() >= 0.2f)
            {
                var state = animancer.Play(jumpStartFromMoving, 0.1f, FadeMode.FixedSpeed);
                state.Time = 0;
                state.Events.OnEnd = PlayJumpIdle;
            }
            else
            {
                var state = animancer.Play(jumpStartFromIdle, 0.1f, FadeMode.FixedSpeed);
                state.Time = 0;
                state.Events.OnEnd = PlayJumpIdle;
            }


            isJumping = true;
        }

        void OnLand(Vector3 _v)
        {
            isJumping = false;

            if (jumpEnd)
            {
                var state = animancer.Play(jumpEnd, 0.2f, FadeMode.FixedSpeed);
                state.NormalizedTime = 0.3f;
                state.Speed = 1.5f;
                state.Events.OnEnd = PlayMixerMoveCycle;
            }
            else
            {
                PlayMixerMoveCycle();
            }
        }

        private void PlayMixerMoveCycle()
        {
            animancer.Play(mixerMoveCycle, 0.2f, FadeMode.FixedSpeed);
            mixerMoveCycle.State.Parameter = VelocityHorizontalRelativeToMax();
        }

        private void PlayJumpIdle()
        {
            var state = animancer.Play(jumpIdle, 0.2f, FadeMode.FixedSpeed);
            state.Time = 0;
        }


        private Vector3 VelocityHorizontal()
        {
            Vector3 _velocity = characterReference.CMFAdvWalkerController.GetVelocity();
            Vector3 _horizontalVelocity = VectorMath.RemoveDotVector(_velocity, transform.up);
            Vector3 _verticalVelocity = _velocity - _horizontalVelocity;
            _horizontalVelocity = Vector3.Lerp(velocityHorizontalPrevious, _horizontalVelocity, smoothingFactor * Time.deltaTime);
            velocityHorizontalPrevious = _horizontalVelocity;

            return _horizontalVelocity;
        }

        private float VelocityHorizontalRelativeToMax()
        {
            return VelocityHorizontal().magnitude / characterReference.CMFAdvWalkerController.movementSpeed;
        }
    }
}