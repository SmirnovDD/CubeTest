using UnityEngine;

public class BallisticMotion : MonoBehaviour 
{
    private Vector3 _impulse;
    private float _shotTimer;
    private Transform _transform;
    private float _gravity;
    private Vector3 _lastPos;
    
    public void Initialize(Vector3 pos, float gravity)
    {
        _transform = transform;
        _transform.position = pos;
        _gravity = gravity;
    }

    protected void Update ()
    {
        float dt = Time.deltaTime;
            
        Vector3 curPos = _transform.position;
        Vector3 newPos = curPos + (_transform.forward * (_impulse.magnitude * dt)) + Vector3.up * (_gravity * (dt * dt));
        _lastPos = curPos;
        _transform.position = newPos;
        _transform.forward = newPos - _lastPos;

        if (_transform.position.y <= -0.5)
        {
            Destroy(gameObject);
        }
    }

    public virtual void AddImpulse(Vector3 impulse)
    {
        _impulse = impulse;
    }
}
