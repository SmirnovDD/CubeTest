using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private UnitSpawner[] _spawners;
    [SerializeField] private float _spawnTime;

    private float _timer;

    private void Start()
    {
        _timer = Time.time + _spawnTime;
    }

    private void Update()
    {
        if (Time.time > _timer || Input.GetKeyDown(KeyCode.F11))
        {
            Spawn();
            _timer = Time.time + _spawnTime;
        }
    }

    private void Spawn()
    {
        foreach (var unitSpawner in _spawners)
        {
            unitSpawner.Spawn();
        }
    }
}
