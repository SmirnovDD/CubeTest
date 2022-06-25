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
        public NearSensorCharacterController flockingUnitsensor;
        public NearSensorCharacterController leaderUnitsensor;
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

        void Update()
        {
            if (!MovementActive)
            {
                steeringBasics.Stop();
                return;
            }
            
            var (accel, isJumping) = wallAvoidance.GetSteering();

            if (accel.magnitude < 0.005f)
            {
                if (flockingUnitsensor.targets.Count > 0)
                {
                    accel += separation.GetSteering(flockingUnitsensor.targets) * separationWeight;
                }

                if (leaderUnitsensor.targets.Count > 0)
                {
                    accel += cohesion.GetSteering(leaderUnitsensor.targets) * cohesionWeight;
                    accel += velocityMatch.GetSteering(leaderUnitsensor.targets) * velocityMatchWeight;
                }

                if (leaderUnitsensor.targets.Count == 0)
                {
                    if (target != null)
                        accel += steeringBasics.Arrive(target.position);
                }
            }

            steeringBasics.SetTryJump(isJumping);
            steeringBasics.Steer(accel);
            steeringBasics.LookWhereYoureGoing();
        }
    }
}