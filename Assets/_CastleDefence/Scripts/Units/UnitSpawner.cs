using UnityEngine;
using UnityMovementAI;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private float _spawnTime = 60f;
    [SerializeField] private GameObject _prefab;

    private float _timer;
    private Transform _target;
    
    private void Start()
    {
        _timer = Time.time + _spawnTime;
        _target = FindObjectOfType<PlayerAttack>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))

        for (int i = 0; i < 5; i++)
        {
            var unit = Instantiate(_prefab, transform);
            unit.GetComponentInChildren<FlockingArriveUnit>().target = _target;
        }
    }
}
