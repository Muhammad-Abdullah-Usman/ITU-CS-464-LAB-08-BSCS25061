using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// UIManager adds this to panel buttons at runtime.
// Child Scale must remain unchecked on the parent Vertical Layout Group.
[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public class UIButtonFeedback : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    Button button;
    Vector3 restingScale;
    Tween scaleTween;

    void Awake()
    {
        button = GetComponent<Button>();
        restingScale = transform.localScale;
        button.onClick.AddListener(PlayClick);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && button.IsInteractable())
            Animate(restingScale * 0.96f, 0.07f, Ease.OutQuad);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            Animate(restingScale, 0.12f, Ease.OutCubic);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Animate(restingScale, 0.12f, Ease.OutCubic);
    }

    void Animate(Vector3 target, float duration, Ease ease)
    {
        if (!isActiveAndEnabled)
            return;

        if (scaleTween != null)
            scaleTween.Kill();

        scaleTween = transform.DOScale(target, duration)
            .SetEase(ease)
            .SetUpdate(true)
            .OnComplete(() => scaleTween = null);
    }

    void OnDisable()
    {
        if (scaleTween != null)
            scaleTween.Kill();

        scaleTween = null;
        transform.localScale = restingScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.IsInteractable() && UIAudio.Instance != null)
            UIAudio.Instance.Hover();
    }

    void PlayClick()
    {
        // Back buttons get their own sound from UIManager.Back()
        if (UIAudio.Instance != null && gameObject.name != "BackButton")
            UIAudio.Instance.Click();
    }
}
