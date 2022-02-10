using System;
using UnityEngine;
using UnityEngine.UI;

public class BuildingGUI : MonoBehaviour
{
    [SerializeField] private GameObject _uiPanel;
    [SerializeField] private Button _smallCubeWallButton;
    [SerializeField] private Button _deleteButton;
    [SerializeField] private Image _cursor;
    [SerializeField] private ObjectPlacer _objectPlacer;
    private bool _enabled;

    private void Awake()
    {
        _smallCubeWallButton.onClick.AddListener(OnSmallCubeWallButtonClicked);
        _deleteButton.onClick.AddListener(OnDeleteButtonClicked);
    }

    private void OnDestroy()
    {
        _smallCubeWallButton.onClick.RemoveListener(OnSmallCubeWallButtonClicked);
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
        _uiPanel.SetActive(_enabled);;
        _cursor.enabled = !enabled;
        Cursor.lockState = _enabled ? CursorLockMode.Confined : CursorLockMode.Locked;
    }
    
    private void OnSmallCubeWallButtonClicked()
    {
        _objectPlacer.SetObjectPlacing(ObjectToPlaceType.SmallCubeWall);
        _objectPlacer.ToggleDeleting(false);
    }

    private void OnDeleteButtonClicked()
    {
        _objectPlacer.ToggleDeleting(true);
    }
}