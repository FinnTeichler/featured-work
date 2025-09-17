using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UBGKO.Interactables;
using UBGKO.UI;

namespace UBGKO.PartyControls.UI
{
    public class AvailableInteractionsContextMenu : MonoBehaviour
    {
        [SerializeField] private Canvas canvas; // Main UI canvas
        [SerializeField] private RectTransform panel; // The UI panel that holds the buttons
        [SerializeField] private GameObject content; //The parent of the buttons in the menu
        [SerializeField] private GameObject buttonPrefab; // The button prefab

        private IInteractable currentTarget;
        private IInteractable lastTarget;

        private void OnEnable()
        {
            InteractionEvents.RequestInteractionMenu += ShowInteractions;
        }

        private void OnDisable()
        {
            InteractionEvents.RequestInteractionMenu -= ShowInteractions;
        }

        public void ShowInteractions(IInteractable target, Vector2 screenPos)
        {
            UIEvents.OnUIOpened?.Invoke();

            ClearButtons();
            SpawnButtons(target);

            PlaceMenuAtScreenPos(screenPos);

            panel.gameObject.SetActive(true);
        }

        public void Hide()
        {
            UIEvents.OnUIClosed?.Invoke();
            ClearButtons();
            panel.gameObject.SetActive(false);
            currentTarget = null;
        }

        private void ClearButtons()
        {
            foreach (Transform child in content.transform)
            {
                Destroy(child.gameObject);
            }
        }

        private void SpawnButtons(IInteractable target)
        {
            currentTarget = target;

            List<InteractionType> available = target.GetAvailableInteractions();

            foreach (var interactionType in available)
            {
                GameObject buttonObj = Instantiate(buttonPrefab, content.transform);
                Button button = buttonObj.GetComponent<Button>();
                TMP_Text label = buttonObj.GetComponentInChildren<TMP_Text>();
                Image icon = buttonObj.transform.Find("Icon")?.GetComponent<Image>();

                if (label != null)
                    label.text = interactionType.displayName;

                if (icon != null && interactionType.icon != null)
                    icon.sprite = interactionType.icon;

                lastTarget = currentTarget;

                button.onClick.AddListener(() => {
                    Hide();
                    lastTarget.Interact(interactionType);
                    InteractionEvents.RaiseInteraction(target, interactionType);
                });
            }
        }

        private void PlaceMenuAtScreenPos(Vector2 screenPos)
        {
            Vector2 clampedPos = screenPos;
            clampedPos.x = Mathf.Min(clampedPos.x, Screen.width - panel.rect.width);
            clampedPos.y = Mathf.Max(clampedPos.y, panel.rect.height); // y inverted in UI space

            // Convert screen point to canvas local position
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPos,
                null,
                out Vector2 localPoint);

            panel.anchoredPosition = localPoint;
        }
    }
}