using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float raycastDistance = 100f;
    [SerializeField] private float dragThreshHold = 20f;
    [SerializeField] LayerMask cubeLayer;
    private Vector2 touchBeganPosition;
    private bool isDragging = false;
    private CubeMover selectedCube;

    private void Update()
    {
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
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            // GetFirstCubeOnRaycast(Input.mousePosition);
        }
#endif
    }
    private CubeMover GetFirstCubeOnRaycast(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, cubeLayer))
        {
            CubeMover other = hit.collider.GetComponent<CubeMover>();
            return other;
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
            RotatePuzzleBlock();
        }
    }
    private void HandleTouchEnded(Touch touch)
    {
        if (!isDragging && selectedCube != null)
        {
            selectedCube.MoveOut();
        }
        ResetTouch();
    }
    private void ResetTouch()
    {
        selectedCube = null;
        isDragging = false;
    }
    private void RotatePuzzleBlock()
    {

    }
}