using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Toggle))]
public class SwitchToggle : MonoBehaviour
{
    [SerializeField] private Toggle _toggle;
    [Required]
    [SerializeField] private RectTransform _handle;
    [SerializeField] private Color _onBgColor;
    [SerializeField] private Color _onHandleColor;

    private Vector2 _handleStartPosition;
    private Color _defaultBgColor, _defaultHandleColor;
    private Image _bgImage, _handleImage;

    private void Awake()
    {
        if (_toggle == null)
        {
            _toggle = GetComponent<Toggle>();
        }

        _handleStartPosition = _handle.anchoredPosition;
        _bgImage = _toggle.targetGraphic as Image;
        _handleImage = _handle.GetComponent<Image>();

        _defaultBgColor = _bgImage.color;
        _defaultHandleColor = _handleImage.color;

        _toggle.onValueChanged.AddListener(OnToggled);
        OnToggled(_toggle.isOn);
    }

    private void OnToggled(bool isOn)
    {
        _handle.DOAnchorPos(isOn ? _handleStartPosition * -1 : _handleStartPosition, 0.4f).SetEase(Ease.InOutBack);
        _bgImage.DOColor(isOn ? _onBgColor : _defaultBgColor, 0.6f);
        _handleImage.DOColor(isOn ? _onHandleColor : _defaultHandleColor, 0.6f);
    }

    private void OnDestroy()
    {
        _toggle?.onValueChanged.RemoveListener(OnToggled); // Remove all if listen to more functions
    }
}
