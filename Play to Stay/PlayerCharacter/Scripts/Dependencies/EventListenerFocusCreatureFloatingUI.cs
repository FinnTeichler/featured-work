using UnityEngine;
using FinnTeichler.UI;
using FinnTeichler.Event.Bus;
using SpiritGarden.Character;
using SpiritGarden.Creature.Interaction;
using SpiritGarden.Creature;
using SpiritGarden.Creature.Interaction.Data;

namespace SpiritGarden
{
    public class EventListenerFocusCreatureFloatingUI : MonoBehaviour
    {
        [SerializeField] private bool debugMode;
        [SerializeField] private Camera floatingUICamera;
        [SerializeField] private float yOffset = 2f;

        private FloatingUI floatingUI;
        private EventBinding<EngageCreatureCharacterState.OnCreatureFocusEnter> onFocusEnterBinding;
        private EventBinding<EngageCreatureCharacterState.OnCreatureFocusExit> onFocusExitBinding;

        private CreatureReference creatureReference;

        private void Start()
        {
            floatingUI = GetComponent<FloatingUI>();
            floatingUI.SetaData(new FloatingUI.Data(floatingUICamera, null, Vector3.zero, false));
        }

        void OnEnable()
        {
            onFocusEnterBinding = new EventBinding<EngageCreatureCharacterState.OnCreatureFocusEnter>(HandleOnFocusEnter);
            EventBus<EngageCreatureCharacterState.OnCreatureFocusEnter>.Register(onFocusEnterBinding);

            onFocusExitBinding = new EventBinding<EngageCreatureCharacterState.OnCreatureFocusExit>(HandleOnFocusExit);
            EventBus<EngageCreatureCharacterState.OnCreatureFocusExit>.Register(onFocusExitBinding);
        }

        void OnDisable()
        {
            EventBus<EngageCreatureCharacterState.OnCreatureFocusEnter>.Deregister(onFocusEnterBinding);
            EventBus<EngageCreatureCharacterState.OnCreatureFocusExit>.Deregister(onFocusExitBinding);

            if (creatureReference)
                creatureReference.Friendship.OnLevelCurrentChanged -= OnFriendLevelChanged;
        }

        private void HandleOnFocusEnter(EngageCreatureCharacterState.OnCreatureFocusEnter onCreatureFocusEnter)
        {
            creatureReference = onCreatureFocusEnter.creatureObject.GetComponent<CreatureReference>();

            floatingUI.SetTarget(creatureReference.MainTransform);
            floatingUI.SetOffset(new Vector3(0, yOffset, 0));
            floatingUI.SetActivation(true);
            floatingUI.image.color = creatureReference.Friendship.EvaluateLevelCurrent().UIColor;
            floatingUI.text.text = creatureReference.MainGameObject.name;

            creatureReference.Friendship.OnLevelCurrentChanged += OnFriendLevelChanged;

            if (debugMode)
                Debug.Log($"OnFocusEnter event received! Focused Creature: {onCreatureFocusEnter.creatureObject.name}");
        }

        private void HandleOnFocusExit(EngageCreatureCharacterState.OnCreatureFocusExit onCreatureFocusExit)
        {
            floatingUI.SetActivation(false);
            floatingUI.SetTarget(null);
            floatingUI.SetOffset(new Vector3(0, 0, 0));
            floatingUI.image.color = Color.white;

            creatureReference.Friendship.OnLevelCurrentChanged -= OnFriendLevelChanged;

            if (debugMode)
                Debug.Log($"OnFocusEnter event received! Focused Creature: null");
        }

        private void OnFriendLevelChanged(FriendshipLevel newLevel)
        {
            floatingUI.image.color = newLevel.UIColor;
        }
    }
}