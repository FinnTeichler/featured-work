using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UBGKO.Interactables
{
    public class InteractionGoalUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text goalNameText;
        [SerializeField] private Image interactionTypeIcon;
        [SerializeField] private Image interactionTargetIcon;

        public void Initialize(InteractionGoal goal)
        {
            goalNameText.text = goal.GetDisplayName();
            interactionTypeIcon.sprite = goal.interactionType.icon;
            //interactionTargetIcon.sprite = goal.interactionTarget.Icon;
        }
    }
}