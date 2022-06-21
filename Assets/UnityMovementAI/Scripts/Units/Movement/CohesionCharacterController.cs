using UnityEngine;
using System.Collections.Generic;
using UnityMovementAI.UnityMovementAI;

namespace UnityMovementAI
{
    public class CohesionCharacterController : MonoBehaviour
    {
        public float facingCosine = 120f;

        float facingCosineVal;

        SteeringBasicsCharacterController steeringBasics;

        void Awake()
        {
            facingCosineVal = Mathf.Cos(facingCosine * Mathf.Deg2Rad);
            steeringBasics = GetComponent<SteeringBasicsCharacterController>();
        }

        public Vector3 GetSteering(ICollection<CharacterController> targets)
        {
            Vector3 centerOfMass = Vector3.zero;
            int count = 0;

            /* Sums up everyone's position who is close enough and in front of the character */
            foreach (CharacterController r in targets)
            {
                if (steeringBasics.IsFacing(r.transform.position, facingCosineVal))
                {
                    centerOfMass += r.transform.position;
                    count++;
                }
            }

            if (count == 0)
            {
                return Vector3.zero;
            }
            else
            {
                centerOfMass = centerOfMass / count;

                return steeringBasics.Arrive(centerOfMass);
            }
        }
    }
}