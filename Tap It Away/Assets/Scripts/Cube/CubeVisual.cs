using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;

public class CubeVisual : MonoBehaviour
{
    // public static event Action<CubeDirection> OnInitializationCompleted;
    [SerializeField] private CubeDirection cubeDirection = CubeDirection.Forward;
    [SerializeField] private ArrowQuadGenerator arrowQuadGenerator;
    [SerializeField] private CubeMover cubeMover;
    [SerializeField] private float ghostOpacity = 0.8f;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private float gradientStart = 1f;
    [SerializeField] private float gradientEnd = 0f;
    private Transform cubeParent;
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
    public void InitByCubeData(CubeData cubeData, Transform parent)
    {
        cubeParent = parent;
        gameObject.transform.SetParent(cubeParent);
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
    private void SetUpTrailRenderer(Color color)
    {
        if (trailRenderer == null)
        {
            return;
        }

        Gradient gradient = new();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(color, 0),
                new GradientColorKey(color, 1)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(gradientStart, 0),
                new GradientAlphaKey(gradientEnd, 1)
            }
        );

        trailRenderer.colorGradient = gradient;
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

        SetUpTrailRenderer(cubeColor);
    }
    public void ChangeTrailRendererState(bool isOn)
    {
        if (trailRenderer == null)
        {
            return;
        }

        if (isOn)
        {
            trailRenderer.Clear();
            trailRenderer.emitting = true;
        }
        else
        {
            trailRenderer.emitting = false;
            trailRenderer.Clear();
        }
    }
    private void FreezePosition()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
    public void SetHasParent(bool hasParent)
    {
        if (hasParent)
        {
            gameObject.transform.parent = cubeParent;
        }
        else
        {
            gameObject.transform.parent = null;
        }
    }
    public void ResetToInitialState()
    {
        SetHasParent(true);
        SetOpacity(1f);
        ChangeTrailRendererState(false);

        foreach (var quad in quadList)
        {
            if (quad != null)
            {
                quad.SetNormalOpacity();
            }
        }
    }
}
