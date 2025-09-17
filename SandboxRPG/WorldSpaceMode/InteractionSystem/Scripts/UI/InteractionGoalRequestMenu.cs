using System;
using System.Linq;
using UBGKO.Party;
using UBGKO.UI;
using UnityEngine;

namespace UBGKO.Interactables
{
    public class InteractionGoalRequestMenu : MonoBehaviour
    {
        [SerializeField] private InteractionGoalEvaluationUI evaluationUIPrefab;
        [Space(5)]
        [SerializeField] private GameObject panel;
        [SerializeField] private InteractionGoalUI goalUI;
        [SerializeField] private Transform evaluationsHolder;

        private InteractionGoal currentGoal;

        private void OnEnable()
        {
            InteractionEvents.OnGoalRequested += HandleGoalRequested;
        }

        private void OnDisable()
        {
            InteractionEvents.OnGoalRequested -= HandleGoalRequested;
        }

        private void HandleGoalRequested(InteractionGoal goal)
        {
            UIEvents.OnUIOpened?.Invoke();

            currentGoal = goal;
            panel.SetActive(true);
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

        public void Accept()
        {
            InteractionEvents.RaiseGoalAccepted(currentGoal);
            Close();
        }

        public void Decline()
        {
            InteractionEvents.RaiseGoalDeclined(currentGoal);
            Close();
        }

        private void Close()
        {
            panel.SetActive(false);
            currentGoal = null;

            UIEvents.OnUIClosed?.Invoke();
        }
    }
}