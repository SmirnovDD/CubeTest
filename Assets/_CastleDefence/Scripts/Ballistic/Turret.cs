// LICENSE
//
//   This software is dual-licensed to the public domain and under the following
//   license: you are granted a perpetual, irrevocable license to copy, modify,
//   publish, and distribute this file as you see fit.

using UnityEngine;
using System.Collections.Generic;

public class Turret : MonoBehaviour {

    // Inspector fields
    [SerializeField] GameObject projPrefab;
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
        float gravity = -Physics.gravity.y;
        Vector3 projPos = muzzle.position;

        if (state == State.Searching) {
            if (_targets.Count > 0)
            {
                curTarget = GetClosestEnemy(_targets);
                state = State.Aiming;
            }

        }

        if (state == State.Aiming) {

            Vector3 targetPos = curTarget.transform.position;
            Vector3 diff = targetPos - projPos;
            Vector3 diffGround = new Vector3(diff.x, 0f, diff.z);

            if (_aimMode == Parameters.AimMode.Normal) 
            {
                Vector3[] solutions = new Vector3[2];
                int numSolutions;

                if (curTarget.velocity.sqrMagnitude > 0)
                    numSolutions = fts.solve_ballistic_arc(projPos, projSpeed, targetPos, curTarget.velocity, gravity, out solutions[0], out solutions[1]);
                else
                    numSolutions = fts.solve_ballistic_arc(projPos, projSpeed, targetPos, gravity, out solutions[0], out solutions[1]);

                if (numSolutions > 0) {
                    transform.forward = diffGround;

                    var proj = GameObject.Instantiate<GameObject>(projPrefab);
                    var motion = proj.GetComponent<BallisticMotion>();
                    motion.Initialize(projPos, gravity);

                    var index = solutionIndex % numSolutions;
                    var impulse = solutions[index];
                    ++solutionIndex;

                    motion.AddImpulse(impulse);

                    state = State.Firing;
                }
            }
            else if (_aimMode == Parameters.AimMode.Lateral) {
                Vector3 fireVel, impactPos;

                if (fts.solve_ballistic_arc_lateral(projPos, projSpeed, targetPos, curTarget.velocity, _arcPeak, out fireVel, out gravity, out impactPos)) {
                    transform.forward = diffGround;

                    var proj = GameObject.Instantiate<GameObject>(projPrefab);
                    var motion = proj.GetComponent<BallisticMotion>();
                    motion.Initialize(projPos, gravity);

                    motion.AddImpulse(fireVel);

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
        for (int i = 0; i < enemies.Count; i++)
        {
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
