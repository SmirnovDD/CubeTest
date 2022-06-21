using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Rigidbody _projectilePrefab;
    [SerializeField] private float _shootForce;
    [SerializeField] private Vector3 _shootPointOffset = new Vector3(0f, 1f, 0.3f);
    [SerializeField] private LayerMask _shootCheckLayerMask;
    
    private Transform _cameraTransform;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0))
            Shoot();
    }

    private void Shoot()
    {
        var position = transform.position;
        var shootPoint = position + _shootPointOffset;
        var projectileRb = Instantiate(_projectilePrefab, shootPoint, quaternion.identity);
        var ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
        var targetPoint = ray.direction * 500f;

        var shootVector = (targetPoint - shootPoint).normalized;
        Debug.DrawLine(shootPoint, shootVector, Color.cyan, 1f);
        projectileRb.AddForce(shootVector * _shootForce);
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position + _shootPointOffset, 0.2f);
    }
#endif
}
