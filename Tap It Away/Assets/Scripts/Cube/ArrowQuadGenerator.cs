using System.Collections.Generic;
using UnityEngine;

public class ArrowQuadGenerator : MonoBehaviour
{
    [SerializeField] private ArrowQuad arrowQuadPrefab;
    [SerializeField] private float subOffset = 0.002f;
    public ArrowQuad CreateArrowQuad(QuadConfig quadConfig, Transform parent, Vector3 positionOnCube)
    {
        ArrowQuad arrowQuad = Instantiate(arrowQuadPrefab, parent);
        arrowQuad.name = quadConfig.quadName;
        arrowQuad.transform.localPosition = positionOnCube;
        arrowQuad.transform.localRotation = Quaternion.LookRotation(quadConfig.faceDirection, quadConfig.upDirection);
        // quadList.Add(arrowQuad);
        return arrowQuad;
    }
    public Vector3 CalculateArrowQuadPosition(Bounds cubeBound, Vector3 faceDirection)
    {
        Vector3 cubeCenter = cubeBound.center;
        Vector3 extents = cubeBound.extents;
        float distanceFromCenter = Mathf.Abs(faceDirection.x) * extents.x + Mathf.Abs(faceDirection.y) * extents.y + Mathf.Abs(faceDirection.z) * extents.z;
        return cubeCenter + faceDirection * (distanceFromCenter + subOffset);
    }
    public List<QuadConfig> GetQuadConfigs(CubeDirection moveDirection)
    {
        Vector3 arrowDirection = CubeDirectionHelper.GetDirectionVector(moveDirection);
        return new()
        {
            CreateConfig("Quad_Up", Vector3.up, arrowDirection),
            CreateConfig("Quad_Down", Vector3.down, arrowDirection),
            CreateConfig("Quad_Left", Vector3.left, arrowDirection),
            CreateConfig("Quad_Right", Vector3.right, arrowDirection),
            CreateConfig("Quad_Forward", Vector3.forward, arrowDirection),
            CreateConfig("Quad_Back", Vector3.back, arrowDirection)
        };
    }
    private QuadConfig CreateConfig(string quadName, Vector3 faceDirection, Vector3 moveDirection)
    {
        Vector3 upDirection;

        bool isParallel = Mathf.Abs(Vector3.Dot(faceDirection.normalized, moveDirection.normalized)) > 0.999f;

        if (isParallel)
        {
            upDirection = CubeDirectionHelper.GetDefaultUpDirection(faceDirection);
        }
        else
        {
            upDirection = moveDirection;
        }

        return new QuadConfig(quadName, faceDirection, upDirection);
    }
}