using MoreMountains.Feedbacks;
using SpiritGarden.Character;
using SpiritGarden.Creature.Interaction.Data;
using System.Collections.Generic;
using UnityEngine;

namespace SpiritGarden.Creature.Behaviour
{
    public class CBS_InteractCharacter : CBStateBase
    {
        private InteractionData interactionData;
        private CharacterReference characterReference;
        
        readonly float duration;

        private float durationTimer = 0f;
        
        private bool needsAnimation = false;

        public CBS_InteractCharacter(CreatureReference creatureReference, float duration) : base(creatureReference)
        {
            this.duration = duration;
        }

        public override void OnEnter()
        {
            hasInteracted = false;
            needsAnimation = true;

            interactionData = creatureReference.Friendship.CurrentInteraction.data;
            characterReference = interactionData.Character.GetComponent<CharacterReference>();

            Vector3 pos = characterReference.ModelTransform.position + characterReference.ModelTransform.forward * 2f;
            creatureReference.Agent.SetDestination(pos);
        }

        public override void Update()
        {
            if (creatureReference.InfoHub.AgentReachedDestination())
            {
                if (needsAnimation)
                {
                    int friendshipChange = creatureReference.Friendship.GetInteractionValue(interactionData);

                    creatureReference.AnimationControl.SetFeedbackExpressions(creatureReference.Friendship.GetInteractionLikeLevels(interactionData));

                    if (friendshipChange > 0)
                        creatureReference.AnimationControl.PlayEmote(CreatureAnimationControl.Emote.YES);
                    else
                        creatureReference.AnimationControl.PlayEmote(CreatureAnimationControl.Emote.NO);

                    needsAnimation = false;
                }

                durationTimer += Time.deltaTime;
                float relativeTime = durationTimer / duration;

                Vector3 direction = characterReference.ModelTransform.position - creatureReference.MainTransform.position;
                direction.y = 0;
                Quaternion rotation = Quaternion.LookRotation(direction);
                creatureReference.MainTransform.rotation = Quaternion.Slerp(creatureReference.MainTransform.rotation, rotation, relativeTime);

                if (durationTimer >= duration)
                {
                    durationTimer = 0f;

                    if (interactionData == null)
                        Debug.LogError($"{creatureReference.gameObject.name} is trying to interact with player, but InteractionData is null.");
                    else
                    {
                        if (interactionData.ItemInfo.Item != null)
                        {
                            interactionData.Character.GetComponent<CharacterReference>().Inventory.RemoveItem(interactionData.ItemInfo);
                        }

                        creatureReference.Friendship.InteractionComplete();

                        Vector3 spawnPos = creatureReference.MainTransform.position + new Vector3(0f, 2f, 0f);
                        int friendshipChange = creatureReference.Friendship.GetInteractionValue(interactionData);

                        MMF_FloatingText floatingTextFeedback = creatureReference.MMFInteractionText.GetFeedbackOfType<MMF_FloatingText>();

                        Gradient gradientGood = new Gradient();
                        Gradient gradientBad = new Gradient();
                            
                        GradientColorKey[] colorKeyGood = new GradientColorKey[2];
                        colorKeyGood[0].color = Color.green;
                        colorKeyGood[0].time = 0.0f;
                        colorKeyGood[1].color = Color.yellow;
                        colorKeyGood[1].time = 1.0f;

                        GradientColorKey[] colorKeyBad = new GradientColorKey[2];
                        colorKeyBad[0].color = Color.red;
                        colorKeyBad[0].time = 0.0f;
                        colorKeyBad[1].color = Color.yellow;
                        colorKeyBad[1].time = 1.0f;
                            
                        GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
                        alphaKey[0].alpha = 1.0f;
                        alphaKey[0].time = 0.0f;
                        alphaKey[1].alpha = 0.3f;
                        alphaKey[1].time = 1.0f;

                        gradientGood.SetKeys(colorKeyGood, alphaKey);
                        gradientBad.SetKeys(colorKeyBad, alphaKey);

                        floatingTextFeedback.ForceColor = true;

                        if (friendshipChange > 0)
                            floatingTextFeedback.AnimateColorGradient = gradientGood;
                        else
                            floatingTextFeedback.AnimateColorGradient = gradientBad;
                        
                        creatureReference.MMFInteractionText.PlayFeedbacks(spawnPos, friendshipChange);
                        hasInteracted = true;
                    }
                }
            }
        }

        public override void OnExit()
        {
            durationTimer = 0f;
            hasInteracted = false;
            creatureReference.Agent.SetDestination(creatureReference.MainTransform.position);
        }

        private bool hasInteracted = false;
        public bool HasInteracted()
        {
            return hasInteracted;
        }
    }
}