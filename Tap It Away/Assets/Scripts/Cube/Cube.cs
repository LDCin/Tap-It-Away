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
    [SerializeField] private float ghostOpacity = 0.8f;
    [SerializeField] private float trailStartOpacity = 0.6f;
    [SerializeField] private float trailEndOpacity = 0f;
    private MeshRenderer meshRenderer;
    private TrailRenderer trailRenderer;
    private Rigidbody rb;
    private readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    private MaterialPropertyBlock cubeMaterialPropertyBlock;
    private Bounds cubeBound;
    private List<QuadConfig> quadConfigList;
    private List<ArrowQuad> quadList;
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
        rb = GetComponent<Rigidbody>();
        cubeBound = meshRenderer.localBounds;
        quadList = new List<ArrowQuad>();
        cubeMaterialPropertyBlock = new();
        quadConfigList = new();
    }
    private void OnEnable()
    {
        SetOpacity(1);
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
        transform.localPosition = cubeData.position;
        cubeMover.StartPosition = transform.position;
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
    public void SetOpacity(float opacity)
    {
        meshRenderer.GetPropertyBlock(cubeMaterialPropertyBlock);

        Color color = Color.white;
        if (cubeMaterialPropertyBlock.HasColor(BaseColorID))
        {
            color = cubeMaterialPropertyBlock.GetColor(BaseColorID);
        }
        else
        {
            color = meshRenderer.sharedMaterial.GetColor(BaseColorID);
        }

        color.a = opacity;
        cubeMaterialPropertyBlock.SetColor(BaseColorID, color);
        meshRenderer.SetPropertyBlock(cubeMaterialPropertyBlock);
    }
    public void SetCubeGhostVisual()
    {
        SetOpacity(ghostOpacity);
        foreach (var quad in quadList)
        {
            quad.SetGhostOpacity();
        }
    }
    public void SetCubeNormalVisual()
    {
        SetOpacity(1f);
        foreach (var quad in quadList)
        {
            quad.SetNormalOpacity();
        }
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
        SetTrailColor(cubeColor);
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
    private void SetTrailColor(Color cubeColor)
    {
        if (trailRenderer == null)
        {
            return;
        }

        Gradient trailGradient = new();
        trailGradient.SetKeys(
            new[]
            {
                new GradientColorKey(cubeColor, 0f),
                new GradientColorKey(cubeColor, 1f)
            },
            new[]
            {
                new GradientAlphaKey(trailStartOpacity, 0f),
                new GradientAlphaKey(trailEndOpacity, 1f)
            }
        );

        trailRenderer.colorGradient = trailGradient;
    }
    private void FreezePosition()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
}
