using UnityEngine;

public class InputManager : MonoBehaviour {
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float raycastDistance = 100f;
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended)
            {
                ShootRaycast(touch.position);
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            ShootRaycast(Input.mousePosition);
        }
    }
    private void ShootRaycast(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
        {
            GameObject other = hit.collider.gameObject;
            if (other.CompareTag(GameConfig.CUBE_TAG))
            {
                Cube cube = other.GetComponent<Cube>();
                cube.Move();
            }
        }
    }
}