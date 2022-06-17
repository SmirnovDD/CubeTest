using UnityEngine;

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
        SteeringBasics steeringBasics;
        Cohesion cohesion;
        Separation separation;
        VelocityMatch velocityMatch;
        WallAvoidance wallAvoidance;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
            cohesion = GetComponent<Cohesion>();
            separation = GetComponent<Separation>();
            velocityMatch = GetComponent<VelocityMatch>();
            wallAvoidance = GetComponent<WallAvoidance>();
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
                    accel += cohesion.GetSteering(leaderUnitsensor.targets) * cohesionWeight;
                    accel += separation.GetSteering(flockingUnitsensor.targets) * separationWeight;
                    accel += velocityMatch.GetSteering(leaderUnitsensor.targets) * velocityMatchWeight;
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