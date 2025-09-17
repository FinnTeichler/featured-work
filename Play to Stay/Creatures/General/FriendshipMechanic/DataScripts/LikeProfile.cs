using System.Collections.Generic;
using UnityEngine;

namespace SpiritGarden.Creature.Interaction.Data
{
    [CreateAssetMenu(fileName = "X_LikeProfile", menuName = "Data/Friendship/LikeProfile")]
    public class LikeProfile : ScriptableObject
    {
        public float TrustfulnessFactor { get => trustfulnessFactor; }
        [SerializeField] private float trustfulnessFactor = 1f;

        [SerializeField] private LikeSet[] likeSetHierarchy;

        public List<FriendshipLevel> PossibleLevels { get => possibleLevels; }
        [SerializeField] private List<FriendshipLevel> possibleLevels = new();

        public int EvaluateInteraction(InteractionData data)
        {
            return GetInteractionValue(data);
        }

        public int EvaluateInteractionWithTrustfulnessFactor(InteractionData data)
        {
            return Mathf.RoundToInt(GetInteractionValue(data) * trustfulnessFactor);
        }

        private int GetInteractionValue(InteractionData data)
        {
            int interactionValue = 0;
            List<InteractionProperty> matchingProperties = new();

            if (data.InteractionType == InteractionData.Type.PET)
            {
                interactionValue = 10;
            }
            else
            {
                for (int indexPropertiesData = 0; indexPropertiesData < data.Properties.Count; indexPropertiesData++)
                {
                    for (int indexSet = 0; indexSet <= likeSetHierarchy.Length - 1; indexSet++)
                    {
                        for (int indexInfos = 0; indexInfos <= likeSetHierarchy[indexSet].LikeInfos.Length - 1; indexInfos++)
                        {
                            for (int indexPropertiesLike = 0; indexPropertiesLike < likeSetHierarchy[indexSet].LikeInfos[indexInfos].InteractionProperties.Length; indexPropertiesLike++)
                            {
                                InteractionProperty propertyLike = likeSetHierarchy[indexSet].LikeInfos[indexInfos].InteractionProperties[indexPropertiesLike];
                                if (data.Properties[indexPropertiesData] == propertyLike)
                                {
                                    matchingProperties.Add(data.Properties[indexPropertiesData]);
                                    interactionValue += likeSetHierarchy[indexSet].LikeInfos[indexInfos].LikeLevel.Value;
                                    //Debug.Log($"Interaction Value Calc: {likeSetHierarchy[indexSet].LikeInfos[indexInfos].LikeLevel.Value} from {data.Properties[indexPropertiesData].name}");
                                }
                            }
                        }
                    }
                }
            }
            return interactionValue;
        }

        public List<LikeLevel> GetInteractionLikeLevels(InteractionData data)
        {
            List<LikeLevel> likeLevels = new();
            List<InteractionProperty> matchingProperties = new();

            for (int indexPropertiesData = 0; indexPropertiesData < data.Properties.Count; indexPropertiesData++)
            {
                for (int indexSet = 0; indexSet <= likeSetHierarchy.Length - 1; indexSet++)
                {
                    for (int indexInfos = 0; indexInfos <= likeSetHierarchy[indexSet].LikeInfos.Length - 1; indexInfos++)
                    {
                        for (int indexPropertiesLike = 0; indexPropertiesLike < likeSetHierarchy[indexSet].LikeInfos[indexInfos].InteractionProperties.Length; indexPropertiesLike++)
                        {
                            InteractionProperty propertyLike = likeSetHierarchy[indexSet].LikeInfos[indexInfos].InteractionProperties[indexPropertiesLike];
                            if (data.Properties[indexPropertiesData] == propertyLike)
                            {
                                matchingProperties.Add(data.Properties[indexPropertiesData]);
                                likeLevels.Add(likeSetHierarchy[indexSet].LikeInfos[indexInfos].LikeLevel);
                            }
                        }
                    }
                }
            }
            return likeLevels;
        }
    }
}