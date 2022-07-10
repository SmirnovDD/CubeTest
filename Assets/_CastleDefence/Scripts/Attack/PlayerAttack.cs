using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private BallisticMotion _projectilePrefab;
    [SerializeField] private float _shootForce;
    [SerializeField] private Vector3 _shootPointOffset = new Vector3(0f, 1f, 0.3f);
    
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
        var projectile = Instantiate(_projectilePrefab, shootPoint, Quaternion.identity);
        var ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
        var targetPoint = ray.direction * 500f;

        var shootVector = (targetPoint - shootPoint).normalized;
        projectile.transform.forward = shootVector;
        projectile.Initialize(shootPoint, Physics.gravity.y);
        projectile.AddImpulse(shootVector * _shootForce);
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position + _shootPointOffset, 0.2f);
    }
#endif
}
