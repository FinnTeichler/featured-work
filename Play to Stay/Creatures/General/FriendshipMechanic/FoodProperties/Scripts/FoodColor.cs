using UnityEngine;

namespace SpiritGarden.Creature.Interaction.Data
{
    [CreateAssetMenu(fileName = "_FoodColor", menuName = "Data/Items/FoodProperties/Color")]
    public class FoodColor : FoodProperty
    {
        public Color Color { get => color; }
        [SerializeField] private Color color;
    }
}