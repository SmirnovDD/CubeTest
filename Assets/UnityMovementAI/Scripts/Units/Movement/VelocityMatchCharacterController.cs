using UnityEngine;
using System.Collections.Generic;
using UnityMovementAI.UnityMovementAI;

namespace UnityMovementAI
{
    public class VelocityMatchCharacterController : MonoBehaviour
    {
        public float facingCosine = 90;
        public float timeToTarget = 0.1f;
        public float maxAcceleration = 4f;

        float facingCosineVal;

        CharacterController _characterController;
        SteeringBasicsCharacterController steeringBasics;

        void Awake()
        {
            facingCosineVal = Mathf.Cos(facingCosine * Mathf.Deg2Rad);

            _characterController = GetComponent<CharacterController>();
            steeringBasics = GetComponent<SteeringBasicsCharacterController>();
        }

        public Vector3 GetSteering(ICollection<CharacterController> targets)
        {
            Vector3 accel = Vector3.zero;
            int count = 0;

            foreach (CharacterController r in targets)
            {
                if (steeringBasics.IsFacing(r.transform.position, facingCosineVal))
                {
                    /* Calculate the acceleration we want to match this target */
                    Vector3 a = r.velocity - _characterController.velocity;
                    /* Rather than accelerate the character to the correct speed in 1 second, 
                     * accelerate so we reach the desired speed in timeToTarget seconds 
                     * (if we were to actually accelerate for the full timeToTarget seconds). */
                    a = a / timeToTarget;

                    accel += a;

                    count++;
                }
            }

            if (count > 0)
            {
                accel = accel / count;

                /* Make sure we are accelerating at max acceleration */
                if (accel.magnitude > maxAcceleration)
                {
                    accel = accel.normalized * maxAcceleration;
                }
            }

            return accel;
        }
    }
}