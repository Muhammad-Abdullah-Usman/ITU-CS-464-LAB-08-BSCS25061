using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Keep this component on the always-active UIManager scene object.
// Existing titleScreen/mainMenuScreen references and button callbacks are preserved.
public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject titleScreen;
    [SerializeField] GameObject mainMenuScreen;

    [Header("Screen motion (unscaled seconds)")]
    [SerializeField, Min(0.01f)] float exitDuration = 0.12f;
    [SerializeField, Min(0.01f)] float enterDuration = 0.24f;
    [SerializeField, Min(0f)] float slideDistance = 110f;

    readonly Dictionary<GameObject, Vector2> homePositions =
        new Dictionary<GameObject, Vector2>();
    readonly Stack<GameObject> history = new Stack<GameObject>();

    GameObject currentScreen;
    Sequence transition;
    bool transitioning;

    public GameObject CurrentScreen => currentScreen;
    public bool IsTransitioning => transitioning;

    void Start()
    {
        if (titleScreen == null || mainMenuScreen == null)
        {
            Debug.LogError("UIManager: assign Title Screen and Main Menu Screen.", this);
            enabled = false;
            return;
        }

        RegisterScreen(titleScreen);
        RegisterScreen(mainMenuScreen);

        // Future screens should be sibling panels with a CanvasGroup.
        Transform screenRoot = titleScreen.transform.parent;
        if (screenRoot != null)
        {
            foreach (Transform child in screenRoot)
            {
                if (child.TryGetComponent<CanvasGroup>(out _))
                    RegisterScreen(child.gameObject);
            }
        }

        foreach (GameObject screen in homePositions.Keys)
            SetInstant(screen, screen == titleScreen);

        currentScreen = titleScreen;
    }

    public void ShowTitle()
    {
        if (Navigate(titleScreen, -1))
            history.Clear();
    }

    public void ShowMainMenu()
    {
        int direction = currentScreen == titleScreen ? 1 : -1;
        if (!Navigate(mainMenuScreen, direction))
            return;

        history.Clear();
        history.Push(titleScreen);
    }

    // Future Play, Equipment, etc. buttons can pass their panel to OpenScreen.
    public void OpenScreen(GameObject screen)
    {
        GameObject previous = currentScreen;
        if (Navigate(screen, 1) && previous != null)
        {
            history.Push(previous);
            if (UIAudio.Instance != null) UIAudio.Instance.Open();
        }
    }

    public void Back()
    {
        if (transitioning || history.Count == 0)
            return;

        GameObject previous = history.Peek();
        if (Navigate(previous, -1))
        {
            history.Pop();
            if (UIAudio.Instance != null) UIAudio.Instance.Back();
        }
    }

    bool RegisterScreen(GameObject screen)
    {
        if (screen == null || !screen.TryGetComponent<RectTransform>(out var rect))
            return false;

        if (homePositions.ContainsKey(screen))
            return true;

        homePositions.Add(screen, rect.anchoredPosition);
        if (!screen.TryGetComponent<CanvasGroup>(out _))
            screen.AddComponent<CanvasGroup>();

        // Apply the same feedback automatically, including on inactive buttons.
        foreach (Button button in screen.GetComponentsInChildren<Button>(true))
        {
            if (!button.TryGetComponent<UIButtonFeedback>(out _))
                button.gameObject.AddComponent<UIButtonFeedback>();
        }

        return true;
    }

    void SetInstant(GameObject screen, bool visible)
    {
        CanvasGroup group = screen.GetComponent<CanvasGroup>();
        group.alpha = visible ? 1f : 0f;
        group.interactable = visible;
        group.blocksRaycasts = visible;
        ((RectTransform)screen.transform).anchoredPosition = homePositions[screen];
        screen.SetActive(visible);
    }

    bool Navigate(GameObject target, int direction)
    {
        if (!isActiveAndEnabled || transitioning || target == currentScreen)
            return false;

        if (!RegisterScreen(target))
        {
            Debug.LogWarning("UIManager: navigation target must be a UI panel.", this);
            return false;
        }

        if (currentScreen == null || PlayerPrefs.GetInt("WanderingBlade.ReducedMotion", 0) == 1)
        {
            if (currentScreen != null)
            {
                SetInstant(currentScreen, false);
            }

            SetInstant(target, true);
            currentScreen = target;
            return true;
        }

        transitioning = true;
        GameObject outgoing = currentScreen;
        CanvasGroup outgoingGroup = outgoing.GetComponent<CanvasGroup>();
        CanvasGroup incomingGroup = target.GetComponent<CanvasGroup>();
        RectTransform outgoingRect = (RectTransform)outgoing.transform;
        RectTransform incomingRect = (RectTransform)target.transform;
        Vector2 outgoingHome = homePositions[outgoing];
        Vector2 incomingHome = homePositions[target];

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        outgoingGroup.interactable = false;
        outgoingGroup.blocksRaycasts = false;
        incomingGroup.interactable = false;
        incomingGroup.blocksRaycasts = false;
        incomingGroup.alpha = 0f;
        incomingRect.anchoredPosition = incomingHome +
            Vector2.right * (slideDistance * direction);
        target.SetActive(true);

        // Fade/slide out first, then bring in the destination without overlapping text.
        // SetUpdate(true) applies to the entire sequence and ignores Time.timeScale.
        transition = DOTween.Sequence().SetUpdate(true);
        transition
            .Append(outgoingGroup.DOFade(0f, exitDuration).SetEase(Ease.InCubic))
            .Join(outgoingRect.DOAnchorPos(
                outgoingHome - Vector2.right * (slideDistance * 0.5f * direction),
                exitDuration).SetEase(Ease.InCubic))
            .AppendCallback(() => outgoing.SetActive(false))
            .Append(incomingRect.DOAnchorPos(incomingHome, enterDuration)
                .SetEase(Ease.OutCubic))
            .Join(incomingGroup.DOFade(1f, enterDuration).SetEase(Ease.OutCubic))
            .OnComplete(() =>
            {
                outgoingRect.anchoredPosition = outgoingHome;
                incomingRect.anchoredPosition = incomingHome;
                incomingGroup.alpha = 1f;
                incomingGroup.interactable = true;
                incomingGroup.blocksRaycasts = true;
                currentScreen = target;
                transitioning = false;
                transition = null;
            });

        return true;
    }

    void OnDestroy()
    {
        if (transition != null)
            transition.Kill();
    }
}

