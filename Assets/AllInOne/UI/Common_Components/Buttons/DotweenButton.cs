using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using Sirenix.OdinInspector;

public class DOTweenButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [Header("Button References")]
    public Button button;
    public RectTransform buttonTransform;
    public Image buttonImage;
    public CanvasGroup canvasGroup;

    [Header("Animation Settings")]
    [SerializeField]
    private ButtonStateSettings normalState = new ButtonStateSettings
    {
        scale = Vector3.one,
        color = Color.white,
        alpha = 1f,
        rotation = Vector3.zero
    };

    [SerializeField]
    private ButtonStateSettings hoverState = new ButtonStateSettings
    {
        scale = Vector3.one * 1.1f,
        color = Color.cyan,
        alpha = 1f,
        rotation = Vector3.zero
    };

    [SerializeField]
    private ButtonStateSettings pressedState = new ButtonStateSettings
    {
        scale = Vector3.one * 0.9f,
        color = Color.yellow,
        alpha = 0.8f,
        rotation = new Vector3(0, 0, 5f)
    };

    [SerializeField]
    private ButtonStateSettings disabledState = new ButtonStateSettings
    {
        scale = Vector3.one * 0.95f,
        color = Color.gray,
        alpha = 0.5f,
        rotation = Vector3.zero
    };

    [Header("Transition Settings")]
    public float transitionDuration = 0.2f;
    public Ease easeType = Ease.OutQuad;
    public bool useUnscaledTime = false;

    [Header("Special Effects")]
    public bool enablePunchScale = true;
    public Vector3 punchAmount = Vector3.one * 0.1f;
    public bool enableShake = false;
    public float shakeStrength = 10f;

    // Current state tracking
    private ButtonState currentState = ButtonState.Normal;
    private bool isPressed = false;
    private bool isHovered = false;

    // Tween sequences for cleanup
    private Sequence currentSequence;

    private enum ButtonState
    {
        Normal,
        Hover,
        Pressed,
        Disabled
    }

    [System.Serializable]
    public class ButtonStateSettings
    {
        public Vector3 scale = Vector3.one;
        public Color color = Color.white;
        [Range(0f, 1f)] public float alpha = 1f;
        public Vector3 rotation = Vector3.zero;
    }

    void Start()
    {
        InitializeButton();
        SetToNormalState(true); // Set initial state instantly
    }

    void InitializeButton()
    {
        // Auto-assign components if not set
        if (!button) button = GetComponent<Button>();
        if (!buttonTransform) buttonTransform = GetComponent<RectTransform>();
        if (!buttonImage) buttonImage = GetComponent<Image>();
        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();

        // Add CanvasGroup if missing (for alpha control)
        if (!canvasGroup)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    #region Pointer Event Handlers

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable) return;

        isHovered = true;
        if (!isPressed)
        {
            TransitionToState(ButtonState.Hover);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (!isPressed)
        {
            TransitionToState(ButtonState.Normal);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable) return;

        isPressed = true;
        TransitionToState(ButtonState.Pressed);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!button.interactable) return;

        isPressed = false;

        // Transition to hover if still hovering, otherwise normal
        if (isHovered)
        {
            TransitionToState(ButtonState.Hover);
        }
        else
        {
            TransitionToState(ButtonState.Normal);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!button.interactable) return;

        // Add click effects
        if (enablePunchScale)
        {
            buttonTransform.DOPunchScale(punchAmount, 0.3f, 5, 0.5f)
                .SetUpdate(useUnscaledTime);
        }

        if (enableShake)
        {
            buttonTransform.DOShakePosition(0.3f, shakeStrength, 10, 90, false, true)
                .SetUpdate(useUnscaledTime);
        }
    }

    #endregion

    #region State Management

    private void TransitionToState(ButtonState targetState)
    {
        if (currentState == targetState) return;

        currentState = targetState;

        ButtonStateSettings stateSettings = GetStateSettings(targetState);
        AnimateToState(stateSettings);
    }

    private ButtonStateSettings GetStateSettings(ButtonState state)
    {
        switch (state)
        {
            case ButtonState.Normal: return normalState;
            case ButtonState.Hover: return hoverState;
            case ButtonState.Pressed: return pressedState;
            case ButtonState.Disabled: return disabledState;
            default: return normalState;
        }
    }

    private void AnimateToState(ButtonStateSettings state)
    {
        // Kill current animation
        currentSequence?.Kill();

        // Create new sequence
        currentSequence = DOTween.Sequence();

        // Scale animation
        currentSequence.Join(buttonTransform.DOScale(state.scale, transitionDuration)
            .SetEase(easeType)
            .SetUpdate(useUnscaledTime));

        // Color animation
        if (buttonImage)
        {
            currentSequence.Join(buttonImage.DOColor(state.color, transitionDuration)
                .SetEase(easeType)
                .SetUpdate(useUnscaledTime));
        }

        // Alpha animation
        if (canvasGroup)
        {
            currentSequence.Join(canvasGroup.DOFade(state.alpha, transitionDuration)
                .SetEase(easeType)
                .SetUpdate(useUnscaledTime));
        }

        // Rotation animation
        if (state.rotation != Vector3.zero)
        {
            currentSequence.Join(buttonTransform.DORotate(state.rotation, transitionDuration)
                .SetEase(easeType)
                .SetUpdate(useUnscaledTime));
        }
        else
        {
            currentSequence.Join(buttonTransform.DORotate(Vector3.zero, transitionDuration)
                .SetEase(easeType)
                .SetUpdate(useUnscaledTime));
        }
    }

    private void SetToNormalState(bool instant = false)
    {
        if (instant)
        {
            buttonTransform.localScale = normalState.scale;
            buttonTransform.rotation = Quaternion.Euler(normalState.rotation);
            if (buttonImage) buttonImage.color = normalState.color;
            if (canvasGroup) canvasGroup.alpha = normalState.alpha;
        }
        else
        {
            TransitionToState(ButtonState.Normal);
        }
    }

    #endregion

    #region Public Methods

    [Button("Set Button Interactable")]
    public void SetButtonInteractable(bool interactable)
    {
        button.interactable = interactable;

        if (interactable)
        {
            TransitionToState(ButtonState.Normal);
        }
        else
        {
            currentState = ButtonState.Disabled;
            AnimateToState(disabledState);
        }
    }

    public void ResetToNormal()
    {
        isPressed = false;
        isHovered = false;
        SetToNormalState();
    }

    public void PlayClickAnimation()
    {
        if (enablePunchScale)
        {
            buttonTransform.DOPunchScale(punchAmount, 0.3f, 5, 0.5f)
                .SetUpdate(useUnscaledTime);
        }
    }

    #endregion

    void OnDestroy()
    {
        // Clean up tweens
        currentSequence?.Kill();
        buttonTransform?.DOKill();
        buttonImage?.DOKill();
        canvasGroup?.DOKill();
    }

    void OnDisable()
    {
        // Kill tweens when disabled
        currentSequence?.Kill();
    }
}