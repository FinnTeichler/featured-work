using UnityEngine;

namespace SpiritGarden.Creature.Interaction.Data
{
    [CreateAssetMenu(fileName = "X_LikeSet", menuName = "Data/Friendship/LikeSet")]
    public class LikeSet : ScriptableObject
    {
        public LikeInfo[] LikeInfos { get => likeInfos; }
        [SerializeField] private LikeInfo[] likeInfos;
    }
}