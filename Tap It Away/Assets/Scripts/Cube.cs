using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class Cube : MonoBehaviour
{
    [SerializeField] private float moveDuration = 1;
    [SerializeField] private float moveDistance = 10;
    [SerializeField] private CubeDirection cubeDirection = CubeDirection.Forward;
    private MeshRenderer meshRenderer;
    public CubeData cubeDataTest;
    private readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    private MaterialPropertyBlock cubeMaterialPropertyBlock;
    private Bounds cubeBound;
    private float subOffset = 0.002f;
    private List<QuadConfig> quadConfigList;
    private List<ArrowQuad> quadList;
    [SerializeField] private ArrowQuad arrowQuadPrefab;
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        cubeBound = meshRenderer.localBounds;
        quadList = new List<ArrowQuad>();
        cubeMaterialPropertyBlock = new();
        quadConfigList = new();
    }
    private void Start()
    {
        InitBySO(cubeDataTest);
    }
    private void InitBySO(CubeData cubeData)
    {
        cubeDirection = cubeData.cubeDirection;
        SetColor(cubeData.cubeColor);
        quadConfigList = GetQuadConfigs(cubeData.cubeDirection);
        InitVisual(cubeData.symbolColor);
    }
    private void InitVisual(Color symbolColor)
    {
        Vector3 moveDirection = GetDirectionVector(cubeDirection);

        foreach (QuadConfig quadConfig in quadConfigList)
        {
            float dot = Vector3.Dot(quadConfig.faceDirection.normalized, moveDirection.normalized);

            if (dot < -0.999f)
            {
                continue;
            }

            Vector3 quadPosition = CalculateArrowQuadPosition(cubeBound, quadConfig.faceDirection, subOffset);

            ArrowQuad arrowQuad = CreateArrowQuad(quadConfig, arrowQuadPrefab, transform, quadPosition);

            bool isArrow = dot < 0.999f;
            arrowQuad.Init(symbolColor, isArrow);
        }
    }
    private Vector3 CalculateArrowQuadPosition(Bounds cubeBound, Vector3 faceDirection, float subOffset)
    {
        Vector3 cubeCenter = cubeBound.center;
        Vector3 extents = cubeBound.extents;
        float distanceFromCenter = Mathf.Abs(faceDirection.x) * extents.x + Mathf.Abs(faceDirection.y) * extents.y + Mathf.Abs(faceDirection.z) * extents.z;
        return cubeCenter + faceDirection * (distanceFromCenter + subOffset);
    }
    private ArrowQuad CreateArrowQuad(QuadConfig quadConfig, ArrowQuad arrowQuadPrefab, Transform parent, Vector3 positionOnCube)
    {
        ArrowQuad arrowQuad = Instantiate(arrowQuadPrefab, parent);

        arrowQuad.name = quadConfig.quadName;
        arrowQuad.transform.localPosition = positionOnCube;

        arrowQuad.transform.localRotation = Quaternion.LookRotation(quadConfig.faceDirection, quadConfig.upDirection);

        quadList.Add(arrowQuad);

        return arrowQuad;
    }
    private void SetColor(Color color)
    {
        meshRenderer.GetPropertyBlock(cubeMaterialPropertyBlock);
        cubeMaterialPropertyBlock.SetColor(BaseColorID, color);
        meshRenderer.SetPropertyBlock(cubeMaterialPropertyBlock);
    }
    private List<QuadConfig> GetQuadConfigs(CubeDirection moveDirection)
    {
        Vector3 arrowDirection = GetDirectionVector(moveDirection);

        return new()
        {
            CreateConfig(
                "Quad_Up",
                Vector3.up,
                arrowDirection
            ),

            CreateConfig(
                "Quad_Down",
                Vector3.down,
                arrowDirection
            ),

            CreateConfig(
                "Quad_Left",
                Vector3.left,
                arrowDirection
            ),

            CreateConfig(
                "Quad_Right",
                Vector3.right,
                arrowDirection
            ),

            CreateConfig(
                "Quad_Forward",
                Vector3.forward,
                arrowDirection
            ),

            CreateConfig(
                "Quad_Back",
                Vector3.back,
                arrowDirection
            )
        };
    }
    private QuadConfig CreateConfig(string quadName, Vector3 faceDirection, Vector3 moveDirection)
    {
        Vector3 upDirection;

        bool isParallel = Mathf.Abs(Vector3.Dot(faceDirection.normalized, moveDirection.normalized)) > 0.999f;

        if (isParallel)
        {
            upDirection = GetDefaultUpDirection(faceDirection);
        }
        else
        {
            upDirection = moveDirection;
        }

        return new QuadConfig(quadName, faceDirection, upDirection);
    }
    private Vector3 GetDefaultUpDirection(Vector3 faceDirection)
    {
        if (faceDirection == Vector3.up)
        {
            return Vector3.forward;
        }

        if (faceDirection == Vector3.down)
        {
            return Vector3.back;
        }

        return Vector3.up;
    }
    private Vector3 GetDirectionVector(CubeDirection direction)
    {
        return direction switch
        {
            CubeDirection.Up => Vector3.up,
            CubeDirection.Down => Vector3.down,
            CubeDirection.Left => Vector3.left,
            CubeDirection.Right => Vector3.right,
            CubeDirection.Forward => Vector3.forward,
            CubeDirection.Back => Vector3.back,
            _ => Vector3.zero,
        };
    }
    public void Move()
    {
        Vector3 directionVector = GetDirectionVector(cubeDirection);
        transform.DOMove(transform.position + directionVector * moveDistance, moveDuration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}
