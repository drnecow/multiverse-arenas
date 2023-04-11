using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;

    private bool _movementActive;
    private Vector2 _lastMousePosition;

    [SerializeField] private float _moveSpeed = 30f;

    private float _minZoomBound = 10f;
    private float _maxZoomBound = 85f;
    private float _currentOrthSize = 45f;


    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        HandleCameraMovement();
        HandleCameraZoom();
    }

    private void HandleCameraMovement()
    {
        Vector2 inputDir = new Vector3(0, 0, 0);

        if (Input.GetMouseButtonDown(1))
        {
            _movementActive = true;
            _lastMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(1))
        {
            _movementActive = false;
        }

        if (_movementActive)
        {
            Vector2 mouseMovementDelta = (Vector2)Input.mousePosition - _lastMousePosition;

            float dragPanSpeed = 1f;
            inputDir.x = mouseMovementDelta.x * dragPanSpeed;
            inputDir.y = mouseMovementDelta.y * dragPanSpeed;

            _lastMousePosition = Input.mousePosition;
        }

        Vector3 moveDir = new Vector3(inputDir.x, inputDir.y);
        transform.position += moveDir * _moveSpeed * Time.deltaTime;
    }

    private void HandleCameraZoom()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            _currentOrthSize -= 5f;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            _currentOrthSize += 5f;
        }

        _currentOrthSize = Mathf.Clamp(_currentOrthSize, _minZoomBound, _maxZoomBound);
        _virtualCamera.m_Lens.OrthographicSize = _currentOrthSize;
    }
}
