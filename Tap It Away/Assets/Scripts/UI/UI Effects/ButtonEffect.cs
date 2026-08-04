using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Button))]
public class ButtonEffect : UIEffect, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("Scale")]
    [SerializeField] private bool useScaleEffect = true;
    [SerializeField] private float scaleFactor = 0.95f;
    [SerializeField] private float scaleDuration = 0.1f;

    [Header("Rotation")]
    [SerializeField] private bool useRotationEffect;
    [SerializeField] private float rotationAngle = 360f;
    [SerializeField] private float rotationDuration = 0.3f;

    [Header("Feedback")]
    [SerializeField] private bool useVibrationEffect = true;

    [Header("Cooldown")]
    [SerializeField] private bool useCooldown;
    [SerializeField] private float cooldownDuration = 0.5f;

    private Button button;
    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Coroutine cooldownCoroutine;

    private void Awake()
    {
        button = GetComponent<Button>();
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnClick);
        }
    }

    private void OnClick()
    {
        PlayRotation();
        PlayVibration();
        StartCooldown();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button != null && button.interactable)
        {
            ScaleToPressed();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ScaleToNormal();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ScaleToNormal();
    }

    private void ScaleToPressed()
    {
        if (!useScaleEffect)
        {
            return;
        }

        rectTransform.DOKill();
        rectTransform.DOScale(originalScale * scaleFactor, scaleDuration).SetEase(Ease.OutQuad);
    }

    private void ScaleToNormal()
    {
        if (!useScaleEffect || rectTransform == null)
        {
            return;
        }

        rectTransform.DOKill();
        rectTransform.DOScale(originalScale, scaleDuration).SetEase(Ease.OutBack);
    }

    private void PlayRotation()
    {
        if (!useRotationEffect)
        {
            return;
        }

        Vector3 targetRotation = new Vector3(0f, 0f, rectTransform.eulerAngles.z + rotationAngle);
        rectTransform.DORotate(targetRotation, rotationDuration, RotateMode.FastBeyond360);
    }

    private void PlayVibration()
    {
        if (!useVibrationEffect)
        {
            return;
        }
    }

    private void StartCooldown()
    {
        if (!useCooldown)
        {
            return;
        }

        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }

        cooldownCoroutine = StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        button.interactable = false;
        yield return new WaitForSeconds(cooldownDuration);
        button.interactable = true;
        cooldownCoroutine = null;
    }
}
