using System;
using System.Collections;
using System.Linq;
using StarterAssets;
using UnityEngine;


public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private float _maxPlaceDistance = 10f;
    [SerializeField] private LayerMask _placeObjectLayer;
    [SerializeField] private Camera _camera;
    [SerializeField] private Material _stubMaterial;
    [SerializeField] private Material _destroyMaterial;
    private Transform _objectPlacingTransform;
    private CollisionChecker _objectPlacingCollisionChecker;
    private GameObject _destroyingGameObject;
    private IColorChangable _objectPlacingColorChangable;
    private IColorChangable _objectToDestroyColorChangable;
    private bool _canPlace;
    private bool _isDeleting;
    private ObjectToPlaceType _objectToPlaceType;
    
    public void ToggleDeleting(bool value)
    {
        _isDeleting = value;
        if (_isDeleting)
            StopPlacing();
    }
    
    public void SetObjectPlacing(ObjectToPlaceType objectToPlaceType)
    {
        StopPlacing();
        _objectPlacingTransform = GetGameObjectByName(objectToPlaceType.ToString()).transform;
        _objectPlacingCollisionChecker = _objectPlacingTransform.GetComponent<CollisionChecker>();
        _objectPlacingColorChangable = _objectPlacingTransform.GetComponent<IColorChangable>();
        if (_objectPlacingColorChangable != null)
            _objectPlacingColorChangable.SetMaterial(_stubMaterial);
        _objectToPlaceType = objectToPlaceType;
    }

    private void Update()
    {
        PlaceObject();
        ReleaseObject();
        if (_isDeleting)
        {
            TryDestroySelectedPlacedObject();
        }
    }

    public void PlaceObject()
    {
        if (_objectPlacingTransform == null)
        {
            _canPlace = false;
            return;
        }


        _objectPlacingTransform.position = GetPlacementPosition();
        if (Input.mouseScrollDelta.y > 0) {
            _objectPlacingTransform.Rotate(Vector3.up, 22.5f);
        }
        if (Input.mouseScrollDelta.y < 0) {
            _objectPlacingTransform.Rotate(Vector3.up, -22.5f);
        }
    }

    private Vector3 GetPlacementPosition()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _maxPlaceDistance, _placeObjectLayer))
        {
            _canPlace = true;
            var targetSnapPoint = hit.point;
            var offset = Vector3.zero;
            
            var closestSnapPoint = TryGetClosestSnapPointToCursorRaycast(hit.collider.gameObject, -hit.normal);
            if (closestSnapPoint.HasValue)
                targetSnapPoint = closestSnapPoint.Value;

            var closestPlacingObjectSnapPoint = TryGetClosestSnapPointToCursorRaycast(_objectPlacingTransform.gameObject, hit.normal);
            if (closestPlacingObjectSnapPoint.HasValue)
                offset = GetOffsetFromCenterToSnapPoint(_objectPlacingTransform.position, closestPlacingObjectSnapPoint.Value);
            
            return targetSnapPoint + offset;
        }

        _canPlace = false;
        return Vector3.down * 500;
    }

    private void ReleaseObject() //TODO remove snap points
    {
        if (Input.GetMouseButtonDown(0) && _objectPlacingTransform && _canPlace && !_objectPlacingCollisionChecker.IsColliding.Value)
        {
            Destroy(_objectPlacingTransform.GetComponent<Rigidbody>());
            _objectPlacingTransform.gameObject.layer = LayerMask.NameToLayer("Default");
            _objectPlacingCollisionChecker.Collider.isTrigger = false;
            _objectPlacingTransform = null;
            _objectPlacingCollisionChecker = null;
            if (_objectPlacingColorChangable != null)
                _objectPlacingColorChangable.SetDefaultMaterial();
            SetObjectPlacing(_objectToPlaceType);
        }
    }
    
    public static GameObject GetGameObjectByName(string name)
    {
        GameObject instance = Instantiate(Resources.Load(name, typeof(GameObject))) as GameObject;
        if (instance == null)
            Debug.LogError($"{name} GO not found in resources");
        
        return instance;
    }

    public static Vector3? TryGetClosestSnapPointToCursorRaycast(GameObject from, Vector3? hitPointNormal)
    {
        var fromTransform = from.transform;
        var placingObject = from.GetComponent<PlacedObject>();
        if (placingObject == null)
            return null;
        
        var snapPoints = ObjectPlacementUtility.GetSnapPointsFromPlacedObject(placingObject);
        
        if (hitPointNormal.HasValue)
        {
            float leastAllignedSnapPointWithNormalDotProduct = int.MaxValue; //ideally dot product of point and normal should be -1, meaning that points will collide like that ---><---
            int leastAllignedSnapPointWithNormalIndex = 0;
            //if I want to do walls like in Vallheim (2 blocks tall), then I should make a list of correctly aligned points
            //and choose one of them based on is cursor above half of the block I want to snap to |||||point 2  cursor points at upper half of a cube from side
            //                                                                                    |||||point 1 so choose point 1, if it points to lower - point 2
            for (var i = 0; i < snapPoints.Length; i++)
            {
                var sortedPoint = snapPoints[i];
                var currentPointVectorToPlacingObjectCenter = (sortedPoint - fromTransform.position).normalized;
                var dotProductOfVectorFromPointToCenterAndHitNormal = Vector3.Dot(currentPointVectorToPlacingObjectCenter, hitPointNormal.Value);
                if (dotProductOfVectorFromPointToCenterAndHitNormal < leastAllignedSnapPointWithNormalDotProduct)
                {
                    leastAllignedSnapPointWithNormalDotProduct = dotProductOfVectorFromPointToCenterAndHitNormal;
                    leastAllignedSnapPointWithNormalIndex = i;
                }
            }
            
            return snapPoints[leastAllignedSnapPointWithNormalIndex];
        }
        
        return snapPoints[0];
    }

    public static Vector3 GetOffsetFromCenterToSnapPoint(Vector3 center, Vector3 snapPoint)
    {
        return center - snapPoint;
    }

    private void TryDestroySelectedPlacedObject()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _maxPlaceDistance, _placeObjectLayer))
        {
            if (_destroyingGameObject != hit.collider.gameObject)
            {
                if (_objectToDestroyColorChangable != null)
                    _objectToDestroyColorChangable.SetDefaultMaterial();
                _destroyingGameObject = hit.collider.gameObject;
                _objectToDestroyColorChangable = _destroyingGameObject.GetComponent<IColorChangable>();
                if (_objectToDestroyColorChangable != null)
                    _objectToDestroyColorChangable.SetMaterial(_destroyMaterial);
            }
            if (Input.GetMouseButtonDown(0))
                DestroyPlacedObject(_destroyingGameObject);
        }
        else
        {
            if (_objectToDestroyColorChangable != null)
                _objectToDestroyColorChangable.SetDefaultMaterial();
        }
    }

    private void StopPlacing()
    {
        if (_objectPlacingTransform != null)
            Destroy(_objectPlacingTransform.gameObject);
    }
    
    private void DestroyPlacedObject(GameObject placedObject)
    {
        if (placedObject == null)
            return;

        var placedObjectComponent = placedObject.GetComponent<IPlacedObject>();
        
        if (placedObjectComponent == null)
            return;
        
        Destroy(placedObject);
        _destroyingGameObject = null;
        _objectToDestroyColorChangable = null;
    }
}
