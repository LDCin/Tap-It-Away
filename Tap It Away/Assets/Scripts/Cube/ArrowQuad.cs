using UnityEngine;

public class ArrowQuad : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private readonly int BaseColorID = Shader.PropertyToID("_BaseColor"); 
    private readonly int BaseMapID = Shader.PropertyToID("_BaseMap"); 
    private MaterialPropertyBlock arrowQuadMaterialPropertyBlock;
    [SerializeField] private Texture2D arrow;
    [SerializeField] private Texture2D circle;
    // [SerializeField] private bool isArrow = true;
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        arrowQuadMaterialPropertyBlock = new MaterialPropertyBlock();
        // Init(Color.red);
    }
    public void Init(Color arrowColor, bool isArrow)
    {
        SetColor(arrowColor);
        SetTexture(isArrow);
    }
    private void SetColor(Color color)
    {
        meshRenderer.GetPropertyBlock(arrowQuadMaterialPropertyBlock);
        arrowQuadMaterialPropertyBlock.SetColor(BaseColorID, color);
        meshRenderer.SetPropertyBlock(arrowQuadMaterialPropertyBlock);
    }
    private void SetTexture(bool isArrow)
    {
        Texture2D texture = isArrow == true ? arrow : circle;
        meshRenderer.GetPropertyBlock(arrowQuadMaterialPropertyBlock);
        arrowQuadMaterialPropertyBlock.SetTexture(BaseMapID, texture);
        meshRenderer.SetPropertyBlock(arrowQuadMaterialPropertyBlock);
    }
}