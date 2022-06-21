using UnityEngine;
using UnityMovementAI.UnityMovementAI;

namespace UnityMovementAI
{
    public class FlockingArriveUnit : UnitMovement
    {
        public Transform target;
        public float cohesionWeight = 1.5f;
        public float separationWeight = 2f;
        public float velocityMatchWeight = 1f;
        public NearSensor flockingUnitsensor;
        public NearSensor leaderUnitsensor;
        SteeringBasicsCharacterController steeringBasics;
        CohesionCharacterController cohesion;
        SeparationCharacterController separation;
        VelocityMatchCharacterController velocityMatch;
        WallAvoidanceCharacterController wallAvoidance;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasicsCharacterController>();
            cohesion = GetComponent<CohesionCharacterController>();
            separation = GetComponent<SeparationCharacterController>();
            velocityMatch = GetComponent<VelocityMatchCharacterController>();
            wallAvoidance = GetComponent<WallAvoidanceCharacterController>();
        }

        void FixedUpdate()
        {
            if (!MovementActive)
            {
                steeringBasics.Stop();
                return;
            }
            
            Vector3 accel = Vector3.zero;
            
            accel += wallAvoidance.GetSteering();

            if (accel.magnitude < 0.005f)
            {
                if (leaderUnitsensor.targets.Count > 0)
                {
                    // accel += cohesion.GetSteering(leaderUnitsensor.targets) * cohesionWeight;
                    // accel += separation.GetSteering(flockingUnitsensor.targets) * separationWeight;
                    // accel += velocityMatch.GetSteering(leaderUnitsensor.targets) * velocityMatchWeight;
                }
                else
                {
                    if (target != null)
                        accel += steeringBasics.Arrive(target.position);
                }
            }

            steeringBasics.Steer(accel);
            steeringBasics.LookWhereYoureGoing();
        }
    }
}