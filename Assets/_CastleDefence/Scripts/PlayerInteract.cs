using Cinemachine;
using UnityEngine;

namespace _CastleDefence.Scripts
{
    public class PlayerInteract : MonoBehaviour
    {
        [SerializeField] private float _interactDistance = 15f;
        [SerializeField] private LayerMask _mask;
        [SerializeField] private Transform _cameraTransform;

        private void Update()
        {
            RaycastHit hit;
            bool hitObject = Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out hit, _interactDistance, _mask);
            if (hitObject && Input.GetKeyDown(KeyCode.E))
            {
                var interactable = hit.collider.GetComponentInParent<IInteractable>();
                if (interactable != null)
                    interactable.Interact();
            }
        }
    }
}