using System.Collections.Generic;
using UniRx;
using Unity.Mathematics;
using UnityEngine;


public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private float _maxPlaceDistance = 10f;
    [SerializeField] private LayerMask _placeObjectLayer;
    [SerializeField] private Camera _camera;
    [SerializeField] private Material _stubMaterial;
    [SerializeField] private Material _blockedMaterial;
    [SerializeField] private Material _destroyMaterial;
    private Transform _objectPlacingTransform;
    private CollisionChecker _objectPlacingCollisionChecker;
    private GameObject _destroyingGameObject;
    private IColorChangable _objectPlacingColorChangable;
    private IColorChangable _objectToDestroyColorChangable;
    private bool _canPlace;
    private bool _isDeleting;
    private ObjectToPlaceType _objectToPlaceType;
    private ObjectToPlaceType _lastPlacedObjectType;
    private Quaternion _lastPlacedObjectRotation;
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
        _objectPlacingTransform.rotation = GetInitialPlacementRotation();
        _objectPlacingCollisionChecker = _objectPlacingTransform.GetComponent<CollisionChecker>();
        _objectPlacingColorChangable = _objectPlacingTransform.GetComponent<IColorChangable>();
        _objectPlacingCollisionChecker.IsBlocked.Subscribe(v =>
        {
            var mat = v ? _blockedMaterial : _stubMaterial;
            _objectPlacingColorChangable.SetMaterial(mat);
        }).AddTo(_objectPlacingCollisionChecker);
        
        if (_objectPlacingColorChangable != null)
            _objectPlacingColorChangable.SetMaterial(_stubMaterial);
        _objectToPlaceType = objectToPlaceType;
    }

    private void Update()
    {
        PlaceObject();
        if(Input.GetMouseButtonDown(0))
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
            var targetSnapPoint = hit.point;
            var offset = Vector3.zero;

            var objectPlacingOn = hit.collider.gameObject.GetComponent<IPlacedObject>();
            var objectToPlace = _objectPlacingTransform.gameObject.GetComponent<IPlacedObject>();
            WorldSnapPoint closestSnapPoint = null;

            if (objectPlacingOn != null)
            {
                if (objectPlacingOn.IsGround)
                {
                    targetSnapPoint = hit.point;
                }
                else
                    closestSnapPoint = TryGetClosestSnapPointToCursorRaycastWithDistance(objectPlacingOn, objectToPlace, hit.point);
            }

            if (closestSnapPoint != null)
                targetSnapPoint = closestSnapPoint.WorldPosition;

            WorldSnapPoint closestPlacingObjectSnapPoint = null;
            if (objectToPlace != null)
            {
                closestPlacingObjectSnapPoint = TryGetClosestSnapPointToCursorRaycast(objectToPlace, objectPlacingOn, hit.normal);
                if (closestPlacingObjectSnapPoint != null)
                    offset = GetOffsetFromCenterToSnapPoint(_objectPlacingTransform.position, closestPlacingObjectSnapPoint.WorldPosition);
            }

            // if (objectToPlace != null && closestPlacingObjectSnapPoint != null && closestSnapPoint != null)
            // {
            //     if (ObjectPlacementUtility.CheckForSupportRule(objectToPlace, closestPlacingObjectSnapPoint.Direction,
            //             closestSnapPoint.Direction))
            //         _canPlace = true;
            // }
            // else
            // {
            //     _canPlace = false;
            // }
            _canPlace = true;
            if (objectPlacingOn != null && objectPlacingOn.IsGround)
            {
                _canPlace = true;
            }
            
            return targetSnapPoint + offset;
        }

        _canPlace = false;
        return Vector3.down * 500;
    }

    private Quaternion GetInitialPlacementRotation()
    {
        if (_objectToPlaceType == _lastPlacedObjectType)
            return _lastPlacedObjectRotation;
        return quaternion.identity;
    }
    
    private void ReleaseObject()
    {
        bool haveObjectToPlace = _objectPlacingTransform != null;
        bool notCollidingWithBlockingObject = _objectPlacingCollisionChecker != null && !_objectPlacingCollisionChecker.IsBlocked.Value;
        if (haveObjectToPlace && _canPlace && notCollidingWithBlockingObject)
        {
            var rb = _objectPlacingTransform.GetComponent<Rigidbody>();
            Destroy(rb);
            var placedObject = _objectPlacingTransform.GetComponent<PlacedObject>();
            SetupNeighbours(_objectPlacingCollisionChecker.Colliders, placedObject);
            _objectPlacingTransform.gameObject.layer = LayerMask.NameToLayer("Default");

            if (placedObject.ColliderToSwap != null)
            {
                _objectPlacingTransform.gameObject.layer = LayerMask.NameToLayer("PlacedObjectIgnorePlayer");
                placedObject.ColliderToSwap.gameObject.layer = LayerMask.NameToLayer("Default");
                placedObject.ColliderToSwap.isTrigger = false;
            }

            _objectPlacingCollisionChecker.Collider.isTrigger = false;
            _lastPlacedObjectRotation = _objectPlacingTransform.rotation;
            _lastPlacedObjectType = _objectToPlaceType;
            placedObject.OnPlaced();
            Destroy(_objectPlacingCollisionChecker);
            _objectPlacingTransform = null;
            _objectPlacingCollisionChecker = null;
            if (_objectPlacingColorChangable != null)
                _objectPlacingColorChangable.SetDefaultMaterial();
            SetObjectPlacing(_objectToPlaceType);
        }
    }

    private void SetupNeighbours(List<Collider> _colliders, PlacedObject placedObject)
    {
        foreach (var collidingObject in _colliders)
        {
            var neighbour = collidingObject.gameObject;
            var neighbourObject = neighbour.GetComponent<IPlacedObject>();
            if (neighbourObject == null)
                continue;
            AddNeighbourToPlacedObject(placedObject, neighbourObject);
            AddNeighbourToPlacedObject(neighbourObject, placedObject);
        }
    }

    private void AddNeighbourToPlacedObject(IPlacedObject placedObject, IPlacedObject neighbourObject)
    {
        if (CheckForGroundObject(placedObject, neighbourObject))
            return;
        
        var connectedFromSide = ObjectPlacementUtility.GetConnectedSideFromNeighbourObject(placedObject, neighbourObject);
        if (connectedFromSide != ConnectedFromSide.None)
            placedObject.AddNeighbourObject(new NeighbourObject(neighbourObject, connectedFromSide));
    }

    private bool CheckForGroundObject(IPlacedObject placedObject, IPlacedObject neighbourObject)
    {
        if (placedObject.IsGround)
            return true;
        if (neighbourObject.IsGround)
        {
            placedObject.AddNeighbourObject(new NeighbourObject(neighbourObject, ConnectedFromSide.Bottom));
            return true;
        }

        return false;
    }
    
    public static GameObject GetGameObjectByName(string name)
    {
        GameObject instance = Instantiate(Resources.Load(name, typeof(GameObject)), Vector3.down * 500, Quaternion.identity) as GameObject;
        if (instance == null)
            Debug.LogError($"{name} GO not found in resources");
        
        return instance;
    }

    public static WorldSnapPoint TryGetClosestSnapPointToCursorRaycastWithDistance(IPlacedObject objectPlacingOn, IPlacedObject objectToPlace, Vector3? hitPoint)
    {
        var snapPoints = ObjectPlacementUtility.GetSnapPointsFromPlacedObject(objectPlacingOn, objectToPlace);
        
        if (hitPoint.HasValue)
        {
            float closestDistanceToHitPoint = int.MaxValue; //ideally dot product of point and normal should be -1, meaning that points will collide like that ---><---
            int closestSnapPointToHitPointIndex = 0;
            //if I want to do walls like in Vallheim (2 blocks tall), then I should make a list of correctly aligned points
            //and choose one of them based on is cursor above half of the block I want to snap to |||||point 2  cursor points at upper half of a cube from side
            //                                                                                    |||||point 1 so choose point 1, if it points to lower - point 2
            for (var i = 0; i < snapPoints.Length; i++)
            {
                var distanceFromSnapPointToHitPoint = (snapPoints[i].WorldPosition - hitPoint.Value).magnitude;
                if (distanceFromSnapPointToHitPoint < closestDistanceToHitPoint)
                {
                    closestDistanceToHitPoint = distanceFromSnapPointToHitPoint;
                    closestSnapPointToHitPointIndex = i;
                }
            }
            
            return snapPoints[closestSnapPointToHitPointIndex];
        }
        
        return snapPoints[0];
    }
    
    public static WorldSnapPoint TryGetClosestSnapPointToCursorRaycast(IPlacedObject objectPlacingOn, IPlacedObject objectToPlace, Vector3? hitPointNormal)
    {
        if (objectToPlace == null)
            return null;
        
        var fromTransform = objectPlacingOn.Collider.transform;
        var snapPoints = ObjectPlacementUtility.GetSnapPointsFromPlacedObject(objectPlacingOn, objectToPlace);
        
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
                var currentPointVectorToPlacingObjectCenter = (sortedPoint.WorldPosition - fromTransform.position).normalized;
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

    public void StopPlacing()
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
        
        if (placedObjectComponent.IsGround)
            return;
        
        Destroy(placedObject);
        _destroyingGameObject = null;
        _objectToDestroyColorChangable = null;
    }
}

public class WorldSnapPoint
{
    public SnapPointsDirections Direction { get; }
    public Vector3 WorldPosition { get; }

    public WorldSnapPoint(SnapPointsDirections direction, Vector3 worldPosition)
    {
        Direction = direction;
        WorldPosition = worldPosition;
    }
}
