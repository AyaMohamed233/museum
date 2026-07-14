using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

/// <summary>
/// تأثير إضاءة، تكبير، وتغيير لون النص للأزرار.
/// تم التعديل لمنع استهلاك المعالج (CPU) عن طريق استخدام AnimationCurve وتجنب توليد الصور برمجياً.
/// </summary>
[RequireComponent(typeof(Button))]
public class GlowButtonEffect : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    ISelectHandler, IDeselectHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("━━━ ANIMATION SETTINGS ━━━")]
    [Tooltip("مقدار التكبير عند التمرير — 1.04 = تكبير 4%")]
    public float hoverScale = 1.04f;
    [Tooltip("مقدار التصغير عند الضغط — 0.97 = تصغير 3%")]
    public float clickScale = 0.97f;
    [Tooltip("مدة الحركة بالثواني")]
    public float animDuration = 0.35f;

    [Tooltip("منحنى الحركة (Easing Curve) للتحكم في نعومة التكبير")]
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("━━━ TEXT SETTINGS ━━━")]
    [Tooltip("مرجع النص — لو فاضي بياخده أوتوماتيك")]
    public TextMeshProUGUI buttonText;
    public Color normalTextColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    public Color hoverTextColor = Color.white;
    public float colorFadeSpeed = 15f;

    [Header("━━━ GLOW (Optional) ━━━")]
    [Tooltip("صورة الإضاءة (Glow Sprite) - ضع هنا صورة خفيفة للحواف إن أردت إضاءة حقيقية")]
    public Sprite glowSprite;
    public Color glowColor = new Color(1f, 0.85f, 0.4f, 0f); // Default to transparent

    private Vector3 originalScale;
    private Coroutine scaleCoroutine;
    private Coroutine colorCoroutine;
    private bool isPointerInside = false;

    // Optional glow image reference
    private Image glowImage;

    void Awake()
    {
        originalScale = transform.localScale;

        if (buttonText == null)
            buttonText = GetComponentInChildren<TextMeshProUGUI>();

        // Create glow image child if a sprite is provided
        if (glowSprite != null)
        {
            GameObject glowObj = new GameObject("_GlowOverlay");
            glowObj.transform.SetParent(transform, false);
            glowObj.transform.SetAsFirstSibling();

            RectTransform glowRT = glowObj.AddComponent<RectTransform>();
            glowRT.anchorMin = Vector2.zero;
            glowRT.anchorMax = Vector2.one;
            glowRT.offsetMin = new Vector2(-10, -10); // Spread
            glowRT.offsetMax = new Vector2(10, 10);

            glowImage = glowObj.AddComponent<Image>();
            glowImage.sprite = glowSprite;
            glowImage.type = Image.Type.Sliced;
            glowImage.color = new Color(glowColor.r, glowColor.g, glowColor.b, 0f);
            glowImage.raycastTarget = false;
        }
    }

    void Start()
    {
        if (buttonText != null)
            buttonText.color = normalTextColor;
    }

    public void OnPointerEnter(PointerEventData eventData) => ShowGlow();
    public void OnPointerExit(PointerEventData eventData) => HideGlow();
    public void OnSelect(BaseEventData eventData) => ShowGlow();
    public void OnDeselect(BaseEventData eventData) => HideGlow();
    public void OnPointerDown(PointerEventData eventData) => AnimateScale(clickScale);
    public void OnPointerUp(PointerEventData eventData) => AnimateScale(isPointerInside ? hoverScale : 1f);

    private void ShowGlow()
    {
        isPointerInside = true;
        AnimateScale(hoverScale);

        if (colorCoroutine != null) StopCoroutine(colorCoroutine);
        if (gameObject.activeInHierarchy)
            colorCoroutine = StartCoroutine(FadeEffect(hoverTextColor, 1f));
    }

    private void HideGlow()
    {
        isPointerInside = false;
        AnimateScale(1f);

        if (colorCoroutine != null) StopCoroutine(colorCoroutine);
        if (gameObject.activeInHierarchy)
            colorCoroutine = StartCoroutine(FadeEffect(normalTextColor, 0f));
    }

    private void OnDisable()
    {
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        if (colorCoroutine != null) StopCoroutine(colorCoroutine);
        
        transform.localScale = originalScale;
        isPointerInside = false;

        if (buttonText != null) buttonText.color = normalTextColor;
        if (glowImage != null) glowImage.color = new Color(glowColor.r, glowColor.g, glowColor.b, 0f);
    }

    private void AnimateScale(float targetScale)
    {
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        if (gameObject.activeInHierarchy)
            scaleCoroutine = StartCoroutine(ScaleAnimationRoutine(targetScale));
    }

    private IEnumerator ScaleAnimationRoutine(float targetScale)
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = originalScale * targetScale;
        float elapsed = 0f;

        while (elapsed < animDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / animDuration);
            float ease = scaleCurve.Evaluate(t);

            transform.localScale = Vector3.LerpUnclamped(startScale, endScale, ease);
            yield return null;
        }

        transform.localScale = endScale;
    }

    private IEnumerator FadeEffect(Color targetTextColor, float targetGlowAlpha)
    {
        while (true)
        {
            bool isDone = true;

            if (buttonText != null)
            {
                buttonText.color = Color.Lerp(buttonText.color, targetTextColor, Time.unscaledDeltaTime * colorFadeSpeed);
                if (Mathf.Abs(buttonText.color.a - targetTextColor.a) > 0.02f) isDone = false;
            }

            if (glowImage != null)
            {
                float currentAlpha = glowImage.color.a;
                float newAlpha = Mathf.Lerp(currentAlpha, targetGlowAlpha, Time.unscaledDeltaTime * colorFadeSpeed);
                glowImage.color = new Color(glowColor.r, glowColor.g, glowColor.b, newAlpha);
                if (Mathf.Abs(newAlpha - targetGlowAlpha) > 0.02f) isDone = false;
            }

            if (isDone) break;
            yield return null;
        }

        if (buttonText != null) buttonText.color = targetTextColor;
        if (glowImage != null) glowImage.color = new Color(glowColor.r, glowColor.g, glowColor.b, targetGlowAlpha);
    }
}
