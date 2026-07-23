using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;

public class Cube : MonoBehaviour
{
    // public static event Action<CubeDirection> OnInitializationCompleted;
    [SerializeField] private CubeDirection cubeDirection = CubeDirection.Forward;
    [SerializeField] private ArrowQuadGenerator arrowQuadGenerator;
    [SerializeField] private CubeMover cubeMover;
    private MeshRenderer meshRenderer;
    private Rigidbody rb;
    private Vector3 positionInWorld;
    private readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    private MaterialPropertyBlock cubeMaterialPropertyBlock;
    private Bounds cubeBound;
    private List<QuadConfig> quadConfigList;
    private List<ArrowQuad> quadList;
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        cubeBound = meshRenderer.localBounds;
        quadList = new List<ArrowQuad>();
        cubeMaterialPropertyBlock = new();
        quadConfigList = new();
    }
    public void InitBySO(CubeSO cubeData)
    {
        cubeDirection = cubeData.cubeDirection;
        // SetColor(cubeData.cubeColor);
        quadConfigList = arrowQuadGenerator.GetQuadConfigs(cubeData.cubeDirection);
        InitVisual(cubeData.cubeColor, cubeData.symbolColor);
    }
    public void InitByCubeData(CubeData cubeData)
    {
        cubeDirection = cubeData.moveDirection;
        FreezePosition();
        cubeMover.CubeDirection = cubeDirection;
        positionInWorld = cubeData.position;
        transform.position = positionInWorld;
        cubeMover.StartPosition = positionInWorld;
        quadConfigList = arrowQuadGenerator.GetQuadConfigs(cubeData.moveDirection);
        Color cubeColor = GetColorByCode(cubeData.cubeColor);
        Color symbolColor = GetColorByCode(cubeData.symbolColor);
        InitVisual(cubeColor, symbolColor);
        // OnInitializationCompleted?.Invoke(cubeDirection);
    }
    private void SetColor(Color cubeColor)
    {
        meshRenderer.GetPropertyBlock(cubeMaterialPropertyBlock);
        cubeMaterialPropertyBlock.SetColor(BaseColorID, cubeColor);
        meshRenderer.SetPropertyBlock(cubeMaterialPropertyBlock);
    }
    private Color GetColorByCode(string code)
    {
        Color newColor = Color.white;
        if (ColorUtility.TryParseHtmlString(code, out Color color))
        {
            newColor = color;
        }
        else
        {
            Debug.Log("Fail to load cube color");
        }
        return newColor;
    }
    private void InitVisual(Color cubeColor, Color symbolColor)
    {
        SetColor(cubeColor);
        Vector3 moveDirection = CubeDirectionHelper.GetDirectionVector(cubeDirection);

        foreach (QuadConfig quadConfig in quadConfigList)
        {
            float dot = Vector3.Dot(quadConfig.faceDirection.normalized, moveDirection.normalized);
            if (dot < -0.999f)
            {
                continue;
            }
            Vector3 quadPosition = arrowQuadGenerator.CalculateArrowQuadPosition(cubeBound, quadConfig.faceDirection);
            ArrowQuad arrowQuad = arrowQuadGenerator.CreateArrowQuad(quadConfig, arrowQuadGenerator.transform, quadPosition);
            bool isArrow = dot < 0.999f;
            arrowQuad.Init(symbolColor, isArrow);
        }
    }
    private void FreezePosition()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
}
