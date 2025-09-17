using System;
using System.Linq;
using TMPro;
using UBGKO.Party;
using UBGKO.UI;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

namespace UBGKO.Interactables
{
    public class InteractionGoalDisplay : MonoBehaviour
    {
        [SerializeField] private InteractionGoalEvaluationUI evaluationUIPrefab;
        [Space(5)]
        [SerializeField] private GameObject panel;
        [SerializeField] private InteractionGoalUI goalUI;
        [SerializeField] private Transform evaluationsHolder;
        [SerializeField] private TMP_Text emptyText;
        [SerializeField] private GameObject cycleButtons;

        private int currentIndex = 0;

        public void OpenDisplay()
        {
            UIEvents.OnUIOpened?.Invoke();
            panel.SetActive(true);

            if (PartyManager.Instance.interactionGoals.Count < 1)
            {
                goalUI.gameObject.SetActive(false);
                evaluationsHolder.gameObject.SetActive(false);
                cycleButtons.SetActive(false);
                emptyText.gameObject.SetActive(true);
            }
            else
            {
                goalUI.gameObject.SetActive(true);
                evaluationsHolder.gameObject.SetActive(true);
                if (PartyManager.Instance.interactionGoals.Count > 1)
                    cycleButtons.SetActive(true);
                else
                    cycleButtons.SetActive(false);
                emptyText.gameObject.SetActive(false);

                DisplayAt(0);
            }            
        }

        private void UpdateDisplay(int index)
        {
            currentIndex = index;
            InteractionGoal goal = PartyManager.Instance.interactionGoals[index];
            goalUI.Initialize(goal);

            foreach (Transform child in evaluationsHolder)
                Destroy(child.gameObject);

            var partyMembers = PartyManager.Instance.GetPartyMembers().ToList();
            foreach (var member in PartyManager.Instance.GetPartyMembers())
            {
                var evalUI = Instantiate(evaluationUIPrefab, evaluationsHolder);
                evalUI.Initialize(member, goal);
            }
        }

        public void DisplayAt(int index)
        {
            int nextIndex = (index) % PartyManager.Instance.interactionGoals.Count;
            UpdateDisplay(index);
        }

        public void DisplayNext()
        {
            int nextIndex = (currentIndex + 1) % PartyManager.Instance.interactionGoals.Count;
            UpdateDisplay(nextIndex);
        }

        public void DisplayPrevious()
        {
            int nextIndex = (currentIndex - 1) % PartyManager.Instance.interactionGoals.Count;
            UpdateDisplay(nextIndex);
        }

        public void Close()
        {
            panel.SetActive(false);
            UIEvents.OnUIClosed?.Invoke();
        }
    }
}