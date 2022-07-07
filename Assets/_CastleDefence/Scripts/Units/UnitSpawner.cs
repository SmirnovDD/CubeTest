using UnityEngine;
using UnityMovementAI;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private int _min;
    [SerializeField] private int _max;
    [SerializeField] private GameObject _prefab;

    // Update is called once per frame
    public void Spawn()
    {
        var randomAmount = Random.Range(_min, _max);
        for (int i = 0; i < randomAmount; i++)
        {
            var unit = Instantiate(_prefab, transform);
            unit.GetComponentInChildren<FlockingArriveUnit>().target = _target;
        }
    }
}
