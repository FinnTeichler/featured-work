using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UBGKO.UI;
using UBGKO.Interactables;

namespace UBGKO.Party.Controls
{
    public class PartyControls : MonoBehaviour
    {
        private const string IDLE = "idle";

        [Header("Movement")]
        [SerializeField] private InputActionReference tapAction;
        [SerializeField] private InputActionReference pointerPositionAction;
        [Header("Feedback")][Space(5)]
        [SerializeField] private LayerMask clickableLayers;
        [SerializeField] private ParticleSystem clickEffect;

        private List<PartyMember> partyMembers => PartyManager.Instance.GetPartyMembers().ToList();
        private PartyMember Interactor => partyMembers.Count > 0 ? partyMembers[interactorIndex] : null;
        private int interactorIndex = 0;

        private IInteractable currentTarget;
        private bool isRotating = false;
        private Quaternion targetRotation;

        private void OnEnable()
        {
            UIEvents.OnUIOpened += SwitchToUIMap;
            UIEvents.OnUIClosed += SwitchToWorldSpaceMap;

            PartyEvents.OnNextInteractorRequested += SwitchToNextInteractor;
            PartyEvents.OnPreviousInteractorRequested += SwitchToPreviousInteractor;

            PartyEvents.OnPartyCreated += Initialize;
        }

        private void OnDisable()
        {
            UIEvents.OnUIOpened -= SwitchToUIMap;
            UIEvents.OnUIClosed -= SwitchToWorldSpaceMap;

            PartyEvents.OnNextInteractorRequested -= SwitchToNextInteractor;
            PartyEvents.OnPreviousInteractorRequested -= SwitchToPreviousInteractor;

            PartyEvents.OnPartyCreated -= Initialize;
        }

        private void SwitchToUIMap() => GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
        private void SwitchToWorldSpaceMap() => GetComponent<PlayerInput>().SwitchCurrentActionMap("WorldSpace");

        private void Initialize()
        {
            SetInteractor(0);
        }

        private void Update()
        {
            SetAnimations();
            
            if (HasTarget() && HasArrivedAtTarget())
            {
                if (!isRotating)
                    SetRotation(currentTarget.GetInteractionFacingPoint());

                RotateTowards(targetRotation);

                if(!isRotating)
                {
                    TryInteract(currentTarget);
                    currentTarget = null;
                }
            }
        }

        public void OnTap(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Canceled)
            {
                Vector2 pos = pointerPositionAction.action.ReadValue<Vector2>();

                if (Physics.Raycast(Camera.main.ScreenPointToRay(pos), out RaycastHit hit, 200, clickableLayers))
                {
                    GameObject hitObject = hit.collider.gameObject;

                    IInteractable interactable = hitObject.GetComponent<IInteractable>();
                    if (interactable != null)
                    {
                        MoveToAndInteract(interactable);
                    }
                    else if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 1.0f, NavMesh.AllAreas))
                    {
                        MoveParty(navHit.position);

                        if (clickEffect != null)
                            Instantiate(clickEffect, hit.point += new Vector3(0, 0.1f, 0), clickEffect.transform.rotation);
                    }
                }
            }
        }

        public void SwitchToNextInteractor()
        {
            if (partyMembers.Count <= 1) return;
            int index = (interactorIndex + 1) % partyMembers.Count;
            SetInteractor(index);
        }

        public void SwitchToPreviousInteractor()
        {
            if (partyMembers.Count <= 1) return;
            int index = (interactorIndex - 1 + partyMembers.Count) % partyMembers.Count;
            SetInteractor(index);
        }

        public void SetInteractor(PartyMember member)
        {
            if (partyMembers.Count <= 1) return;

            int index = partyMembers.IndexOf(member);
            if (index != -1)
                SetInteractor(index);
        }

        public void SetInteractor(int index)
        {
            if (index >= 0 || index < partyMembers.Count)
            {
                interactorIndex = index;
                PartyEvents.RaiseInteractorChanged(partyMembers[interactorIndex]);
            }
            else
                Debug.LogWarning("PartyControls SetInteractor index to invalid index");
        }

        private void MoveParty(Vector3 destination)
        {
            if (Interactor == null) return;

            Interactor.GetAgent().SetDestination(destination);

            // Define some fixed offsets
            Vector3[] offsets = new Vector3[]
            {
                new Vector3(1.5f, 0, 1.5f),
                new Vector3(-1.5f, 0, 1.5f),
                new Vector3(1.5f, 0, -1.5f),
                new Vector3(-1.5f, 0, -1.5f),
                new Vector3(0, 0, 2.5f),
            };

            int offsetIndex = 0;
            for (int i = 0; i < partyMembers.Count; i++)
            {
                if (partyMembers[i] == Interactor)
                    continue;  // Skip the interactor, but keep looping

                Vector3 offset;
                if (offsetIndex < offsets.Length)
                    offset = offsets[offsetIndex];
                else
                    offset = (Vector3)UnityEngine.Random.insideUnitCircle * 2f;

                offsetIndex++;  // increment only when we assign an offset
                Vector3 followPos = destination + offset;
                NavMesh.SamplePosition(followPos, out NavMeshHit navHit, 1.0f, NavMesh.AllAreas);
                partyMembers[i].GetAgent().SetDestination(navHit.position);
            }
        }

        private void TryInteract(IInteractable target)
        {
            var interactions = target.GetAvailableInteractions();
            if (interactions.Count > 0)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(target.GetInteractionPoint());
                InteractionEvents.RaiseMenuRequest(target, screenPos);
            }
        }

        private void MoveToAndInteract(IInteractable target)
        {
            currentTarget = target;
            Vector3 targetPoint = target.GetInteractionPoint();
            Interactor.GetAgent().SetDestination(targetPoint);
        }

        private bool HasTarget()
        {
            if (currentTarget != null && !Interactor.GetAgent().pathPending) return true;
            else return false;
        }

        private bool HasArrivedAtTarget()
        {
            float distance = Vector3.Distance(Interactor.transform.position, Interactor.GetAgent().destination);
            if (distance <= Interactor.GetAgent().stoppingDistance + 0.1f)
            {
                return true;
            }
            else return false;
        }

        private void SetRotation(Vector3 targetPoint)
        {
            Vector3 direction = (targetPoint - Interactor.transform.position).normalized;
            direction.y = 0;
            if (direction.sqrMagnitude < 0.01f) return;

            targetRotation = Quaternion.LookRotation(direction);
            isRotating = true;
        }

        private void RotateTowards(Quaternion targetRotation)
        {
            Interactor.transform.rotation = Quaternion.RotateTowards(Interactor.transform.rotation, targetRotation, Time.deltaTime * 360f);

            float angle = Quaternion.Angle(Interactor.transform.rotation, targetRotation);
            if (angle < 1f)
                isRotating = false;
        }

        private void SetAnimations()
        {
            for (int i = 0; i < partyMembers.Count; i++)
            {
                if (partyMembers[i].GetAgent().velocity == Vector3.zero)
                    partyMembers[i].GetAnimator().SetBool(IDLE, true);
                else
                    partyMembers[i].GetAnimator().SetBool(IDLE, false);
            }
        }
    }
}