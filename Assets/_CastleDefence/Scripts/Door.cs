using DG.Tweening;
using UnityEngine;

namespace _CastleDefence.Scripts
{
    public class Door : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform _doorTransform;
        [SerializeField] private float _movementSpeed = 5;
        [SerializeField] private float _maxOpenAngle = 90;
        
        private bool _opened;
        
        public void Interact()
        {
            _opened = !_opened;
            _doorTransform.DOKill();
            var angle = _opened ? _maxOpenAngle : 0;
            _doorTransform.DOLocalRotate(new Vector3(0, angle, 0), _movementSpeed).SetSpeedBased();
        }
    }
}