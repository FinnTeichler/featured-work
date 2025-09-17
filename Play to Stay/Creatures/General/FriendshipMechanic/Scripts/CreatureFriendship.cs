using System;
using UnityEngine;
using SpiritGarden.Creature.Interaction.Data;
using FinnTeichler.Event.Bus;
using SpiritGarden.Character;
using System.Collections.Generic;
using MoreMountains.Feedbacks;

namespace SpiritGarden.Creature.Interaction
{
    public class CreatureFriendship : MonoBehaviour
    {
        [HideInInspector] public CreatureReference creatureReference;

        [SerializeField] private bool debugMode;

        public LikeProfile ProfileLike { get => profileLike; }
        [SerializeField] private LikeProfile profileLike;

        public event Action<int> OnValueCurrentChanged;
        public event Action<FriendshipLevel> OnLevelCurrentChanged;

        //public Action<int, InteractionData> OnEvaluateInteraction;
        //public Action<int, InteractionData> OnInteraction;        

        public struct EvaluatedInteractionData
        {
            public int valueChange;
            public InteractionData data;
            public bool completed;

            public EvaluatedInteractionData(int valueChange, InteractionData data, bool completed)
            {
                this.valueChange = valueChange;
                this.data = data;
                this.completed = completed;
            }

            public EvaluatedInteractionData(bool isNull)
            {
                this.valueChange = 0;
                this.data = null;
                this.completed = true;
            }
        }
        [HideInInspector] public EvaluatedInteractionData CurrentInteraction { get { return currentInteraction; } }
        private EvaluatedInteractionData currentInteraction;
        [HideInInspector] public EvaluatedInteractionData PreviousInteraction { get { return previousInteraction; } }
        private EvaluatedInteractionData previousInteraction;

        public struct OnInteractionEnter : IEvent { public GameObject creatureObject; }
        public struct OnInteractionComplete : IEvent { public GameObject creatureObject; }
        public struct OnInteractionAbort : IEvent { public GameObject creatureObject; }

        public FriendshipLevel LevelCurrent { get => EvaluateLevelCurrent(); }
        public FriendshipLevel LevelNext { get => EvaluateLevelNext(); }

        public float TimeSinceLastInteraction { get => timeSinceLastInteraction; }
        private float timeSinceLastInteraction;

        public int ValueCurrent
        {
            get { return valueCurrent; }
            private set
            {
                int valuePrevious = valueCurrent;
                FriendshipLevel levelPrevious = LevelCurrent;

                valueCurrent = value;
                if (valueCurrent < 0)
                    valueCurrent = 0;

                if (debugMode)
                    Debug.Log(gameObject.name + " CreatureFriendship component ValueChangend: " + valueCurrent);

                if (valuePrevious != valueCurrent)
                    OnValueCurrentChanged?.Invoke(valueCurrent);

                if (LevelCurrent != levelPrevious)
                {
                    OnLevelCurrentChanged?.Invoke(LevelCurrent);

                    MMF_FloatingText floatingText = creatureReference.MMFLevelUp.GetFeedbackOfType<MMF_FloatingText>();

                    floatingText.Value = $"{LevelCurrent.Name}!";
                    creatureReference.MMFLevelUp.PlayFeedbacks();
                }
            }
        }
        private int valueCurrent;


        public int ValueInCurrentLevel()
        {
            return valueCurrent - EvaluateLevelCurrent().Cost;
        }


        public FriendshipLevel EvaluateLevelCurrent()
        {
            int highestReachedLevelCost = 0;
            FriendshipLevel highestReachedLevel = profileLike.PossibleLevels[0];

            foreach (FriendshipLevel level in profileLike.PossibleLevels)
            {
                if (ValueCurrent >= level.Cost)
                {
                    if (level.Cost > highestReachedLevelCost)
                    {
                        highestReachedLevelCost = level.Cost;
                        highestReachedLevel = level;
                    }
                }
            }

            return highestReachedLevel;
        }

        public FriendshipLevel EvaluateLevelNext()
        {
            int currentLevelIndex = profileLike.PossibleLevels.IndexOf(EvaluateLevelCurrent());
            int nextLevelIndex = currentLevelIndex + 1;

            if (nextLevelIndex >= 0 && nextLevelIndex < profileLike.PossibleLevels.Count)
                return profileLike.PossibleLevels[nextLevelIndex];
            else
                return null;
        }

