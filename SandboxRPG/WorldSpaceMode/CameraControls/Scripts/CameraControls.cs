using UnityEngine;
using UnityEngine.InputSystem;

namespace UBGKO.PartyControls
{
    public class CameraControls : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActionAsset;
        [Space(5)]
        [SerializeField] private float moveSpeed;
        [SerializeField] private float moveTime;

        private InputAction actionMoveCamera;

        private void Start()
        {
            actionMoveCamera = inputActionAsset["MoveCamera"];
        }

        private void OnEnable()
        {
            inputActionAsset.FindActionMap("WorldSpace").Enable();
        }

        private void OnDisable()
        {
            inputActionAsset.FindActionMap("WorldSpace").Disable();
        }

        private void Update()
        {
            UpdateCameraPosition();
        }

        private void UpdateCameraPosition()
        {
            Vector3 newPosition = transform.position;

            if (InputDirection().y > 0) { newPosition += transform.forward * moveSpeed; }
            if (InputDirection().y < 0) { newPosition += transform.forward * -moveSpeed; }
            if (InputDirection().x < 0) { newPosition += transform.right * -moveSpeed; }
            if (InputDirection().x > 0) { newPosition += transform.right * moveSpeed; }

            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * moveTime);
        }

        private Vector2 InputDirection()
        {
            return actionMoveCamera.ReadValue<Vector2>();
        }
    }
}