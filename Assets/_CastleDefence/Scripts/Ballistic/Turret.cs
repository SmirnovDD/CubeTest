// LICENSE
//
//   This software is dual-licensed to the public domain and under the following
//   license: you are granted a perpetual, irrevocable license to copy, modify,
//   publish, and distribute this file as you see fit.

using UnityEngine;
using System.Collections.Generic;
using ShipShooting.Math;

public class Turret : MonoBehaviour {

    // Inspector fields
    [SerializeField] BallisticMotion projPrefab;
    [SerializeField] Transform muzzle;
    // Private fields
    private List<CharacterController> _targets = new ();
    CharacterController curTarget;
    State state = State.Searching;
    float cooldownTime;
    uint solutionIndex;
    bool paused;
    [SerializeField] private float _velocity = 2000f;
    [SerializeField] private Parameters.AimMode _aimMode;
    private float _rateOfFire = 3f;
    private float _arcPeak = 3f;

    // Helper enums
    enum State {
        Searching,
        Aiming,
        Firing,
        Waiting
    };
    
    // Methods
    void Update() {
        float projSpeed = _velocity;
        float gravity = Physics.gravity.y;
        Vector3 projPos = muzzle.position;

        if (state == State.Searching) {
            if (_targets.Count > 0)
            {
                curTarget = GetClosestEnemy(_targets);
                if (curTarget != null)
                    state = State.Aiming;
            }

        }

        if (state == State.Aiming) {

            Vector3 targetPos = curTarget.transform.position + Vector3.up;
            Vector3 diff = targetPos - projPos;
            Vector3 diffGround = new Vector3(diff.x, 0f, diff.z);

            if (_aimMode == Parameters.AimMode.Normal) 
            {
                Vector3 targetDirLow;
                Vector3 targetDirHigh;
                float shootAngle;

                var muzzlePosition = muzzle.position;
                
                BallisticShootingMath.SolveBallisticArc(muzzlePosition, _velocity, targetPos, -gravity, out targetDirLow, out targetDirHigh, out shootAngle);
			
                Vector3 targetVelocity = curTarget.velocity;
                var ammo = Instantiate(projPrefab, muzzlePosition, Quaternion.identity);
                ammo.Initialize(muzzlePosition, gravity);
                
                Vector3 targetPredictedPos = BallisticShootingMath.ApproximateTargetPositionBallisticSimple(ammo.transform.position, _velocity, shootAngle, targetPos, targetVelocity);
                //Vector3 scatter = Random.insideUnitCircle * (_shipCannons.ShipCannonsParams.ShootScatter + _shipCannons.GetScatterModifier());
                //Quaternion scatterRotation = Quaternion.FromToRotation(Vector3.forward, Vector3.up);
                //Vector3 rotatedScatter = scatterRotation * scatter;
                //Vector3 targetPredictedPosWithScatter = targetPredictedPos + rotatedScatter;
                BallisticShootingMath.SolveBallisticArc(muzzlePosition, _velocity, targetPredictedPos, -gravity, out targetDirLow, out targetDirHigh, out shootAngle);
            
                ammo.transform.forward = targetDirLow.normalized;
                ammo.AddImpulse(targetDirLow);

                state = State.Firing;
            }
            else if (_aimMode == Parameters.AimMode.Lateral) {
                Vector3 fireVel, impactPos;

                if (fts.solve_ballistic_arc_lateral(projPos, projSpeed, targetPos, curTarget.velocity, _arcPeak, out fireVel, out gravity, out impactPos)) {
                    transform.forward = diffGround;

                    var proj = Instantiate(projPrefab);
                    
                    proj.Initialize(projPos, gravity);
                    proj.AddImpulse(fireVel);

                    state = State.Firing;
                }
            }
            else {
                state = State.Searching;
            }
        }

        if (state == State.Firing) {
            float cooldown = 1f / _rateOfFire;
            cooldownTime = Time.time + cooldown;
            state = State.Waiting;
        }

        if (state == State.Waiting) {
            if (Time.time > cooldownTime)
                state = State.Searching;
        }
    }
    
    CharacterController GetClosestEnemy(List<CharacterController> enemies)
    {
        CharacterController bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i] == null)
            {
                enemies.RemoveAt(i);
                continue;
            }
            Vector3 directionToTarget = enemies[i].transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if(dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = enemies[i];
            }
        }
 
        return bestTarget;
    }

    private void OnTriggerEnter(Collider other)
    {
        var characterController = other.GetComponent<CharacterController>();
        if (characterController != null)
            _targets.Add(characterController);
    }

    private void OnTriggerExit(Collider other)
    {
        var characterController = other.GetComponent<CharacterController>();
        if (characterController != null)
            _targets.Remove(characterController);
    }
}
