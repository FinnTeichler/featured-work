using UnityEngine;

namespace SpiritGarden.Creature.Interaction.Data
{
    [CreateAssetMenu(menuName = "Data/Interaction/InteractionDataObject", fileName = "_IDO")]
    public class InteractionDataObject : ScriptableObject
    {
        public InteractionProperty[] InteractionProperties => interactionProperties;
        [SerializeField] private InteractionProperty[] interactionProperties;
    }
}