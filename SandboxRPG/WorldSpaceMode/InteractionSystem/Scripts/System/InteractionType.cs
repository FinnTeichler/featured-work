using UnityEngine;

namespace UBGKO.Interactables
{
    [CreateAssetMenu(fileName = "InteractionType", menuName = "Game/Interaction/InteractionType")]
    public class InteractionType : ScriptableObject
    {
        public string displayName;
        public Sprite icon;
        public AudioClip interactionSound;
        public string description;

        // You can even add conditions, animation clips, etc.
    }
}