using System;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class BuildingGUI : MonoBehaviour
{
    [SerializeField] private GameObject _uiPanel;
    [SerializeField] private Button _deleteButton;
    [SerializeField] private Image _cursor;
    [SerializeField] private ObjectPlacer _objectPlacer;
    [SerializeField] private ThirdPersonController _thirdPersonController;
    private bool _uiEnabled;

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
        _uiEnabled = !_uiEnabled;
        _uiPanel.SetActive(_uiEnabled);
        _cursor.enabled = !_cursor.enabled;
        Cursor.lockState = _uiEnabled ? CursorLockMode.Confined : CursorLockMode.Locked;
        if (_uiEnabled)
            _objectPlacer.StopPlacing();
        _thirdPersonController.LockCameraYRotation = _uiEnabled;
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