using UnityEngine;
using System.Collections.Generic;

namespace UnityMovementAI
{
    public class SeparationCharacterController : MonoBehaviour
    {
        /// <summary>
        /// The maximum acceleration for separation
        /// </summary>
        public float sepMaxAcceleration = 25;

        /// <summary>
        /// This should be the maximum separation distance possible between a
        /// separation target and the character. So it should be: separation
        /// sensor radius + max target radius
        /// </summary>
        public float maxSepDist = 1f;

        CharacterController _characterController;

        void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        public Vector3 GetSteering(ICollection<CharacterController> targets)
        {
            Vector3 acceleration = Vector3.zero;

            foreach (CharacterController r in targets)
            {
                /* Get the direction and distance from the target */
                Vector3 direction = _characterController.transform.position - r.transform.position;
                float dist = direction.magnitude;

                if (dist < maxSepDist)
                {
                    /* Calculate the separation strength (can be changed to use inverse square law rather than linear) */
                    var strength = sepMaxAcceleration * (maxSepDist - dist) / (maxSepDist - _characterController.radius - r.radius);

                    /* Added separation acceleration to the existing steering */
                    direction.Normalize();
                    acceleration += direction * strength;
                }
            }

            return acceleration;
        }
    }
}