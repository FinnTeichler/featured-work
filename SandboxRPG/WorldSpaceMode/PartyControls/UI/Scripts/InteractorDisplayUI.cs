using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UBGKO.Party.Controls.UI
{
    public class InteractorDisplayUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;

        private void OnEnable()
        {
            PartyEvents.OnInteractorChanged += UpdateDisplay;
        }

        private void OnDisable()
        {
            PartyEvents.OnInteractorChanged -= UpdateDisplay;
        }

        private void UpdateDisplay(PartyMember newInteractor)
        {
            iconImage.sprite = newInteractor.Data.Portrait;
            nameText.text = newInteractor.Data.DisplayName;
        }

        public void OnNextInteractor()
        {
            PartyEvents.RaiseNextInteractorRequest();
        }

        public void OnPreviousInteractor()
        {
            PartyEvents.RaisePreviousInteractorRequest();
        }
    }
}