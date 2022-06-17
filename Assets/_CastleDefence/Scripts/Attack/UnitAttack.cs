using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityMovementAI;

public class UnitAttack : MonoBehaviour
{
    [SerializeField] private UnitMovement _unitMovement;
    [SerializeField] private LayerMask _attackMask;
    [SerializeField] private Vector3 _attackRayCheckStartOffset = Vector3.up * 0.5f;
    [SerializeField] private float _attackRange = 1f;
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _attackCooldown = 1.5f;
    private Transform _transform;
    private float _timer;
    
    private void Awake()
    {
        _transform = transform;
    }

    private void Update()
    {
        var target = TryGetTarget();
        
        if (target == null)
        {
            ToggleUnitMovement(true);
            return;
        }
        
        ToggleUnitMovement(false);
        
        if (Time.time > _timer)
            Attack(target);
    }

    private void ToggleUnitMovement(bool value)
    {
        if (_unitMovement != null)
            _unitMovement.MovementActive = value;
    }

    private IDestructable TryGetTarget()
    {
        var attackRayStartPos = _transform.position + _attackRayCheckStartOffset;
        if (Physics.SphereCast(attackRayStartPos, 0.5f,_transform.forward, out var hit, _attackRange, _attackMask))
        {
            return hit.collider.GetComponentInParent<IDestructable>();
        }

        return null;
    }
    
    private void Attack(IDestructable target)
    {
        target.Damage(_damage);
        StartAttackCooldown();
    }

    private void StartAttackCooldown()
    {
        _timer = Time.time + _attackCooldown;
    }
}
