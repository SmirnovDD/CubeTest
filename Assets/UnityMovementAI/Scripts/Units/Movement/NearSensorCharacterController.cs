using UnityEngine;
using System.Collections.Generic;

namespace UnityMovementAI
{
    public class NearSensorCharacterController : MonoBehaviour
    {
        HashSet<CharacterController> _targets = new HashSet<CharacterController>();

        public HashSet<CharacterController> targets
        {
            get
            {
                /* Remove any MovementAIRigidbodies that have been destroyed */
                _targets.RemoveWhere(IsNull);
                return _targets;
            }
        }

        static bool IsNull(CharacterController r)
        {
            return (r == null || r.Equals(null));
        }

        void TryToAdd(Component other)
        {
            CharacterController characterController = other.GetComponent<CharacterController>();
            if (characterController != null)
            {
                _targets.Add(characterController);
            }
        }

        void TryToRemove(Component other)
        {
            CharacterController characterController = other.GetComponent<CharacterController>();
            if (characterController != null)
            {
                _targets.Remove(characterController);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            TryToAdd(other);
        }

        void OnTriggerExit(Collider other)
        {
            TryToRemove(other);
        }
    }
}