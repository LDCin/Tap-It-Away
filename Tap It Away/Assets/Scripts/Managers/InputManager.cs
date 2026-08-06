using System;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public static event Action<CubeMover> OnTapCube;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CastConfig castConfig;
    [SerializeField, Range(0.1f, 30f)] private float dragThreshHold = 20f;
    [SerializeField, Range(0.1f, 2f)] private float rotateSensitivity = 1f;
    [Header("Zoom")]
    [SerializeField, Range(1f, 20f)] private float mouseZoomSensitivity = 5f;
    [SerializeField, Range(0.01f, 1f)] private float pinchZoomSensitivity = 0.08f;
    [SerializeField, Range(1f, 179f)] private float minFov = 30f;
    [SerializeField, Range(1f, 179f)] private float maxFov = 80f;
    [SerializeField] private Transform puzzleRoot;
    private bool isLocked = false;
    private Vector2 touchBeganPosition;
    private bool isDragging = false;
    private CubeMover selectedCube;
    private Vector3 puzzleRootInitialLocalPosition;
    private Quaternion puzzleRootInitialLocalRotation;
    private Vector3 puzzleRootInitialLocalScale;
    public CubeMover SelectedCube => selectedCube;

    public override void Awake()
    {
        base.Awake();
        if (Instance != this)
        {
            return;
        }

        CachePuzzleRootTransform();
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (isLocked)
        {
            return;
        }

        if (Input.touchCount >= 2)
        {
            HandlePinchZoom(Input.GetTouch(0), Input.GetTouch(1));
            ResetTouch();
            return;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            // if (touch.phase == TouchPhase.Ended)
            // {
            //     ShootRaycast(touch.position);
            // }
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    HandleTouchBegan(touch);
                    break;
                case TouchPhase.Moved:
                    HandleTouchMoved(touch);
                    break;
                case TouchPhase.Ended:
                    HandleTouchEnded(touch);
                    break;
                case TouchPhase.Canceled:
                    ResetTouch();
                    break;
                default:
                    break;
            }
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            // GetFirstCubeOnRaycast(Input.mousePosition);
        }

        HandleMouseZoom();
#endif
    }
    private CubeMover GetFirstCubeOnRaycast(Vector2 screenPosition)
    {
        if (castConfig == null)
        {
            Debug.LogError("CastConfig is missing.");
            return null;
        }

        if (CastHelper.ShootRaycast(
            mainCamera,
            screenPosition,
            castConfig,
            out CubeMover cubeMover))
        {
            return cubeMover;
        }

        return null;
    }
    private void HandleTouchBegan(Touch touch)
    {
        touchBeganPosition = touch.position;
        isDragging = false;
        selectedCube = GetFirstCubeOnRaycast(touch.position);
    }
    private void HandleTouchMoved(Touch touch)
    {
        float moveDistance = Vector2.Distance(touchBeganPosition, touch.position);
        if (!isDragging && moveDistance > dragThreshHold)
        {
            isDragging = true;
            selectedCube = null;
        }
        if (isDragging)
        {
            RotatePuzzleBlock(touch.deltaPosition);
        }
    }
    private void HandleTouchEnded(Touch touch)
    {
        if (!isDragging && selectedCube != null)
        {
            OnTapCube?.Invoke(selectedCube);
            selectedCube.MoveOut();
        }
        ResetTouch();
    }
    private void ResetTouch()
    {
        selectedCube = null;
        isDragging = false;
    }
    private void RotatePuzzleBlock(Vector2 touchDelta)
    {
        float horizontalAngle = -touchDelta.x * rotateSensitivity;
        float verticalAngle = touchDelta.y * rotateSensitivity;

        puzzleRoot.Rotate(Vector3.up, horizontalAngle, Space.World);
        puzzleRoot.Rotate(mainCamera.transform.right, verticalAngle, Space.World);
    }

    private void HandleMouseZoom()
    {
        float scrollDelta = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scrollDelta) <= 0.001f)
        {
            scrollDelta = Input.GetAxis("Mouse ScrollWheel") * 10f;
        }

        if (Mathf.Abs(scrollDelta) <= 0.001f)
        {
            return;
        }

        ZoomByDelta(-scrollDelta * mouseZoomSensitivity);
    }

    private void HandlePinchZoom(Touch firstTouch, Touch secondTouch)
    {
        Vector2 firstPreviousPosition = firstTouch.position - firstTouch.deltaPosition;
        Vector2 secondPreviousPosition = secondTouch.position - secondTouch.deltaPosition;

        float previousDistance = Vector2.Distance(firstPreviousPosition, secondPreviousPosition);
        float currentDistance = Vector2.Distance(firstTouch.position, secondTouch.position);
        float pinchDelta = currentDistance - previousDistance;

        if (Mathf.Abs(pinchDelta) <= 0.01f)
        {
            return;
        }

        ZoomByDelta(-pinchDelta * pinchZoomSensitivity);
    }

    private void ZoomByDelta(float fovDelta)
    {
        if (mainCamera == null)
        {
            return;
        }

        mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView + fovDelta, minFov, maxFov);
    }
    public void LockInput()
    {
        isLocked = true;
    }
    public void UnlockInput()
    {
        isLocked = false;
    }

    private void CachePuzzleRootTransform()
    {
        if (puzzleRoot == null)
        {
            return;
        }

        puzzleRootInitialLocalPosition = puzzleRoot.localPosition;
        puzzleRootInitialLocalRotation = puzzleRoot.localRotation;
        puzzleRootInitialLocalScale = puzzleRoot.localScale;
    }

    public void ResetPuzzleRootTransform()
    {
        if (puzzleRoot == null)
        {
            return;
        }

        puzzleRoot.localPosition = puzzleRootInitialLocalPosition;
        puzzleRoot.localRotation = puzzleRootInitialLocalRotation;
        puzzleRoot.localScale = puzzleRootInitialLocalScale;
        ResetTouch();
    }
}
