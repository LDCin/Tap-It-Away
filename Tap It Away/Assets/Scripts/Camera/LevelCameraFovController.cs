// using System.Collections.Generic;
// using UnityEngine;

// [RequireComponent(typeof(Camera))]
// public class LevelCameraFovController : MonoBehaviour
// {
//     [SerializeField] private Camera targetCamera;
//     [SerializeField] private Transform puzzleRoot;
//     [SerializeField, Min(0f)] private float cubeSize = 1f;
//     [SerializeField, Min(0f)] private float padding = 1.5f;
//     [SerializeField, Range(1f, 179f)] private float minFov = 35f;
//     [SerializeField, Range(1f, 179f)] private float maxFov = 75f;

//     private void Awake()
//     {
//         if (targetCamera == null)
//         {
//             targetCamera = GetComponent<Camera>();
//         }
//     }

//     private void OnEnable()
//     {
//         Observer.Subscribe(ObserverEvent.LevelLoaded, FitToCurrentLevel);
//     }

//     private void OnDisable()
//     {
//         Observer.Unsubscribe(ObserverEvent.LevelLoaded, FitToCurrentLevel);
//     }

//     [ContextMenu("Fit To Current Level")]
//     public void FitToCurrentLevel()
//     {
//         if (targetCamera == null || LevelManager.Instance == null)
//         {
//             return;
//         }

//         FitToCubes(LevelManager.Instance.LevelCubeList);
//     }

//     private void FitToCubes(List<CubeMover> cubeList)
//     {
//         if (cubeList == null || cubeList.Count == 0)
//         {
//             return;
//         }

//         float maxAbsPosition = GetMaxAbsCubePosition(cubeList);
//         float levelHalfSize = maxAbsPosition + cubeSize * 0.5f + padding;
//         float distance = GetCameraDistanceToLevelCenter();

//         if (distance <= 0.01f)
//         {
//             return;
//         }

//         float verticalFov = Mathf.Rad2Deg * 2f * Mathf.Atan(levelHalfSize / distance);
//         float horizontalFov = Mathf.Rad2Deg * 2f * Mathf.Atan(levelHalfSize / (distance * targetCamera.aspect));
//         float targetFov = Mathf.Clamp(Mathf.Max(verticalFov, horizontalFov), minFov, maxFov);

//         targetCamera.fieldOfView = targetFov;
//     }

//     private float GetMaxAbsCubePosition(List<CubeMover> cubeList)
//     {
//         float maxAbsPosition = 0f;

//         foreach (CubeMover cubeMover in cubeList)
//         {
//             if (cubeMover == null)
//             {
//                 continue;
//             }

//             Vector3 position = GetCubePositionInLevelSpace(cubeMover.transform);
//             maxAbsPosition = Mathf.Max(
//                 maxAbsPosition,
//                 Mathf.Abs(position.x),
//                 Mathf.Abs(position.y),
//                 Mathf.Abs(position.z)
//             );
//         }

//         return maxAbsPosition;
//     }

//     private Vector3 GetCubePositionInLevelSpace(Transform cubeTransform)
//     {
//         if (puzzleRoot == null)
//         {
//             return cubeTransform.localPosition;
//         }

//         return puzzleRoot.InverseTransformPoint(cubeTransform.position);
//     }

//     private float GetCameraDistanceToLevelCenter()
//     {
//         Vector3 levelCenter = puzzleRoot != null ? puzzleRoot.position : Vector3.zero;
//         Vector3 cameraToLevel = levelCenter - targetCamera.transform.position;
//         float forwardDistance = Mathf.Abs(Vector3.Dot(cameraToLevel, targetCamera.transform.forward));

//         return forwardDistance > 0.01f ? forwardDistance : cameraToLevel.magnitude;
//     }
// }
