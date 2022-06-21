using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Destructable _destructable;
    [SerializeField] private Image _bar;
    private Transform _cameraTransform;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        UpdateBarValue();
        FaceTheCamera();
    }

    private void UpdateBarValue()
    {
        _bar.fillAmount = _destructable.Durability / _destructable.MaxDurability;
    }

    private void FaceTheCamera()
    {
        transform.rotation = _cameraTransform.rotation;
    }
}
