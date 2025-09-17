using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UBGKO.Party;

namespace UBGKO.Interactables
{
    public class InteractionGoalEvaluationUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text evaluationResultText;
        [SerializeField] private TMP_Text characterNameText;
        [SerializeField] private Image characterIcon;

        public void Initialize(PartyMember partyMember, InteractionGoal goal)
        {
            characterNameText.text = partyMember.name;

            bool aligns = partyMember.IsAlignedWith(goal);
            bool contradicts = partyMember.IsContradictedWith(goal);

            if (aligns && contradicts)
                evaluationResultText.text = "Conflicted";
            else if (aligns && !contradicts)
                evaluationResultText.text = "Aligned";
            else if (!aligns && contradicts)
                evaluationResultText.text = "Contradicted";
            else if (!aligns && !contradicts)
                evaluationResultText.text = "Neutral";
        }
    }
}