﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class BuildingGUI : MonoBehaviour
{
    [SerializeField] private GameObject _uiPanel;
    [SerializeField] private Button _deleteButton;
    [SerializeField] private Image _cursor;
    [SerializeField] private ObjectPlacer _objectPlacer;
    private bool _enabled;

    private void Awake()
    {
        _deleteButton.onClick.AddListener(OnDeleteButtonClicked);
    }

    public void OnClick(string s)
    {
        OnBuildButtonClicked(s);
    }
    
    private void OnDestroy()
    {
        _deleteButton.onClick.RemoveListener(OnDeleteButtonClicked);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            ToggleUI();
    }

    private void ToggleUI()
    {
        _enabled = !_enabled;
        _uiPanel.SetActive(_enabled);
        _cursor.enabled = !_cursor.enabled;
        Cursor.lockState = _enabled ? CursorLockMode.Confined : CursorLockMode.Locked;
        if (_enabled)
            _objectPlacer.StopPlacing();
    }
    
    private void OnBuildButtonClicked(string type)
    {
        if (Enum.TryParse(type, out ObjectToPlaceType objectToPlaceType))
        {
            _objectPlacer.SetObjectPlacing(objectToPlaceType);
            _objectPlacer.ToggleDeleting(false);
            ToggleUI();
        }
    }

    private void OnDeleteButtonClicked()
    {
        _objectPlacer.ToggleDeleting(true);
    }
}