        public bool CurrentLevelIsMaxLevel()
        {
            int nextLevelIndex = profileLike.PossibleLevels.IndexOf(EvaluateLevelNext());

            if (nextLevelIndex >= 0 && nextLevelIndex < profileLike.PossibleLevels.Count)
                return false;
            else
                return true;
        }

        public int GetInteractionValue(InteractionData data)
        {
            return profileLike.EvaluateInteraction(data);
        }

        public List<LikeLevel> GetInteractionLikeLevels(InteractionData data)
        {
            return profileLike.GetInteractionLikeLevels(data);
        }

        public void InteractionEnter(InteractionData data)
        {
            int valueChange = profileLike.EvaluateInteraction(data);
            currentInteraction = new EvaluatedInteractionData(valueChange, data, false);
            EventBus<OnInteractionEnter>.Raise(new OnInteractionEnter { creatureObject = creatureReference.MainGameObject });
        }

        public void InteractionComplete()
        {
            ValueCurrent += currentInteraction.valueChange;

            previousInteraction = currentInteraction;
            currentInteraction = new EvaluatedInteractionData(false);
            timeSinceLastInteraction = 0f;

            EventBus<OnInteractionComplete>.Raise(new OnInteractionComplete { creatureObject = creatureReference.MainGameObject });
        }

        public void InteractionAbort()
        {
            Debug.Log("InteractionAbort");
            currentInteraction = new EvaluatedInteractionData(false);
            EventBus<OnInteractionAbort>.Raise(new OnInteractionAbort { creatureObject = creatureReference.MainGameObject });
        }

        private void OnEnable()
        {
            SubsribeEventsFocus();
        }

        private void OnDisable()
        {
            UnsubsribeEventsFocus();
        }

        private void Update()
        {
            timeSinceLastInteraction += Time.deltaTime;

            if (creatureReference.Friendship.CurrentInteraction.data != null &&
                creatureReference.Friendship.CurrentInteraction.data.IsDropInteraction)
            {
                if (creatureReference.Friendship.CurrentInteraction.data.DroppedItem == null ||
                creatureReference.Friendship.CurrentInteraction.data.DroppedItem.activeSelf == false)
                {
                    InteractionAbort();
                }
            }
        }

        #region isFocused
        private EventBinding<EngageCreatureCharacterState.OnCreatureFocusEnter> onFocusEnterBinding;
        private EventBinding<EngageCreatureCharacterState.OnCreatureFocusExit> onFocusExitBinding;

        private void SubsribeEventsFocus()
        {
            onFocusEnterBinding = new EventBinding<EngageCreatureCharacterState.OnCreatureFocusEnter>(HandleOnFocusEnter);
            EventBus<EngageCreatureCharacterState.OnCreatureFocusEnter>.Register(onFocusEnterBinding);

            onFocusExitBinding = new EventBinding<EngageCreatureCharacterState.OnCreatureFocusExit>(HandleOnFocusExit);
            EventBus<EngageCreatureCharacterState.OnCreatureFocusExit>.Register(onFocusExitBinding);
        }

        private void UnsubsribeEventsFocus()
        {
            EventBus<EngageCreatureCharacterState.OnCreatureFocusEnter>.Deregister(onFocusEnterBinding);
            EventBus<EngageCreatureCharacterState.OnCreatureFocusExit>.Deregister(onFocusExitBinding);
        }

        private void HandleOnFocusEnter(EngageCreatureCharacterState.OnCreatureFocusEnter onCreatureFocusEnter)
        {
            if (creatureReference.MainGameObject == onCreatureFocusEnter.creatureObject)
            {
                
            }
        }

        private void HandleOnFocusExit(EngageCreatureCharacterState.OnCreatureFocusExit onCreatureFocusExit)
        {
            if (creatureReference.MainGameObject == onCreatureFocusExit.creatureObject)
            {
                if (currentInteraction.data != null)
                {
                    if (currentInteraction.data.IsDropInteraction == false)
                    {
                        InteractionAbort();
                    }
                }
            }
        }
        #endregion isFocused
    }
}