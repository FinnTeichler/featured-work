using UnityEngine;

namespace SpiritGarden.Creature.Interaction.Data
{
    [CreateAssetMenu(fileName = "X_LikeLevel", menuName = "Data/Friendship/LikeLevel")]
    public class LikeLevel : ScriptableObject
    {
        public int Value { get => value; }
        [SerializeField] private int value = 1;
    }
}