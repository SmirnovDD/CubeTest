using UnityEngine;

namespace ShipShooting.Math
{
    public class BallisticShootingMath
    {
        public static int SolveBallisticArc(Vector3 projPos, float projSpeed, Vector3 target, float gravity, out Vector3 s0, out Vector3 s1, out float shootAngle) 
        {
            Debug.Assert(projSpeed > 0 && gravity > 0, "Solve ballistic arc called with invalid data");

            s0 = Vector3.zero;
            s1 = Vector3.zero;
            shootAngle = 0;

            Vector3 diff = target - projPos;
            Vector3 diffXZ = new Vector3(diff.x, 0f, diff.z);
            float groundDist = diffXZ.magnitude;

            float speed2 = projSpeed*projSpeed;
            float speed4 = speed2*speed2;
            float y = diff.y;
            float x = groundDist;
            float gx = gravity*x;

            float root = speed4 - gravity*(gravity*x*x + 2*y*speed2);

            // No solution
            if (root < 0)
            {
                Debug.LogError("No shooting solution!");
                return 0;
            }

            root = Mathf.Sqrt(root);

            float lowAng = Mathf.Atan2(speed2 - root, gx);
            float highAng = Mathf.Atan2(speed2 + root, gx);
            int numSolutions = lowAng != highAng ? 2 : 1;
            shootAngle = lowAng;
            Vector3 groundDir = diffXZ.normalized;
            s0 = groundDir * (Mathf.Cos(lowAng) * projSpeed) + Vector3.up * (Mathf.Sin(lowAng) * projSpeed);
            if (numSolutions > 1)
                s1 = groundDir * (Mathf.Cos(highAng) * projSpeed) + Vector3.up * (Mathf.Sin(highAng) * projSpeed);
            
            return numSolutions;
        }
        
        public static Vector3 ApproximateTargetPositionBallisticSimple(Vector3 ammoPosition, float ammoVelocity, float shootAngle, Vector3 targetPosition, Vector3 targetVelocity)
        {
            if (targetVelocity == Vector3.zero)
                return targetPosition;
	        
            Vector3 diff = targetPosition - ammoPosition;
            Vector3 diffXZ = new Vector3(diff.x, 0f, diff.z);
            float groundDist = diffXZ.magnitude;

            float timeToReachCurrentTargetPos = GetTimeToHitTargetWithBallistics(groundDist, ammoVelocity, shootAngle);
	    
            diff = PredictPos(targetPosition, targetVelocity, timeToReachCurrentTargetPos) - ammoPosition;
            diffXZ = new Vector3(diff.x, 0f, diff.z);
            groundDist = diffXZ.magnitude;
	    
            float timeToReachPredictedTargetPos = GetTimeToHitTargetWithBallistics(groundDist, ammoVelocity, shootAngle);
		
            return PredictPos(targetPosition, targetVelocity, timeToReachPredictedTargetPos);
        }

        private static float GetTimeToHitTargetWithBallistics(float toTargetGroundDistance, float projectileVelocity, float shootAngle)
        {
            if (projectileVelocity * Mathf.Cos(shootAngle) == 0)
                return float.MaxValue;
	    
            float time = toTargetGroundDistance / (projectileVelocity * Mathf.Cos(shootAngle));
            return time;
        }
        
        private static Vector3 PredictPos(Vector3 position, Vector3 velocity, float time)
        {
            return velocity * time + position;
        }
    }
}