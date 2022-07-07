using UnityEngine;
using UnityMovementAI;

public class UnitAttack : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private UnitMovement _unitMovement;
    [SerializeField] private WallAvoidanceCharacterController _wallAvoidanceCharacterController;
    [SerializeField] private LayerMask _attackMask;
    [SerializeField] private Vector3 _attackRayCheckStartOffset = Vector3.up * 0.5f;
    [SerializeField] private float _attackRange = 1f;
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _attackCooldown = 1.5f;
    [SerializeField] private float _attackDelayMin = 0;
    [SerializeField] private float _attackDelayMax = 100f;
    
    private Transform _transform;
    private float _attackTimer;
    private float _decisionToAttackDelay;
    private bool _chosenAttackDelay;
    private static readonly int AnimatorAttackTriggerName = Animator.StringToHash("Attack");

    private void Awake()
    {
        _transform = transform;
    }

    private void Update()
    {
        var target = TryGetTarget();
        
        if (target == null)
        {
            _decisionToAttackDelay = 0;
            _chosenAttackDelay = false;
            ToggleUnitMovement(true);
            return;
        }

        if (_wallAvoidanceCharacterController.PathBlocked && !_chosenAttackDelay)
        {
            _decisionToAttackDelay = Time.time + Random.Range(_attackDelayMin, _attackDelayMax); //TODO do not reassign
            _chosenAttackDelay = true;
        }

        if (target.IsCharacter)
            _decisionToAttackDelay = 0;
        
        if (Time.time < _decisionToAttackDelay)
            return;
        
        ToggleUnitMovement(false);

        if (Time.time > _attackTimer)
        {
            Attack(target);
            _chosenAttackDelay = false;
        }
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
        _animator.SetTrigger(AnimatorAttackTriggerName);
        StartAttackCooldown();
    }

    private void StartAttackCooldown()
    {
        _attackTimer = Time.time + _attackCooldown;
    }
}
