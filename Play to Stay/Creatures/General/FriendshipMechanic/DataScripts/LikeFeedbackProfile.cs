using System;
using UnityEngine;

namespace SpiritGarden.Creature.Interaction.Data
{
    [CreateAssetMenu(fileName = "X_LikeFeedbackProfile", menuName = "Data/Friendship/LikeFeedbackProfile")]
    public class LikeFeedbackProfile : ScriptableObject
    {
        [Serializable]
        public struct LikeFeedbackData
        {
            public LikeLevel likeLevel;
            public AnimationClip eyeAnimation;

            public LikeFeedbackData
            (
                LikeLevel likeLevel,
                AnimationClip eyeAnimation
            )
            {
                this.likeLevel = likeLevel;
                this.eyeAnimation = eyeAnimation;
            }
        }

        public LikeFeedbackData[] Data { get => data; }
        [SerializeField] private LikeFeedbackData[] data;

        public LikeFeedbackData GetFeedbackForLikeLevel(LikeLevel _likeLevel)
        {
            for (int index = 0; index <= data.Length - 1; index++)
            {
                if (_likeLevel == data[index].likeLevel)
                {
                    return data[index];
                }
            }

            Debug.LogError(name + "could not find LikeFeedbackData for LikeLevel + " + _likeLevel + "returned first data in array, if null, then there is no data at all.");
            return data[0];
        }
    }
}